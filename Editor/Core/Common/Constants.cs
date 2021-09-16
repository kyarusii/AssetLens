namespace RV
{
	public static class Constants
	{
#if LANGUAGE_KR
		public const string TOOL = "Tools/레퍼런스";

		public const string MENU_INDEX_ALL_ASSETS = TOOL + "/모든 에셋 인덱싱";
		public const string MENU_LOG_REFERENCE = TOOL + "/선택한 오브젝트 로깅";
		
		public const string WINDOW_VIEWER = "Window/레러펀스 뷰어";
		
		public const string ASSETMENU_FindReferenceIn = "Assets/프로젝트에서 레퍼런스 찾기";
		public const string ASSETMENU_FindExplicitReferenceInProject = "Assets/프로젝트에서 레퍼런스 재검색 (slow)";

#elif LANGUAGE_JP
		public const string TOOL = "Tools/Reference";
		
		public const string MENU_INDEX_ALL_ASSETS = TOOL + "/Index All Assets";
		public const string MENU_LOG_REFERENCE = TOOL + "/Log Selection";
		
		public const string WINDOW_VIEWER = "Window/Reference Viewer";
		
		public const string ASSETMENU_FindReferenceIn = "Assets/Find References In Project";
		public const string ASSETMENU_FindExplicitReferenceInProject =
 "Assets/Find Explicit References In Project (slow)";

#else
		public const string TOOL = "Tools/Reference";

		public const string MENU_INDEX_ALL_ASSETS = TOOL + "/Index All Assets";
		public const string MENU_LOG_REFERENCE = TOOL + "/Log Selection";

		public const string WINDOW_VIEWER = "Window/Reference Viewer";

		public const string ASSETMENU_FindReferenceIn = "Assets/Find References In Project";
		public const string ASSETMENU_FindExplicitReferenceInProject =
			"Assets/Find Explicit References In Project (slow)";
#endif
	}
}