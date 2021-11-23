using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	using Reference;
	
	[CustomEditor(typeof(Setting))]
	public class AssetLensSettingInspector : AssetLensInspector
	{
		private VisualElement options;
		private VisualElement buttons;
		
		private Toggle enabled;
		private DropdownField localization;
		
		private Label globalOptionHeader;
		private Label indexOptionHeader;
		private Label viewOptionHeader;
		private Label inspectorOptionHeader;

		private Toggle IndexByGuidRegEx;
		private Toggle IndexSceneObject;
		private Toggle IndexPackageSubDir;
		
		private Toggle ViewSceneObject;
		private Toggle ViewInPlayMode;
		
		private Toggle InspectorLensEnable;
		private Toggle InspectorHideWithNoLink;
		private Toggle InspectorDrawObjectInstanceId;
		
		
		protected override void Constructor()
		{
			LoadLayout("SettingInspector");

			options = root.Q<VisualElement>("options");
			buttons = root.Q<VisualElement>("buttons");

			enabled = new Toggle();
			localization = new DropdownField();

			globalOptionHeader = new Label();
			indexOptionHeader = new Label();
			viewOptionHeader = new Label();
			inspectorOptionHeader = new Label();

			IndexByGuidRegEx = new Toggle();
			IndexSceneObject = new Toggle();
			IndexPackageSubDir = new Toggle();
			
			ViewSceneObject = new Toggle();
			ViewInPlayMode = new Toggle();
			
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
			options.Add(IndexByGuidRegEx);
			options.Add(IndexSceneObject);
			options.Add(IndexPackageSubDir);
			
			options.Add(viewOptionHeader);
			options.Add(ViewSceneObject);
			options.Add(ViewInPlayMode);
			
			options.Add(inspectorOptionHeader);
			options.Add(InspectorLensEnable);
			options.Add(InspectorHideWithNoLink);
			options.Add(InspectorDrawObjectInstanceId);
			
			globalOptionHeader.AddToClassList("header");
			indexOptionHeader.AddToClassList("header");
			viewOptionHeader.AddToClassList("header");
			inspectorOptionHeader.AddToClassList("header");
			
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
			
			InspectorLensEnable.label = Localize.Inst.InspectorLensEnableLabel;
			InspectorHideWithNoLink.label = Localize.Inst.InspectorHideWithNoLinkLabel;
			InspectorDrawObjectInstanceId.label = Localize.Inst.InspectorDrawObjectInstanceIdLabel;

			localization.choices = Setting.GetLanguageChoices();
		}
	}
}