using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.Reference
{
    /// <summary>
    /// Sortable Multi Column Reference Viewer Window
    /// </summary>
    public class MultiColumnReferenceViewer : EditorWindow
    {
        private const string UXML = "Packages/com.calci.assetlens/Editor/EditorWindow/MultiColumnReferenceViewer.uxml";
        
#if DEBUG_ASSETLENS
        [MenuItem("Window/Asset Lens/Multi Column Reference Viewer", false, 111)]
#endif
        public static MultiColumnReferenceViewer ShowExample()
        {
            MultiColumnReferenceViewer wnd = GetWindow<MultiColumnReferenceViewer>();

            wnd.titleContent = new GUIContent("Multi Column Reference Viewer");
            wnd.minSize = new Vector2(800, 300);

            return wnd;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML);
#if UNITY_2020_3_OR_NEWER
            VisualElement editorWindowTree = visualTree.Instantiate();
#else
            VisualElement editorWindowTree = visualTree.CloneTree();
#endif
            
            root.Add(editorWindowTree);
        }
    }
}