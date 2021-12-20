using System;
using System.IO;
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
        internal enum EDrawMode
        {
            NOT_SELECTED ,
            
            OBJECT ,
            DIRECTORY,
            
            NOT_INITIALIZED ,
        }
        
        private TopBar topBar;
        
        private ObjectField selected = default;
        private Toggle lockToggle = default;

        private ScrollView dependencies_container;
        private ScrollView used_by_container;

        private Label dependencies_label;
        private Label used_by_label;

#if UNITY_2020_1_OR_NEWER
        private HelpBox no_selection;
#endif
        private VisualElement additional_info;

        private Label versionTypeLabel;
        private Label lastModified;

        private Label PackageLabel;
        
        private Button substitute_button;

        private double lastUpdateTime;
        
        private bool needRebuild;

        private Object current;
        private EDrawMode drawMode;
        
        protected override void Constructor()
        {
            LoadLayout("ReferenceViewer");
            LoadStylesheet("ReferenceViewer");
            
            root.AddHeader();
            root.AddTopBar();
            
            QueryElements();
            InitElements();
            RefreshLocalizedText();

#if DEBUG_ASSETLENS
            // CreateFeatureButtons();
#endif
             
            dependencies_container.SetHorizontalVisibility(false);
            used_by_container.SetHorizontalVisibility(false);

            ConfigureSelection();
            OnDockingStateChange();
        }
        
        private void Awake()
        {
            L.onUpdate += OnLocalizationChange;
        }

        private void OnDestroy()
        {
            L.onUpdate -= OnLocalizationChange;
        }

        private void OnAddedAsTab()
        {
            OnDockingStateChange();
        }

        private void OnTabDetached()
        {
            OnDockingStateChange();
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

        private void QueryElements()
        {
            PackageLabel = root.Q<Label>("ci-label");
            topBar = root.Q<TopBar>("top-bar");
            selected = root.Q<ObjectField>("selection-field");

            lockToggle = root.Q<Toggle>("lock-toggle");

            dependencies_container = root.Q<ScrollView>("dependencies-container");
            used_by_container = root.Q<ScrollView>("used-by-container");
            
            dependencies_label = root.Q<Label>("dependencies-label");
            used_by_label = root.Q<Label>("used-by-label");

#if UNITY_2020_1_OR_NEWER
            // no_selection = root.Q<HelpBox>("no-selection-helpbox");
            no_selection = new HelpBox();
            root.Q<VisualElement>("help-box-container").Add(no_selection);
#endif
            additional_info = root.Q<VisualElement>("selection-info");

            versionTypeLabel = root.Q<Label>("version-info");
            lastModified = root.Q<Label>("modification-info");
        }

        private void InitElements()
        {
            selected.objectType = typeof(Object);
            selected.SetEnabled(false);
            
            topBar.closeButton.clickable.clicked += OnCloseButton;
            topBar.questionButton.clickable.clicked += OnQuestionButton;
        }

        private void OnCloseButton()
        {
            Close();
        }

        private void OnQuestionButton()
        {
            AssetLensConsole.Verbose(R.L("OnQuestionButton"));
        }

        private void RefreshLocalizedText()
        {
#if UNITY_2021_1_OR_NEWER
            no_selection.messageType = HelpBoxMessageType.Info;
            no_selection.text = L.Inst.inspector_nothing_selected;
#endif

            lockToggle.label = L.Inst.inspector_lockSelect;
            PackageLabel.text = L.Inst.DisplayName;
        }

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

        private void Update()
        {
            // temporal interval to refresh after compiling.
            // need to change initialize on load
            if (Time.realtimeSinceStartup - lastUpdateTime > 0.1f)
            {
                ConfigureSelection();
            }
        }

        private void OnSelectionChange()
        {
            needRebuild = true;
            ConfigureSelection();
        }

        private void OnFocus()
        {
            needRebuild = true;
            ConfigureSelection();
        }

        private void ConfigureSelection()
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            
            if (lockToggle == null || lockToggle.value)
            {
                return;
            }

            if (!Setting.IsEnabled)
            {
                // @TODO :: 초기화 되지 않음 페이지 뷰 보이기
                drawMode = EDrawMode.NOT_INITIALIZED;
                goto escape;
            }
            else
            {
                drawMode = EDrawMode.OBJECT;
            }
            
            current = Selection.activeObject;

            // when the selected object is a gameObject in the scene
            if (!Setting.Inst.ViewSceneObject && current is GameObject go && go.IsSceneObject())
            {
                needRebuild = true;
                
                // @TODO 
                // null 넣는 대신 씬 오브젝트임을 표기
                current = null;
                goto escape;
            }

            // object changed
            if (!ReferenceEquals(current, selected.value))
            {
                // current = selected.value;
                needRebuild = true;
            }

            // check changed
            if (current != null)
            {
                string guid = ReferenceUtil.GetGuid(current);
                string path = AssetDatabase.GetAssetPath(current);
                
                // if current object is not folder
                if (!Directory.Exists(path))
                {
                    if (ReferenceUtil.GUID.GetAssetCategory(guid) == EAssetCategory.Object)
                    {
                        // RefData data = RefData.Get(guid);
                    }    
                }

                drawMode = EDrawMode.NOT_SELECTED;
            }
            
            escape: ;
            
            if (needRebuild)
            {
                RebuildVisualElement();
                needRebuild = false;
            }
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
                drawMode = EDrawMode.NOT_SELECTED;
                DontDraw();
                
                return;
            }
            
            if (Setting.Inst.ViewIndexerVersion)
            {
                selected.style.display = DisplayStyle.Flex;
                additional_info.style.display = DisplayStyle.Flex;
            }

