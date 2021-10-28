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

        private double lastUpdateTime;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML);

            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            selected = root.Q<ObjectField>("selectedObject");
            selected.objectType = typeof(Object);
            selected.SetEnabled(false);

            lockToggle = root.Q<Toggle>("lockToggle");

            dependencies_container = root.Q<ScrollView>("dependencies-container");
            used_by_container = root.Q<ScrollView>("used-by-container");
            dependencies_label = root.Q<Label>("dependencies-label");
            used_by_label = root.Q<Label>("used-by-label");

            dependencies_container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            used_by_container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
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
            ConfigureSelection();
        }

        private void ConfigureSelection()
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            
            if (lockToggle == null || lockToggle.value)
            {
                return;
            }
            
            Object current = Selection.activeObject;
            if (!ReferenceEquals(current, selected.value))
            {
                selected.value = current;
            
                // update view
                dependencies_container.Clear();
                used_by_container.Clear();

                if (selected.value != null)
                {
                    var path = AssetDatabase.GetAssetPath(selected.value);
                    var guid = AssetDatabase.AssetPathToGUID(path);

                    if (Directory.Exists(path))
                    {
                        return;
                    }

                    RefData data = RefData.Get(guid);

                    foreach (string assetGuid in data.ownGuids)
                    {
                        // @TODO :: Template으로 빼기
                        EAssetCategory assetCategory = ReferenceUtil.GUID.GetAssetCategory(assetGuid);
                        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                        VisualElement buttonRoot = new VisualElement();
                        buttonRoot.AddToClassList("reference-view-container");
                        
                        Image image = new Image();
                        Button button = new Button(onClick);

                        switch (assetCategory)
                        {
                            case EAssetCategory.Object:
                                // @TODO : use uss style instead space
                                button.text = $"      {obj.name} ({ReferenceUtil.AddSpacesToSentence(obj.GetType().Name)})";
                                Texture img = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
                                image.image = img;
                                image.AddToClassList("reference-view-image");
                                
                                break;
                            case EAssetCategory.DefaultResource:
                                button.text = "Default Resource";
                                button.SetEnabled(false);
                                break;
                            case EAssetCategory.BuiltInExtra:
                                button.text = "Built-in Resource";
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
                        
                        dependencies_container.Add(buttonRoot);
                    }
                    
                    foreach (string assetGuid in data.referedByGuids)
                    {
                        // @TODO :: Template으로 빼기
                        EAssetCategory assetCategory = ReferenceUtil.GUID.GetAssetCategory(assetGuid);
                        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                        VisualElement buttonRoot = new VisualElement();
                        buttonRoot.AddToClassList("reference-view-container");

                        Image image = new Image();
                        Button button = new Button(onClick);

                        switch (assetCategory)
                        {
                            case EAssetCategory.Object:
                                // @TODO : use uss style instead space
                                button.text = $"      {obj.name} ({ReferenceUtil.AddSpacesToSentence(obj.GetType().Name)})";
                                Texture img = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
                                image.image = img;
                                image.AddToClassList("reference-view-image");

                                break;
                            case EAssetCategory.DefaultResource:
                                button.text = "Default Resource";
                                button.SetEnabled(false);
                                break;
                            case EAssetCategory.BuiltInExtra:
                                button.text = "Built-in Resource";
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
                        
                        used_by_container.Add(buttonRoot);
                    }

                    dependencies_label.text = $"Dependencies ({data.ownGuids.Count})";
                    used_by_label.text = $"Used By ({data.ownGuids.Count})";
                    
                    dependencies_label.style.visibility = data.ownGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                    used_by_label.style.visibility = data.referedByGuids.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                }
                else
                {
                    dependencies_label.style.visibility = Visibility.Hidden;
                    used_by_label.style.visibility = Visibility.Hidden;
                }
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