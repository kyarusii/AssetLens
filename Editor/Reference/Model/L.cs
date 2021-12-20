using System;

namespace AssetLens
{
	using Reference;
	
	[Serializable]
	internal sealed class L
	{
		private static DateTime lastModifiedTime;
		private static L _context = default;

		public static L Inst {
			get
			{
				// not loaded yet
				if (_context == null)
				{
					lastModifiedTime = Setting.GetLocalizationLastWriteTime();
					_context = Setting.LoadLocalization;
					onUpdate(_context);
					
					AssetLensConsole.Log(R.D("Localization Create"));
				}

				// file change check
				if (lastModifiedTime != Setting.GetLocalizationLastWriteTime())
				{
					lastModifiedTime = Setting.GetLocalizationLastWriteTime();
					_context = Setting.LoadLocalization;
					onUpdate(_context);
					
					
					AssetLensConsole.Log(R.D("Localization Reloaded"));
				}

				return _context;
			}
			set
			{
				_context = value;
				onUpdate(_context);
			}
		}

		public static event Action<L> onUpdate = delegate(L localize) {  };

		public string name = "Language";
		public string DisplayName = "Asset Lens";

		public string guid = "guid";

		public string indexing_title = nameof(indexing_title);
		public string indexing_message = nameof(indexing_message);
		public string indexing_proceed = nameof(indexing_proceed);
		public string indexing_cancel = nameof(indexing_cancel);

		public string processing_title = nameof(processing_title);

		public string setting_enabled = nameof(setting_enabled);
		public string setting_language = nameof(setting_language);

		public string fmt_inspector_usageCount_singular = "{0} usage";
		public string fmt_inspector_usageCount_multiple = "{0} usages";
		public string fmt_inspector_dependencies = "Dependencies : {0}";
		public string fmt_inspector_referencedBy = "Usages : {0}";
		
		public string inspector_nothing_selected = "Nothing selected";
		public string inspector_not_initialized = "Please initialize asset lens first.";
		public string inspector_selected = "Selected";
		public string inspector_missingObject = "Missing Object";
		public string inspector_editorUtilityMissingObjectHelpBox = nameof(inspector_editorUtilityMissingObjectHelpBox);
		public string inspector_lockSelect = "Lock";
		public string inspector_sceneObjectHelpbox = "Disabled on Scene Object";
		public string inspector_buildInResources = "Built-in Resources";
		public string inspector_playmodeHelpBox = "Disabled in PlayMode";

		public string remove_messageContent = "This asset is used by other asset. \nWill you delete this asset?";
		public string remove_titleContent = "Warning!";
		public string remove_removeProceed = "Remove";
		public string remove_removeCancel = "Cancel";
		public string remove_cancelAlert = "Canceled.";

		public string assets_has_dependencies_format = "<color=#7FFF00>{0} uses {1}</color>";
		public string assets_is_referenced_by_format = "<color=#7FFF00>{0} is used by {1}</color>";

		#region Setting

		/*
		 * Index
		 */
		public string IndexOptionLabel = "Indexing Options";
		
		public string IndexByGuidRegExLabel = "IndexByGuidRegLabel";
		public string IndexSceneObjectLabel = "IndexSceneObject";
		public string IndexPackageSubDirLabel = "IndexPackageSubDir";
		
		public string IndexByGuidRegExTooltip = "IndexByGuidRegTooltip";
		public string IndexSceneObjectTooltip = "IndexSceneObjectTooltip";
		public string IndexPackageSubDirTooltip = "IndexPackageSubDirTooltip";

		/*
		 * View
		 */
		public string ViewOptionLabel = "Viewer Options";
		public string ViewSceneObjectLabel = "Enable when scene object selected";
		public string ViewInPlayModeLabel = "Enable in PlayMode";
		public string ViewIndexerVersionLabel = "Show indexer version";
		public string ViewObjectFocusMethodLabel = "Reference Focus Method";

		/*
		 * Inspector Lens
		 */

		public string InspectorOptionLabel = "Inspector Lens Options";
		public string InspectorLensEnableLabel = "Enable";
		public string InspectorHideWithNoLinkLabel = "Don't draw when no dependency";
		public string InspectorDrawObjectInstanceIdLabel = "Draw Instance ID";
		
		/*
		 * Logging
		 */

		public string LogOptionLabel = "Logging Options";
		public string LogReferenceAdd = "Print Reference Add Log";
		public string LogReferenceRemove = "Print Reference Remove Log";
		public string LogReferenceAddMessage = "Reference Added!";
		public string LogReferenceRemoveMessage = "Reference Removed!";
		
		#endregion

		public string IndexWizard_StatusConsoleLabel = "Plugin Status";
		
		public string IndexWizard_EntranceTooltip =
			"Is this your first time using AssetLens? Press the button to check the quick start guide.";
		public string IndexWizard_OpenWhenProjectStartup = "Always open when you start a project.";
		public string IndexWizard_StatusLabel = "Status : {0}";
		public string IndexWizard_StatusReadyToUse = "Ready To Use";
		public string IndexWizard_StatusNotInitialized = "Not Initialized";
		public string IndexWizard_ManagedAssetLabel = "Managed Asset : {0}";
		public string IndexWizard_Title = "Index Wizard";
		
		public string Close = "Close";
		public string Proceed = "Proceed";
		public string Generate = "Generate";
		public string Regenerate = "Regenerate";
		public string Start = "Start";
		public string Save = "Save";

		public string OpenIndexWizard = "Open Index Wizard";
		public string ResetSetting = "Reset Settings to Default";
		public string CleanCachedIndices = "Clean up Cached Indicies";
		public string UninstallPackage = "Uninstall Asset Lens";

		public string OpenReadme = "Getting Started";
		public string Documentation = "Documentation";
		public string ReportIssue = "Report Issue";
		public string ChangeLog = "Change Log";
		public string License = "License";
		public string Credit = "Credit";
	}
}