#if UNITY_2020_1_OR_NEWER
            no_selection.style.display = DisplayStyle.None;
#endif

            string path = AssetDatabase.GetAssetPath(current);
            string guid = AssetDatabase.AssetPathToGUID(path);

            if (Directory.Exists(path))
            {
                drawMode = EDrawMode.DIRECTORY;
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
                dependencies_container.Add(CreateRefDataButton(assetGuid));
            }
                    
            foreach (string assetGuid in data.referedByGuids)
            {
                used_by_container.Add(CreateRefDataButton(assetGuid));
            }

            dependencies_label.text = $"Dependencies ({data.ownGuids.Count})";
            used_by_label.text = $"Used By ({data.referedByGuids.Count})";
                    
            dependencies_label.style.visibility = data.ownGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            used_by_label.style.visibility = data.referedByGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
            
            void DontDraw()
            {
                // 그리지 않기
                if (drawMode == EDrawMode.NOT_SELECTED || drawMode == EDrawMode.DIRECTORY)
                {
                    dependencies_label.style.visibility = Visibility.Hidden;
                    used_by_label.style.visibility = Visibility.Hidden;
                
                    selected.style.display = DisplayStyle.None;
                    additional_info.style.display = DisplayStyle.None;                    
                }
                
#if UNITY_2020_1_OR_NEWER
                no_selection.style.display = DisplayStyle.Flex;

                if (drawMode == EDrawMode.NOT_INITIALIZED)
                {
                    no_selection.text = L.Inst.inspector_not_initialized;
                    no_selection.messageType = HelpBoxMessageType.Error;
                }
#endif
            }

            VisualElement CreateRefDataButton(string targetGuid)
            {
                // @TODO :: Template으로 빼기
                EAssetCategory assetCategory = ReferenceUtil.GUID.GetAssetCategory(targetGuid);

                string assetPath = AssetDatabase.GUIDToAssetPath(targetGuid);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                VisualElement buttonRoot = new VisualElement();
                buttonRoot.AddToClassList("reference-view-container");

                Image image = new Image();
                Button button = new Button(onClick);

                switch (assetCategory)
                {
                    case EAssetCategory.Object:
                        // @TODO : use uss style instead space

                        if (obj != null)
                        {
                            button.text = $"     {obj.name} ({ReferenceUtil.AddSpacesToSentence(obj.GetType().Name)})";
                            button.tooltip = assetPath;
                            Texture img = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
                            image.image = img;
                            image.AddToClassList("reference-view-image");    
                        }
                        else
                        {
                            button.text = $"     (null) (guid:{targetGuid})";
                            button.tooltip = assetPath;
                            Texture img = EditorGUIUtility.ObjectContent(null, typeof(Object)).image;
                            image.image = img;
                            image.AddToClassList("reference-view-image"); 
                        }
                        
                        break;
                    
                    case EAssetCategory.DefaultResource:
                        button.text = "Default Resource";
                        button.SetEnabled(false);
                        break;
                    
                    case EAssetCategory.BuiltInExtra:
                        button.text = "Built-in Resource";
                        button.SetEnabled(false);
                        break;
                    
                    case EAssetCategory.Others:
                        button.text = "Other Internals";
                        button.SetEnabled(false);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                button.AddToClassList("reference-view-button");

                void onClick()
                {
                    ReferenceUtil.Focus(obj);
                }

                buttonRoot.Add(button);
                button.Add(image);

                return buttonRoot;
            }
        }

#if DEBUG_ASSETLENS
        [MenuItem("Window/Asset Lens/Reference Viewer UIToolkit", false, 111)]
#endif
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
    }
}