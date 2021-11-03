using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AssetLens.Reference
{
    public class ConfigurationWizard : EditorWindow
    {
        private const string UXML = "Packages/com.calci.assetlens/Editor/EditorWindow/ConfigurationWizard.uxml";
        private const string USS = "Packages/com.calci.assetlens/Editor/EditorWindow/ConfigurationWizard.uss";

        [MenuItem("Window/UI Toolkit/ConfigurationWizard")]
        public static void ShowExample()
        {
            ConfigurationWizard wnd = GetWindow<ConfigurationWizard>();
            wnd.titleContent = new GUIContent("ConfigurationWizard");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML);
            VisualElement mainUXML = visualTree.Instantiate();
            root.Add(mainUXML);
            
            
        }
        
#if DEBUG_ASSETLENS
        [MenuItem("Window/Asset Lens/Configuration Wizard", false, 140)]
#endif
        public static ReferenceViewer GetWindow()
        {
            ReferenceViewer wnd = GetWindow<ReferenceViewer>();
            wnd.titleContent = new GUIContent("Configuration Wizard");
            wnd.minSize = new Vector2(380, 400);

            wnd.Focus();
            wnd.Repaint();

            wnd.Show();
            return wnd;
        }
    }
}