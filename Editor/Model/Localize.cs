using System;

namespace RV
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
					_context = ReferenceSetting.LoadLocalization;
				}

				return _context;
			}
			set => _context = value;
		}

		public string name = "Language";

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

		public string dialog_noIndexedData = nameof(dialog_noIndexedData);
		public string dialog_indexedAssetExpired = nameof(dialog_indexedAssetExpired);
		public string dialog_enablePlugin = nameof(dialog_enablePlugin);
		public string dialog_disablePlugin = nameof(dialog_disablePlugin);
	}
}