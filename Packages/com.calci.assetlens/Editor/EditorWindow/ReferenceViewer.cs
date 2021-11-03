using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AssetLens.Reference
{
    public class ReferenceViewer : EditorWindow
    {
        private const string UXML = "Packages/com.calci.assetlens/Editor/EditorWindow/ReferenceViewer.uxml";
        private const string USS = "Packages/com.calci.assetlens/Editor/EditorWindow/ReferenceViewer.uss";

        private ObjectField selected = default;
        private Toggle lockToggle = default;

        private ScrollView dependencies_container;
        private ScrollView used_by_container;

        private Label dependencies_label;
        private Label used_by_label;

        private VisualElement no_selection;
        private VisualElement additional_info;

        private Label versionTypeLabel;
        private Label namePathLabel;
        private Label lastModified;

        private double lastUpdateTime;
        
        private bool needRebuild;

        private Object current;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML);

            VisualElement mainUXML = visualTree.Instantiate();
            root.Add(mainUXML);

            selected = root.Q<ObjectField>("selectedObject");
            selected.objectType = typeof(Object);
            selected.SetEnabled(false);

            lockToggle = root.Q<Toggle>("lockToggle");

            dependencies_container = root.Q<ScrollView>("dependencies-container");
            used_by_container = root.Q<ScrollView>("used-by-container");
            
            dependencies_label = root.Q<Label>("dependencies-label");
            used_by_label = root.Q<Label>("used-by-label");

            no_selection = root.Q<VisualElement>("no-selection");
            additional_info = root.Q<VisualElement>("additional-info");

            versionTypeLabel = new Label();
            namePathLabel = new Label();
            lastModified = new Label();
            
            additional_info.Add(versionTypeLabel);
            // additional_info.Add(namePathLabel);
            additional_info.Add(lastModified);

            dependencies_container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            used_by_container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            
            ConfigureSelection();
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
            
            current = Selection.activeObject;

            // when the selected object is a gameObject in the scene
            if (!Setting.TraceSceneObject && current is GameObject go && go.IsSceneObject())
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
                        RefData data = RefData.Get(guid);
                    }    
                }
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
                DontDraw();
                return;
            }
            
            if (Setting.DisplayIndexerVersion)
            {
                selected.style.display = DisplayStyle.Flex;
                no_selection.style.display = DisplayStyle.None;
                additional_info.style.display = DisplayStyle.Flex;
            }
            
            string path = AssetDatabase.GetAssetPath(current);
            string guid = AssetDatabase.AssetPathToGUID(path);

            if (Directory.Exists(path))
            {
                return;
            }

            RefData data = RefData.Get(guid);

            versionTypeLabel.text = $"{data.GetVersionText()} ({data.objectType})";
            // namePathLabel.text = $"{data.objectName} ({data.objectPath})";
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
                dependencies_label.style.visibility = Visibility.Hidden;
                used_by_label.style.visibility = Visibility.Hidden;

                no_selection.style.display = DisplayStyle.Flex;
                selected.style.display = DisplayStyle.None;
                additional_info.style.display = DisplayStyle.None;
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
                            Texture img = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
                            image.image = img;
                            image.AddToClassList("reference-view-image");    
                        }
                        else
                        {
                            button.text = $"     (null) (guid:{targetGuid}) {assetPath}";
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
#if UNITY_2021_1_OR_NEWER
                    EditorUtility.OpenPropertyEditor(obj);
#else
                    Selection.activeObject = obj;
#endif
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