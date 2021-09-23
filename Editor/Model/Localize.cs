using System;

namespace AssetLens
{
	[Serializable]
	public class Localize
	{
		private static Localize _context = default;

		public static Localize Inst {
			get
			{
				if (_context == null)
				{
					_context = AssetLensSetting.LoadLocalization;
				}

				return _context;
			}
			set
			{
				_context = value;
				onUpdate(_context);
			}
		}

		public static event Action<Localize> onUpdate = delegate(Localize localize) {  };

		public string name = "Language";
		public string displayName = "Asset Lens";

		public string guid = "guid";

		public string indexing_title = nameof(indexing_title);
		public string indexing_message = nameof(indexing_message);
		public string indexing_proceed = nameof(indexing_proceed);
		public string indexing_cancel = nameof(indexing_cancel);

		public string processing_title = nameof(processing_title);

		public string setting_enabled = nameof(setting_enabled);
		public string setting_pauseInPlaymode = nameof(setting_pauseInPlaymode);
		public string setting_traceSceneObjects = nameof(setting_traceSceneObjects);

		public string setting_useEditorUtilityWhenSearchDependencies =
			nameof(setting_useEditorUtilityWhenSearchDependencies);

		public string setting_language = nameof(setting_language);
		public string setting_workflow = nameof(setting_workflow);
		public string setting_miscellaneous = nameof(setting_miscellaneous);
		public string setting_unlockDangerzone = nameof(setting_unlockDangerzone);
		public string setting_dangerzone = nameof(setting_dangerzone);
		public string setting_cleanUpCache = nameof(setting_cleanUpCache);
		public string setting_uninstall = nameof(setting_uninstall);

		public string dialog_titleContent = nameof(dialog_titleContent);
		public string dialog_noIndexedData = nameof(dialog_noIndexedData);
		public string dialog_indexedAssetExpired = nameof(dialog_indexedAssetExpired);
		public string dialog_enablePlugin = nameof(dialog_enablePlugin);
		public string dialog_disablePlugin = nameof(dialog_disablePlugin);

		public string fmt_inspector_usageCount_singular = "{0} usage";
		public string fmt_inspector_usageCount_multiple = "{0} usages";
		public string fmt_inspector_dependencies = "Dependencies : {0}";
		public string fmt_inspector_referencedBy = "Usages : {0}";
		
		public string inspector_nothing_selected = "Nothing selected";
		public string inspector_selected = "Selected";
		public string inspector_missingObject = "Missing Object";
		public string inspector_editorUtilityMissingObjectHelpBox = nameof(inspector_editorUtilityMissingObjectHelpBox);
		public string inspector_lockSelect = "Lock";
		public string inspector_sceneObjectHelpbox = "Disabled on Scene Object";
		public string inspector_buildInResources = "Built-in Resources";
		public string inspector_playmodeHelpBox = "Disabled in PlayMode";

		public string inspector_notInitializeHelpBox = "Asset Lens is not initialized";
		public string inspector_generateIndex = "Initialize";
	}
}