using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AssetLens.UI
{
    using Reference;
    using Component;

    internal sealed class ReferenceViewer : AssetLensEditorWindow
    {
        public enum EDisplayMode
        {
            Undefined,
            
            Empty,

            GameObject,
            SceneObject,
            Directory,
            
            ActivationGuide,
            InvalidDataWarning,
        }
        
        private TopBar topBar;
        
        private ObjectField selected = default;
        private Toggle lockToggle = default;

        private ScrollView dependencies_container;
        private ScrollView used_by_container;

        private Label dependencies_label;
        private Label used_by_label;

        private CustomHelpBox messageBox;
        private VisualElement additional_info;

        private Label versionTypeLabel;
        private Label lastModified;

        private Label PackageLabel;
        
        private Button substitute_button;

        private VisualElement sub_ActivationGuide;
        private VisualElement sub_Directory;
        private VisualElement sub_EmptyPanel;
        private VisualElement sub_GameObject;
        private VisualElement sub_InvalidData;
        private VisualElement sub_SceneObject;

        private Label sub_CustomHelpBox;

        private double lastUpdateTime;
        
        private Object current;
        private EDisplayMode displayMode = EDisplayMode.Undefined;
        
        private bool forceUpdate = false;
        private bool afterReloadScripts = false;
        private bool initialized = false;

        #region Unity Event
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ReloadScriptCallback()
        {
            var view = Resources.FindObjectsOfTypeAll<ReferenceViewer>();
            if (view != null && view.Length > 0)
            {
                AssetLensConsole.Log(R.D($"Reload Scripts : {view.Length}"));
                
                view.First().RegisterCallbacks();
                view.First().afterReloadScripts = true;
            }
        }

        private void Awake()
        {
            RegisterCallbacks();
        }

        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        private void OnAddedAsTab()
        {
            OnDockingStateChange();
        }

        private void OnTabDetached()
        {
            OnDockingStateChange();
        }
        
        private void Update()
        {
            if (!Setting.Inst.ViewRefreshOnUpdate)
            {
                return;
            }

            if (!initialized)
            {
                return;
            }
            
            // temporal interval to refresh after compiling.
            // need to change initialize on load
            // if (Time.realtimeSinceStartup - lastUpdateTime > Setting.Inst.ViewRefreshRate.AsSecond())
            if (Time.realtimeSinceStartup - lastUpdateTime > 0.2f)
            {
                try
                {
                    if (!initialized) return;
                    
                    UpdateData();
                }
                catch (Exception e)
                {
#if DEBUG_ASSETLENS
                    AssetLensConsole.Log(R.D(e.ToString()));
#else
                    AssetLensConsole.Warn(R.L(e.ToString()));
#endif
                }
            }
        }

        private void OnSelectionChange()
        {
            forceUpdate = true;
            UpdateData();
        }

        private void OnFocus()
        {
            UpdateData();
        }

        #endregion

        #region Callbacks

        private void OnSettingUpdate()
        {
            forceUpdate = true;
            UpdateData();
        }
        
        private void OnLocalizationChange(L l)
        {
            RefreshLocalizedText();
        }

        private void OnDockingStateChange()
        {
#if UNITY_2020_1_OR_NEWER
            if (docked)
            {
                topBar.SetEnabled(true);
            }
            else
            {
                topBar.SetEnabled(false);
            }
#endif
        }
        
        private void OnCloseButton()
        {
            Close();
        }

        private void OnQuestionButton()
        {
            AssetLensConsole.Verbose(R.L("OnQuestionButton"));
        }

        #endregion

        #region Overrides

        protected override void Constructor()
        {
            Cleanup();
            
            LoadLayout("ReferenceViewer");
            LoadStylesheet("ReferenceViewer");
            
            root.AddHeader();
            root.AddTopBar();
            // root.AddSwitchToggle();

            CreateBody();
            SetupSubBody();
            
            InitElements();  
            RefreshLocalizedText();

            dependencies_container.SetHorizontalVisibility(false);
            used_by_container.SetHorizontalVisibility(false);

            forceUpdate = true;
            UpdateData(); 
            OnDockingStateChange();
            SetPanel(); 
            SetMessage();

            initialized = true;
            
            AssetLensConsole.Log(R.D("RV::Constructor"));
        }

        #endregion

        private void SetMessage()
        {
            switch (displayMode)
            {
                case EDisplayMode.SceneObject:
                    messageBox.SetVisible(true);
                    
                    if (Setting.Inst.ViewSceneObject)
                    {
                        messageBox.messageType = HelpBoxMessageType.Info;
                        messageBox.text = "(Experimental Feature)";
                    }
                    else
                    {
                        messageBox.messageType = HelpBoxMessageType.Info;
                        messageBox.text = "Scene object view setting is off.";
                    }
                    break;
                case EDisplayMode.Directory:
                    messageBox.SetVisible(true);
                    messageBox.text = "Current selection is the folder.";
                    messageBox.messageType = HelpBoxMessageType.Info;
                    break;
                case EDisplayMode.ActivationGuide:
                    messageBox.SetVisible(true);
                    messageBox.text = L.Inst.inspector_not_initialized;
                    messageBox.messageType = HelpBoxMessageType.Error;
                    break;
                case EDisplayMode.InvalidDataWarning:
                    messageBox.SetVisible(true);
                    messageBox.text = "Invalid cached data!";
                    messageBox.messageType = HelpBoxMessageType.Warning;
                    break;
                case EDisplayMode.Empty:
                    messageBox.SetVisible(true);
                    messageBox.text = L.Inst.inspector_nothing_selected;
                    messageBox.messageType = HelpBoxMessageType.Info;
                    break;
                
                case EDisplayMode.GameObject:
                case EDisplayMode.Undefined:
                default:
                    messageBox.SetVisible(false);
                    break;
            }
        }

        private void Cleanup()
        {
            current = null;
            if (selected != null)
            {
                selected.value = null;
            }
        }

        private void RegisterCallbacks()
        {
            L.onUpdate += OnLocalizationChange;
            Setting.onSettingChange += OnSettingUpdate;
        }

        private void UnregisterCallbacks()
        {
            L.onUpdate -= OnLocalizationChange;
            Setting.onSettingChange -= OnSettingUpdate;
            
        }

        private void CreateBody()
        {
            var body = root.Q<VisualElement>("body");
            sub_ActivationGuide = GetLayout("SubPanel/RV_ActivationGuide").CopyTree();
            body.Add(sub_ActivationGuide);

            sub_Directory = GetLayout("SubPanel/RV_Directory").CopyTree();
            body.Add(sub_Directory);
            
            sub_EmptyPanel = GetLayout("SubPanel/RV_Empty").CopyTree();
            body.Add(sub_EmptyPanel);
            
            sub_GameObject = GetLayout("SubPanel/RV_GameObject").CopyTree();
            body.Add(sub_GameObject);
            
            sub_InvalidData = GetLayout("SubPanel/RV_InvalidDataWarning").CopyTree();
            body.Add(sub_InvalidData);
            
            sub_SceneObject = GetLayout("SubPanel/RV_SceneObject").CopyTree();
            body.Add(sub_SceneObject);
        }

        private void SetupSubBody()
        {
            PackageLabel = root.Q<Label>("ci-label");
            topBar = root.Q<TopBar>("top-bar");
            selected = root.Q<ObjectField>("selection-field");

            lockToggle = root.Q<Toggle>("lock-toggle");

            dependencies_container = root.Q<ScrollView>("dependencies-container");
            used_by_container = root.Q<ScrollView>("used-by-container");
            
            dependencies_label = root.Q<Label>("dependencies-label");
            used_by_label = root.Q<Label>("used-by-label");

// #if UNITY_2020_1_OR_NEWER
            // no_selection = root.Q<HelpBox>("no-selection-helpbox");
            messageBox = new CustomHelpBox();
            root.Q<VisualElement>("help-box-container").Add(messageBox);
// #endif
            additional_info = root.Q<VisualElement>("selection-info");

            versionTypeLabel = root.Q<Label>("version-info");
            lastModified = root.Q<Label>("modification-info");

            sub_CustomHelpBox = sub_EmptyPanel.Q<Label>("message");
        }

        private void InitElements()
        {
            selected.objectType = typeof(Object);
            selected.SetEnabled(false);
            
            topBar.closeButton.clickable.clicked += OnCloseButton;
            topBar.questionButton.clickable.clicked += OnQuestionButton;
        }

        private void RefreshLocalizedText()
        {
// #if UNITY_2021_1_OR_NEWER
            // messageBox.messageType = HelpBoxMessageType.Info;
            // messageBox.text = L.Inst.inspector_nothing_selected;
// #endif
            sub_CustomHelpBox.text = L.Inst.inspector_nothing_selected;

            lockToggle.label = L.Inst.inspector_lockSelect;
            PackageLabel.text = L.Inst.DisplayName;
        }

        private bool IsTargetLocked()
        {
            return lockToggle == null || lockToggle.value;
        }

        /// <summary>
        /// 새로 그려야 하는지 확인해야 하는 상황
        /// 1. 선택 변경
        /// 2. 리컴파일
        /// 3. 설정 변경
        /// 3.1 현지화 변경
        /// </summary>
        private void UpdateData()
        {
            var previousMode = displayMode;
            var previousObject = current;
            
            DecideMode();

            if (previousMode != displayMode)
            {
                RebuildVisualElement();
                SetPanel();

                // AssetLensConsole.Log(R.D($"Mode Change : ({previousMode}) -> ({displayMode})"));
            }
            else if (previousObject != current)
            {
                RebuildGameObjectPanel();
                
                // @TODO :: Temporal implementation
                RebuildVisualElement(); 
                
                // AssetLensConsole.Log(R.D($"Object Change : ({previousObject}) -> ({current})"));
            }
            
            if (afterReloadScripts)
            {
                AssetLensConsole.Log(R.D($"Script Reloads!"));
                try
                {
                    RebuildVisualElement();
                    RebuildGameObjectPanel();
                    SetPanel();
                }
                catch (Exception e)
                {
                    AssetLensConsole.Log(R.D(e.ToString()));
                }
                
                afterReloadScripts = false;
            }
            
            SetMessage();
        }

        private void SetPanel()
        {
            sub_GameObject.style.display = DisplayStyle.Flex;
            sub_ActivationGuide.style.display = DisplayStyle.None;
            sub_Directory.style.display = DisplayStyle.None;
            sub_EmptyPanel.style.display = DisplayStyle.None;
            sub_InvalidData.style.display = DisplayStyle.None;
            sub_SceneObject.style.display = DisplayStyle.None;
            return;
            
            // sub_ActivationGuide.style.display = displayMode == EDisplayMode.ActivationGuide
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
            // sub_Directory.style.display = displayMode == EDisplayMode.Directory
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
            // sub_EmptyPanel.style.display = displayMode == EDisplayMode.Empty
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
            // sub_GameObject.style.display = displayMode == EDisplayMode.GameObject
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
            // sub_InvalidData.style.display = displayMode == EDisplayMode.InvalidDataWarning
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
            // sub_SceneObject.style.display = displayMode == EDisplayMode.SceneObject
            //     ? DisplayStyle.Flex
            //     : DisplayStyle.None;
        }

        /// <summary>
        /// 오브젝트 선택 확인
        /// </summary>
        private void DecideMode()
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            
            // 1. 플러그인 활성화 여부 확인
            // 2. 선택된 오브젝트가 있는지 확인
            // 3. 디렉터리인지 확인
            // 4. 유효한 에셋 데이터가 있는지 확인
            // 5. 씬 오브젝트인지 확인
            // 6. 그리기
            
            // 1. Check Plugin Activation
            if (!Setting.IsEnabled)
            {
                displayMode = EDisplayMode.ActivationGuide;
                // 활성화 가이드를 띄울것이므로 현재 선택 오브젝트 정보 파기
                
                current = null;
                return;
            }

            // 타겟이 고정되어 있으면 아무 작업 안 함
            if (IsTargetLocked())
            {
                return;
            }

            var activeObject = Selection.activeObject;
            
            // 2. Check NULL
            if (activeObject == null)
            {
                displayMode = EDisplayMode.Empty;
                current = null;
                return;
            }

            // 변경되지 않았음! - 스킵 업데이트
            if (ReferenceEquals(activeObject, current) && !forceUpdate)
            {
                return;
            }
            
            if (forceUpdate)
            {
                forceUpdate = false;
                // AssetLensConsole.Log(R.D($"ReferenceEquals Skipped! ({current}) > ({activeObject})"));
            }
            
            string path = AssetDatabase.GetAssetPath(activeObject);
            string guid = AssetDatabase.AssetPathToGUID(path);

            current = activeObject;

            // 3. Check directory
            if (Directory.Exists(path))
            {
                displayMode = EDisplayMode.Directory;
                return;
            }
            
            // 4. Validate asset data
            if (!RefData.CacheExist(guid))
            {
                if (Setting.Inst.DataCreateIfDataInvalid)
                {
                    var data = RefData.New(guid);
                    data.Save();
                    
                    AssetLensConsole.Log(R.D($"Created {guid}.ref because of invalid data."));
                    displayMode = EDisplayMode.GameObject;
                }
                else
                {
                    displayMode = EDisplayMode.InvalidDataWarning;
                }
                
                return;
            }
            
            // 5. Check Scene Object (Persistence)
            if (Setting.Inst.ViewSceneObject && activeObject is GameObject go && go.IsSceneObject())
            {
                // scene object mode
                displayMode = EDisplayMode.SceneObject;
                return;
            }

            displayMode = EDisplayMode.GameObject;
        }

        private void RebuildGameObjectPanel()
        {
            
        }

        private void RebuildVisualElement()
        {
            selected.value = current;
            
            // clear previous visual element
            dependencies_container.Clear();
            used_by_container.Clear();

            // when selected object is null
            if (null == current)
            {
                DontDraw();
                
                return;
            }
            
            if (Setting.Inst.ViewIndexerVersion)
            {
                selected.style.display = DisplayStyle.Flex;
                additional_info.style.display = DisplayStyle.Flex;
            }

            SetMessage();
            
            string path = AssetDatabase.GetAssetPath(current);
            string guid = AssetDatabase.AssetPathToGUID(path);
            
            if (Directory.Exists(path))
            {
                DontDraw();
                
                return;
            }

            RefData data = RefData.Get(guid);

            var versionText = data.GetVersionText();
            var objectType = data.objectType;

            versionTypeLabel.text = $"{versionText} ({objectType})";
            lastModified.text = $"Last Modified : {data.GetLastEditTime()}";

            foreach (string assetGuid in data.ownGuids)
            {
                // dependencies_container.Add(CreateRefDataButton(assetGuid));
                dependencies_container.Add(new AssetReference(assetGuid));
            }
                    
            foreach (string assetGuid in data.referedByGuids)
            {
                // used_by_container.Add(CreateRefDataButton(assetGuid));
                used_by_container.Add(new AssetReference(assetGuid));
            }

            dependencies_label.text = $"Dependencies ({data.ownGuids.Count})";
            used_by_label.text = $"Used By ({data.referedByGuids.Count})";
                    
            dependencies_label.style.visibility = data.ownGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            used_by_label.style.visibility = data.referedByGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            
            dependencies_container.SetVisible(data.ownGuids.Count > 0);
            used_by_container.SetVisible(data.referedByGuids.Count > 0);
            
            void DontDraw()
            {
                // 그리지 않기
                // if (drawMode == EDrawMode.NOT_SELECTED || drawMode == EDrawMode.DIRECTORY)
                {
                    dependencies_label.style.visibility = Visibility.Hidden;
                    used_by_label.style.visibility = Visibility.Hidden;
                
                    selected.style.display = DisplayStyle.None;
                    additional_info.style.display = DisplayStyle.None;                    
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ReferenceViewer GetWindow()
        {
            ReferenceViewer wnd = GetWindow<ReferenceViewer>();
            wnd.titleContent = new GUIContent("Reference Viewer");
            wnd.minSize = new Vector2(380, 400);

            wnd.Focus();
            wnd.Repaint();

            wnd.Show();
            return wnd;
        }

        #region Experimental

        private void CreateFeatureButtons()
        {
            var featureRoot = root.Q<VisualElement>("feature-buttons");
            substitute_button = new Button();
            featureRoot.Add(substitute_button);

            // substitute_button = root.Q<Button>("substitute-button");
            substitute_button.clicked += () =>
            {
                string openFile = EditorUtility.OpenFilePanel("replace", "Assets", "");
                if (!string.IsNullOrWhiteSpace(openFile))
                {
                    var newGuid = AssetDatabase.AssetPathToGUID(openFile);

                    RefData newData = RefData.Get(newGuid);

                    var path = AssetDatabase.GetAssetPath(selected.value);
                    var guid = AssetDatabase.AssetPathToGUID(path);

                    RefData data = RefData.Get(guid);

                    AssetDatabase.StartAssetEditing();
                    foreach (string referedByGuid in data.referedByGuids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(referedByGuid);
                        var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                        if (obj != null)
                        {
                            RefData refData = RefData.Get(referedByGuid);

                            refData.ownGuids.Remove(guid);
                            refData.ownGuids.Add(newGuid);

                            newData.referedByGuids.Add(referedByGuid);

                            string assetContent = File.ReadAllText(assetPath);
                            string newAssetContent = assetContent.Replace(guid, newGuid);

                            File.WriteAllText(assetPath, newAssetContent);
                            AssetDatabase.ImportAsset(assetPath);

                            refData.Save();
                        }
                    }

                    AssetDatabase.StopAssetEditing();

                    newData.Save();
                    AssetDatabase.SaveAssets();
                }
            };
        }
        
        #endregion
    }
}