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
			set
			{
				_context = value;
			}
		}

		public string name = "Language";
		
		public string indexing_title = nameof(indexing_title);
		public string indexing_message = nameof(indexing_message);
		public string indexing_proceed = nameof(indexing_proceed);
		public string indexing_cancel = nameof(indexing_cancel);

		public string setting_enabled = nameof(setting_enabled);
		public string setting_pauseInPlaymode = nameof(setting_pauseInPlaymode);
		public string setting_traceSceneObjects = nameof(setting_traceSceneObjects);
		public string setting_useEditorUtilityWhenSearchDependencies = nameof(setting_useEditorUtilityWhenSearchDependencies);
		public string setting_language = nameof(setting_language);
	}
}