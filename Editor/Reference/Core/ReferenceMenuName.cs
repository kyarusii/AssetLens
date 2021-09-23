namespace AssetLens.Reference
{
	/// <summary>
	/// Compile-time Localization
	/// LANGUAGE_CN, LANGUAGE_ES...
	/// </summary>
	internal static class ReferenceMenuName
	{
#if LANGUAGE_KR
		internal const string TOOL = "Tools/에셋 렌즈";

		internal const string MENU_INDEX_ALL_ASSETS = TOOL + "/모든 에셋 인덱싱";
		internal const string MENU_LOG_REFERENCE = TOOL + "/선택한 오브젝트 로깅";
		
		internal const string WINDOW = "Window/Asset Lens";
		internal const string WINDOW_VIEWER = WINDOW + "/Reference Viewer";
		internal const string WINDOW_SAFE_DELETE = WINDOW + "/Safe Deleter";
		internal const string WINDOW_REFERENCE_REPLACE = WINDOW + "/Reference Replacer";

		internal const string ASSETMENU_FindReferenceIn = "Assets/프로젝트에서 레퍼런스 찾기";
		internal const string ASSETMENU_FindExplicitReferenceInProject = "Assets/프로젝트에서 레퍼런스 재검색 (slow)";

#elif LANGUAGE_JP
		internal const string TOOL = "Tools/Asset Lens";
		
		internal const string MENU_INDEX_ALL_ASSETS = TOOL + "/Index All Assets";
		internal const string MENU_LOG_REFERENCE = TOOL + "/Log Selection";
		
		internal const string WINDOW = "Window/Asset Lens";
		internal const string WINDOW_VIEWER = WINDOW + "/Reference Viewer";
		internal const string WINDOW_SAFE_DELETE = WINDOW + "/Safe Deleter";
		internal const string WINDOW_REFERENCE_REPLACE = WINDOW + "/Reference Replacer";

		internal const string ASSETMENU_FindReferenceIn = "Assets/Find References In Project";
		internal const string ASSETMENU_FindExplicitReferenceInProject =
 "Assets/Find Explicit References In Project (slow)";

#else
		internal const string TOOL = "Tools/Asset Lens";

		internal const string MENU_INDEX_ALL_ASSETS = TOOL + "/Index All Assets";
		internal const string MENU_LOG_REFERENCE = TOOL + "/Log Selection";

		internal const string WINDOW = "Window/Asset Lens";
		internal const string WINDOW_VIEWER = WINDOW + "/Reference Viewer";
		internal const string WINDOW_SAFE_DELETE = WINDOW + "/Safe Deleter";
		internal const string WINDOW_REFERENCE_REPLACE = WINDOW + "/Reference Replacer";

		internal const string ASSETMENU_FindReferenceIn = "Assets/Find References In Project";
		internal const string ASSETMENU_FindExplicitReferenceInProject =
			"Assets/Find Explicit References In Project (slow)";
#endif
	}
}