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
#if UNITY_2020_3_OR_NEWER
            VisualElement mainUXML = visualTree.Instantiate();
#else
            VisualElement mainUXML = visualTree.CloneTree();
#endif
            root.Add(mainUXML);
            
            /*
             * 물어볼 것들
             *
             * 에셋 렌즈
             * 사용 여부
             *
             * 레퍼런스 뷰어
             * - 씬 오브젝트 탐지 여부
             * - 플레이모드에서 활성화 여부
             * - 디펜던시 탐지에 에디터 유틸리티 함수를 사용할지
             *
             * 인스펙터 렌즈
             * - 사용 여부
             * - 씬 오브젝트 탐지 여부
             * - 표시할 프로퍼티 선택
             *
             * 세이프 딜리트
             * - 사용 여부
             * - 화이트 리스트 (경로) - 미구현
             *
             * 빌드 렌즈 (미구현)
             * - 사용 여부
             * - 코드 검사
             * - 어드레서블
             * - 에셋 번들
             * - 빌드된 씬에 간접 연결
             * - 리소스 폴더에 간접 연결
             */
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