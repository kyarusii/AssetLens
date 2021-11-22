using System;
using System.Collections.Generic;
using System.IO;
using AssetLens.Reference.Component;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.Reference
{
	public class ConfigurationWizard : UIWindow
	{
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

		private VisualElement main;
		
		private TopBar topBar;
		private Label startupSwitchLabel;
		private Toggle startupSwitch;

		private Label option1Label;
		private Label option2Label;
		private Label option3Label;

		private Toggle option1;
		private Toggle option2;
		private Toggle option3;

		private Label optionLeftTitle;
		private Label optionRightTitle;
		
		private DropdownField language;

		private Label statusLabel;
		private Label managedAssetLabel;
		private Button proceedButton;

		private double initTime;
		private bool initialized;
		private float delay = 1.0f;

		private List<VisualElement> blending = new List<VisualElement>();

		private void Update()
		{
			if (!initialized)
			{
				if (EditorApplication.timeSinceStartup - initTime > delay)
				{
					foreach (VisualElement visualElement in blending)
					{
						visualElement.SetEnabled(true);
					}
					
					initialized = true;
				}
			}
		}

		protected override void Constructor()
		{
			initTime = EditorApplication.timeSinceStartup;
			
			LoadLayout("ConfigurationWizard");
			
			QueryElements();
			ConfigureElements();
			SetLocalizedNames();
			BindCallbacks();
		}

		private void QueryElements()
		{
			main = root.Q<VisualElement>("main");
			topBar = root.Q<TopBar>("header");
			startupSwitchLabel = root.Q<Label>("label-display-on-startup");
			startupSwitch = root.Q<Toggle>("toggle-display-on-startup");
			option1Label = root.Q<Label>("option-1-label");
			option2Label = root.Q<Label>("option-2-label");
			option3Label = root.Q<Label>("option-3-label");
			option1 = root.Q<Toggle>("option-1");
			option2 = root.Q<Toggle>("option-2");
			option3 = root.Q<Toggle>("option-3");
			language = root.Q<DropdownField>("language-dropdown");
			optionLeftTitle = root.Q<Label>("option-label-left");
			optionRightTitle = root.Q<Label>("option-label-right");
			statusLabel = root.Q<Label>("status-label");
			managedAssetLabel = root.Q<Label>("managed-asset-label");
			proceedButton = root.Q<Button>("proceed-btn");
			
			blending.Add(root.Q<VisualElement>("header"));
			blending.Add(root.Q<VisualElement>("title"));
			blending.Add(root.Q<VisualElement>("title-image"));
			blending.Add(root.Q<VisualElement>("display-on-startup"));
			blending.Add(root.Q<VisualElement>("column-header"));
			blending.Add(root.Q<VisualElement>("column"));
		}

		private void ConfigureElements()
		{
			topBar.Remove(topBar.closeButton);
			startupSwitch.value = GetStartupSwitchValue();
			
			language.choices = GetLanguageChoieces();
			int selected = language.choices.IndexOf(Setting.Localization);
			language.index = selected;

			foreach (VisualElement visualElement in blending)
			{
				visualElement.SetEnabled(false);
			}
		} 

		private void SetLocalizedNames()
		{
			topBar.questionButton.tooltip = "처음이신가요? \n이 버튼을 눌러 빠른 시작 가이드를 확인하세요.";
			startupSwitchLabel.text = "프로젝트 시작 시 항상 열기";
			
			/*
			 * Header
			 */
			optionLeftTitle.text = "옵션";
			optionRightTitle.text = "상태";
			
			/*
			 * Left Columns
			 */
			option1Label.text = "GUID 기반 검색";
			option1Label.tooltip = "Off시 유니티 내장 EditorUtility 사용";
			option2Label.text = "Scene 오브젝트 탐지 허용";
			option2Label.tooltip = "인스펙터 그리기가 다소 느려질 수 있음";
			option3Label.text = "캐시 데이터 자동 업데이트";
			option3Label.tooltip = "캐시된 데이터를 읽을 때, 이전 버전이면 자동으로 업데이트 합니다.";
			language.label = "Language";

			/*
			 * Right Columns
			 */
			statusLabel.text = string.Format("Status : <color={0}>{1}</color>",
				Setting.IsEnabled ? "green" : "red",
				Setting.IsEnabled ? "Ready To Use" : "No Cached Data"
			);
			
			DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.ReferenceCacheDirectory);
			var files = rootDirectory.GetFiles("*.ref");
			managedAssetLabel.text = string.Format("Managed Asset : {0}", files.Length);

			proceedButton.text = "Create Cache";
		}

		private void BindCallbacks()
		{
			startupSwitch.RegisterValueChangedCallback(OnStartupSwitchChange);
			topBar.questionButton.clicked += OnQuestionMark;

			option1.RegisterValueChangedCallback(OnOption1);
			option2.RegisterValueChangedCallback(OnOption1);
			option3.RegisterValueChangedCallback(OnOption1);

			language.RegisterValueChangedCallback(OnLanguageChange);
			proceedButton.clickable.clicked += OnProceedButton;
		}

		private async void OnProceedButton()
		{
			Debug.Log("Proceed Button");
			await AssetLensCache.IndexAssetsAsync();
			Setting.IsEnabled = true;
			Debug.Log("Proceed Done");
			
			SetLocalizedNames();
		}

		private void OnLanguageChange(ChangeEvent<string> evt)
		{
			Debug.Log($"OnLanguageChanged : {evt.previousValue} -> {evt.newValue}");
		}

		private List<string> GetLanguageChoieces()
		{
			string languageDir = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages");
			
			List<string> localNames = new List<string>();
			string[] languageFiles = Directory.GetFiles(languageDir, "*.json");
			foreach (string file in languageFiles)
			{
				FileInfo fi = new FileInfo(file);
				localNames.Add(fi.Name.Replace(".json", ""));
			}
			
			return localNames;
		}

		private void OnOption1(ChangeEvent<bool> evt)
		{
			Debug.Log($"OnOption : {evt.previousValue} -> {evt.newValue}");
		}

		private bool GetStartupSwitchValue()
		{
			string startUpKey = $"{Application.productName}.AssetLens.Configuration.ShowOnStartUp";
			return EditorPrefs.GetBool(startUpKey, true);
		}
		private void OnStartupSwitchChange(ChangeEvent<bool> evt)
		{
			string startUpKey = $"{Application.productName}.AssetLens.Configuration.ShowOnStartUp";
			EditorPrefs.SetBool(startUpKey, evt.newValue);
			
			Debug.Log($"New Value : {evt.newValue}");
		}

		private void OnQuestionMark()
		{
			Application.OpenURL("https://github.com/seonghwan-dev/assetlens");
		}

#if DEBUG_ASSETLENS
		[MenuItem("Window/Asset Lens/Configuration Wizard", false, 140)]
#endif
		public static void Open()
		{
			ConfigurationWizard wnd = GetWindow<ConfigurationWizard>();

			const int width = 680; 
			const int height = 480;
			
			wnd.titleContent = new GUIContent("Config Wizard");
			wnd.minSize = wnd.maxSize = new Vector2(width, height);
			wnd.Show();
		}
	}
}