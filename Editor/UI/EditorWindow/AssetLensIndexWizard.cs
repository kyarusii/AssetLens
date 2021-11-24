using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	using Reference;
	using Component;

	public sealed class AssetLensIndexWizard : AssetLensEditorWindow
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

		#region Elements

		private VisualElement main;

		private TopBar topBar;
		private Label startupSwitchLabel;
		private Toggle startupSwitch;

		private Label indexByGuidRegExLabel;
		private Label indexSceneObjectLabel;
		private Label indexPackageSubDirLabel;

		private Toggle option1;
		private Toggle option2;
		private Toggle option3;

		private Label optionLeftTitle;
		private Label optionRightTitle;

		private DropdownField language;

		private Label statusLabel;
		private Label managedAssetLabel;
		private Button proceedButton;
		private Button closeButton;

		#endregion

		#region Transition

		private double initTime;
		private bool started;
		private bool complete;
		
		private const float delay = 0.8f;
		private const float interval = 0.4f;

		private readonly List<VisualElement> blending = new List<VisualElement>();

		#endregion

		#region Unity

		private void Update()
		{
			if (!complete)
			{
				var now = EditorApplication.timeSinceStartup;
				if (!started)
				{
					if (now - initTime > delay)
					{
						HandleTransition();
						started = true;
					}
				}
				else
				{
					if (now - initTime > interval)
					{
						HandleTransition();
					}
				}

				void HandleTransition()
				{
					var ve = blending.FirstOrDefault();

					if (ve == default)
					{
						complete = true;
						return;
					}
						
					ve.SetEnabled(true);
					blending.RemoveAt(0);

					initTime = now;
				}
			}
		}

		#endregion

		#region Core

		protected override void Constructor()
		{
			initTime = EditorApplication.timeSinceStartup;

			LoadLayout("IndexWizard");

			QueryElements();
			ConfigureElements();
			RefreshLocalizedText();
			BindCallbacks();
		}

		private void QueryElements()
		{
			main = root.Q<VisualElement>("main");
			topBar = root.Q<TopBar>("header");
			
			startupSwitchLabel = root.Q<Label>("label-display-on-startup");
			startupSwitch = root.Q<Toggle>("toggle-display-on-startup");
			
			indexByGuidRegExLabel = root.Q<Label>("option-1-label");
			indexSceneObjectLabel = root.Q<Label>("option-2-label");
			indexPackageSubDirLabel = root.Q<Label>("option-3-label");
			
			option1 = root.Q<Toggle>("option-1");
			option2 = root.Q<Toggle>("option-2");
			option3 = root.Q<Toggle>("option-3");
			language = root.Q<DropdownField>("language-dropdown");
			
			optionLeftTitle = root.Q<Label>("option-label-left");
			optionRightTitle = root.Q<Label>("option-label-right");
			
			statusLabel = root.Q<Label>("status-label");
			managedAssetLabel = root.Q<Label>("managed-asset-label");
			
			proceedButton = root.Q<Button>("proceed-btn");
			closeButton = root.Q<Button>("close-btn");

			blending.Add(root.Q<VisualElement>("header"));
			blending.Add(root.Q<VisualElement>("title"));
			blending.Add(root.Q<VisualElement>("title-image"));
			blending.Add(root.Q<VisualElement>("display-on-startup"));
			blending.Add(root.Q<VisualElement>("column-header"));
			blending.Add(root.Q<VisualElement>("column"));
		}

		/// <summary>
		/// 초기값 세팅, 로드
		/// Set up initial values and load saved values.
		/// </summary>
		private void ConfigureElements()
		{
			topBar.Remove(topBar.closeButton);
			startupSwitch.SetValueWithoutNotify(GetStartupSwitchValue());

			language.choices = Setting.GetLanguageChoices();
			int selected = language.choices.IndexOf(Setting.Localization);
			language.index = selected;
			
			option1.SetValueWithoutNotify(Setting.Inst.IndexByGuidRegEx);
			option2.SetValueWithoutNotify(Setting.Inst.IndexSceneObject);
			option3.SetValueWithoutNotify(Setting.Inst.IndexPackageSubDir);

			foreach (VisualElement visualElement in blending)
			{
				visualElement.SetEnabled(false);
			}
		}

		private void RefreshLocalizedText()
		{
			/*
			 * Header
			 */
			topBar.questionButton.tooltip = L.Inst.IndexWizard_EntranceTooltip;
			startupSwitchLabel.text = L.Inst.IndexWizard_OpenWhenProjectStartup;

			/*
			 * Column Header
			 */
			optionLeftTitle.text = L.Inst.IndexOptionLabel;
			optionRightTitle.text = L.Inst.IndexWizard_StatusConsoleLabel;

			/*
			 * Left Columns
			 */
			indexByGuidRegExLabel.text = L.Inst.IndexByGuidRegExLabel;
			indexByGuidRegExLabel.tooltip = L.Inst.IndexByGuidRegExTooltip;
			indexSceneObjectLabel.text = L.Inst.IndexSceneObjectLabel;
			indexSceneObjectLabel.tooltip = L.Inst.IndexSceneObjectTooltip;
			indexPackageSubDirLabel.text = L.Inst.IndexPackageSubDirLabel;
			indexPackageSubDirLabel.tooltip = L.Inst.IndexPackageSubDirTooltip;
			
			language.label = L.Inst.setting_language;

			/*
			 * Right Columns
			 */
			statusLabel.text = string.Format(L.Inst.IndexWizard_StatusLabel,
				Setting.IsEnabled ? Setting.Inst.SuccessColorCode : Setting.Inst.ErrorColorCode,
				Setting.IsEnabled ? L.Inst.IndexWizard_StatusReadyToUse : L.Inst.IndexWizard_StatusNotInitialized
			);

			var files = AssetLensCache.GetIndexedFiles();
			managedAssetLabel.text = string.Format(L.Inst.IndexWizard_ManagedAssetLabel, files.Length);

			closeButton.text = L.Inst.Close;
			proceedButton.text = Setting.IsEnabled ? L.Inst.Regenerate : L.Inst.Generate;
		}

		private void BindCallbacks()
		{
			startupSwitch.RegisterValueChangedCallback(OnStartupSwitchChange);
			topBar.questionButton.clicked += OnQuestionMark;

			option1.RegisterValueChangedCallback(OnIndexByGuidChanged);
			option2.RegisterValueChangedCallback(OnIndexSceneObjectChanged);
			option3.RegisterValueChangedCallback(OnIndexPackageSubDirChanged);

			language.RegisterValueChangedCallback(OnLanguageChange);
			
			proceedButton.clickable.clicked += OnProceedButton;
			closeButton.clickable.clicked += OnCloseButton;
		}

		#endregion

		#region Callbacks

		private async void OnProceedButton()
		{
			AssetDatabase.SaveAssets();
			AssetLensConsole.Log(R.D("인덱싱 시작"));

			await AssetLensCache.IndexAssetsAsync();
			Setting.IsEnabled = true;

			AssetLensConsole.Log(R.D("인덱싱 끝"));

			// 정보 리로드
			RefreshLocalizedText();
			
			// @TODO :: 다른 옵션 열기
			Close();
			
			// Focus Setting
			ReferenceUtil.Focus(Setting.Inst, EObjectFocusMethod.Selection);
		}

		private void OnCloseButton()
		{
			Close();
		}

		private void OnLanguageChange(ChangeEvent<string> evt)
		{
			if (language.choices.Contains(evt.newValue))
			{
				Setting.Localization = evt.newValue;
				L.Inst = Setting.LoadLocalization;

				RefreshLocalizedText();
				AssetLensConsole.Log(R.D($"OnLanguageChanged : {evt.previousValue} -> {evt.newValue}"));
			}
			else
			{
				AssetLensConsole.Log(R.D($"Skip Change Event : Invalid Language ({evt.newValue})"));
			}
		}

		/// <summary>
		/// GUID 기반 검색
		/// </summary>
		/// <param name="evt"></param>
		private void OnIndexByGuidChanged(ChangeEvent<bool> evt)
		{
			AssetLensConsole.Log(R.D($"IndexByGuidRegEx : {evt.previousValue} -> {evt.newValue}"));

			Setting.Inst.IndexByGuidRegEx = evt.newValue;
		}

		/// <summary>
		/// 씬 오브젝트 인덱싱
		/// </summary>
		/// <param name="evt"></param>
		private void OnIndexSceneObjectChanged(ChangeEvent<bool> evt)
		{
			AssetLensConsole.Log(R.D($"IndexSceneObject : {evt.previousValue} -> {evt.newValue}"));
			
			Setting.Inst.IndexSceneObject = evt.newValue;
		}

		/// <summary>
		/// 패키지 인덱싱 포함
		/// </summary>
		/// <param name="evt"></param>
		private void OnIndexPackageSubDirChanged(ChangeEvent<bool> evt)
		{
			AssetLensConsole.Log(R.D($"IndexPackageSubDir : {evt.previousValue} -> {evt.newValue}"));
			
			Setting.Inst.IndexPackageSubDir = evt.newValue;
		}

		private void OnStartupSwitchChange(ChangeEvent<bool> evt)
		{
			string startUpKey = $"{Application.productName}.AssetLens.Configuration.ShowOnStartUp";
			EditorPrefs.SetBool(startUpKey, evt.newValue);

			AssetLensConsole.Log(R.D($"Display On Startup ValueChange : {evt.previousValue} > {evt.newValue}"));
		}

		private void OnQuestionMark()
		{
			Application.OpenURL("https://github.com/seonghwan-dev/assetlens");
		}

		#endregion

		#region Utility

		private bool GetStartupSwitchValue()
		{
			string startUpKey = $"{Application.productName}.AssetLens.Configuration.ShowOnStartUp";
			return EditorPrefs.GetBool(startUpKey, true);
		}
		
		public static void Open()
		{
			AssetLensIndexWizard wnd = GetWindow<AssetLensIndexWizard>();

			const int width = 680;
			const int height = 480;

			wnd.titleContent = new GUIContent(L.Inst.IndexWizard_Title);
			wnd.minSize = wnd.maxSize = new Vector2(width, height);
			wnd.Show();
		}

		#endregion
	}
}