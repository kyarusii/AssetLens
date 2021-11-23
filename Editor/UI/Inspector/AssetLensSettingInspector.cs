using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	using Reference;
	
	[CustomEditor(typeof(Setting))]
	public sealed class AssetLensSettingInspector : AssetLensInspector
	{
		private VisualElement options;
		private VisualElement buttons;
		
		private Toggle enabled;
		private DropdownField localization;

		#region Options Field

		private Label globalOptionHeader;
		private Label indexOptionHeader;
		private Label viewOptionHeader;
		private Label inspectorOptionHeader;

		private VisualElement IndexGroup;
		private Toggle IndexByGuidRegEx;
		private Toggle IndexSceneObject;
		private Toggle IndexPackageSubDir;
		
		private VisualElement ViewGroup;
		private Toggle ViewSceneObject;
		private Toggle ViewInPlayMode;
		private Toggle ViewIndexerVersion;
		private EnumField ViewObjectFocusMethod;
		
		private VisualElement InspectorGroup;
		private Toggle InspectorLensEnable;
		private Toggle InspectorHideWithNoLink;
		private Toggle InspectorDrawObjectInstanceId;
		
		#endregion

		#region Buttons Field

		private Button openIndexWizard;
		private Button resetSetting;
		private Button cleanupCache;
		private Button uninstallPackage;

		#endregion

		#region Help Field
		
		private Button openReadme;
		private Button documentation;
		private Button reportIssue;
		private Button changelog;
		private Button license;
		private Button credit;

		#endregion
		
		/// <summary>
		/// 뭔가 하고 있다!
		/// </summary>
		private bool working = false;
		
		protected override void Constructor()
		{
			LoadLayout("SettingInspector");

			options = root.Q<VisualElement>("options");
			buttons = root.Q<VisualElement>("buttons");

			IndexGroup = new VisualElement();
			ViewGroup = new VisualElement();
			InspectorGroup = new VisualElement();
			
			IndexGroup.AddToClassList("group");
			ViewGroup.AddToClassList("group");
			InspectorGroup.AddToClassList("group");

			enabled = new Toggle();
			localization = new DropdownField();

			InitOptions();
			InitButtons();
			InitHelp();
		}

		private void InitOptions()
		{
			globalOptionHeader = new Label();
			indexOptionHeader = new Label();
			viewOptionHeader = new Label();
			inspectorOptionHeader = new Label();

			// indexOptionHeader.value = true;
			// viewOptionHeader.value = true;
			// inspectorOptionHeader.value = true;

			IndexByGuidRegEx = new Toggle();
			IndexSceneObject = new Toggle();
			IndexPackageSubDir = new Toggle();
			
			ViewSceneObject = new Toggle();
			ViewInPlayMode = new Toggle();
			ViewIndexerVersion = new Toggle();
			ViewObjectFocusMethod = new EnumField();
			
			InspectorLensEnable = new Toggle();
			InspectorHideWithNoLink = new Toggle();
			InspectorDrawObjectInstanceId = new Toggle();
			
			/*
			 * Bind
			 */
			enabled.Bind(serializedObject);
			localization.Bind(serializedObject);
			
			IndexByGuidRegEx.Bind(serializedObject);
			IndexSceneObject.Bind(serializedObject);
			IndexPackageSubDir.Bind(serializedObject);
			
			ViewSceneObject.Bind(serializedObject);
			ViewInPlayMode.Bind(serializedObject);
			ViewIndexerVersion.Bind(serializedObject);
			ViewObjectFocusMethod.Bind(serializedObject);
			
			InspectorLensEnable.Bind(serializedObject);
			InspectorHideWithNoLink.Bind(serializedObject);
			InspectorDrawObjectInstanceId.Bind(serializedObject);
			
			/*
			 * Binding Path
			 */

			enabled.bindingPath = nameof(enabled);
			localization.bindingPath = nameof(localization);
			
			IndexByGuidRegEx.bindingPath = nameof(IndexByGuidRegEx);
			IndexSceneObject.bindingPath = nameof(IndexSceneObject);
			IndexPackageSubDir.bindingPath = nameof(IndexPackageSubDir);
			
			ViewSceneObject.bindingPath = nameof(ViewSceneObject);
			ViewInPlayMode.bindingPath = nameof(ViewInPlayMode);
			ViewIndexerVersion.bindingPath = nameof(ViewIndexerVersion);
			ViewObjectFocusMethod.bindingPath = nameof(ViewObjectFocusMethod);
			
			InspectorLensEnable.bindingPath = nameof(InspectorLensEnable);
			InspectorHideWithNoLink.bindingPath = nameof(InspectorHideWithNoLink);
			InspectorDrawObjectInstanceId.bindingPath = nameof(InspectorDrawObjectInstanceId);
			
			/*
			 * Add
			 */
			options.Add(globalOptionHeader);
			options.Add(enabled);
			options.Add(localization);
			
			options.Add(indexOptionHeader);
			options.Add(IndexGroup);
			
			IndexGroup.Add(IndexByGuidRegEx);
			IndexGroup.Add(IndexSceneObject);
			IndexGroup.Add(IndexPackageSubDir);
			
			options.Add(viewOptionHeader);
			options.Add(ViewGroup);
			
			ViewGroup.Add(ViewSceneObject);
			ViewGroup.Add(ViewInPlayMode);
			ViewGroup.Add(ViewIndexerVersion);
			ViewGroup.Add(ViewObjectFocusMethod);
			
			options.Add(inspectorOptionHeader);
			options.Add(InspectorGroup);
			
			InspectorGroup.Add(InspectorLensEnable);
			InspectorGroup.Add(InspectorHideWithNoLink);
			InspectorGroup.Add(InspectorDrawObjectInstanceId);
			
			globalOptionHeader.AddToClassList("header-1");
			indexOptionHeader.AddToClassList("header-2");
			viewOptionHeader.AddToClassList("header-2");
			inspectorOptionHeader.AddToClassList("header-2");
			
			/*
			* localized text
			*/

			enabled.label = Localize.Inst.setting_enabled;
			localization.label = Localize.Inst.setting_language;
			
			globalOptionHeader.text = "Setting";
			indexOptionHeader.text = Localize.Inst.IndexOptionLabel;
			viewOptionHeader.text = Localize.Inst.ViewOptionLabel;
			inspectorOptionHeader.text = Localize.Inst.InspectorOptionLabel;

			IndexByGuidRegEx.label = Localize.Inst.IndexByGuidRegExLabel;
			IndexSceneObject.label = Localize.Inst.IndexSceneObjectLabel;
			IndexPackageSubDir.label = Localize.Inst.IndexPackageSubDirLabel;
			
			ViewSceneObject.label = Localize.Inst.ViewSceneObjectLabel;
			ViewInPlayMode.label = Localize.Inst.ViewInPlayModeLabel;
			ViewIndexerVersion.label = Localize.Inst.ViewIndexerVersionLabel;
			ViewObjectFocusMethod.label = Localize.Inst.ViewObjectFocusMethodLabel;
			
			InspectorLensEnable.label = Localize.Inst.InspectorLensEnableLabel;
			InspectorHideWithNoLink.label = Localize.Inst.InspectorHideWithNoLinkLabel;
			InspectorDrawObjectInstanceId.label = Localize.Inst.InspectorDrawObjectInstanceIdLabel;

			localization.choices = Setting.GetLanguageChoices();
			
			/*
			 * Callbacks
			 */

			// indexOptionHeader.RegisterValueChangedCallback(OnIndexOptionHeaderChange);
			// viewOptionHeader.RegisterValueChangedCallback(OnViewOptionHeaderChange);
			// inspectorOptionHeader.RegisterValueChangedCallback(OnInspectorOptionHeaderChange);
		}

		private void OnViewOptionHeaderChange(ChangeEvent<bool> evt)
		{
			ViewGroup.SetEnabled(evt.newValue);
		}

		private void OnInspectorOptionHeaderChange(ChangeEvent<bool> evt)
		{
			InspectorGroup.SetEnabled(evt.newValue);
		}

		private void OnIndexOptionHeaderChange(ChangeEvent<bool> evt)
		{
			IndexGroup.SetEnabled(evt.newValue);
		}

		private void InitButtons()
		{
			openIndexWizard = root.Q<Button>("open-index-wizard");
			resetSetting = root.Q<Button>("reset-setting");
			cleanupCache = root.Q<Button>("clean-cached-indecies");
			uninstallPackage = root.Q<Button>("uninstall-package");

			openIndexWizard.text = Localize.Inst.OpenIndexWizard;
			resetSetting.text = Localize.Inst.ResetSetting;
			cleanupCache.text = Localize.Inst.CleanCachedIndicies;
			uninstallPackage.text = Localize.Inst.UninstallPackage;
			
			openIndexWizard.clickable.clicked += OnOpenIndexWizard;
			resetSetting.clickable.clicked += OnResetSetting;
			cleanupCache.clickable.clicked += CleanupCacheData;
			uninstallPackage.clickable.clicked += OnUninstallPackage;
			
			resetSetting.SetEnabled(false);
		}

		private void InitHelp()
		{
			openReadme = root.Q<Button>("open-readme");
			documentation = root.Q<Button>("documentation");
			reportIssue = root.Q<Button>("report-issue");
			changelog = root.Q<Button>("changelog");
			license = root.Q<Button>("license");
			credit = root.Q<Button>("credit");

			openReadme.text = Localize.Inst.OpenReadme;
			documentation.text = Localize.Inst.Documentation;
			reportIssue.text = Localize.Inst.ReportIssue;
			changelog.text = Localize.Inst.ChangeLog;
			license.text = Localize.Inst.License;
			credit.text = Localize.Inst.Credit;

			openReadme.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/README.md#quickstart");
			documentation.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens");
			reportIssue.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/issues");
			changelog.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/CHANGELOG.md");
			license.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/LICENSE");
			credit.clickable.clicked += () => Application.OpenURL("https://github.com/seonghwan-dev");
			
			documentation.SetEnabled(false);
		}

		private async void OnUninstallPackage()
		{
			if (working) return;
			
			working = true;
			Setting.IsEnabled = false;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Log(R.L($"{processedAssetCount} asset caches removed!"));
			
			Directory.Delete(FileSystem.ReferenceCacheDirectory);

#if DEBUG_ASSETLENS
			
			string projectManifest = await File.ReadAllTextAsync(FileSystem.Manifest);
			if (!projectManifest.Contains(Constants.PackageName))
			{
				AssetLensConsole.Log(R.L("Cannot be uninstalled under development."));
				return;
			}
#endif

			// @TODO :: NEED TEST
			string resultMessage = await PackageSystem.Uninstall();
			working = false;
			
			AssetLensConsole.Log(R.L(resultMessage));
		}

		private async void CleanupCacheData()
		{
			if (working) return;
			
			working = true;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Log($"{processedAssetCount} asset caches removed!");

			working = false;
		}

		private void OnResetSetting()
		{
			
		}

		private void OnOpenIndexWizard()
		{
			AssetLensIndexWizard.Open();
		}
	}
}