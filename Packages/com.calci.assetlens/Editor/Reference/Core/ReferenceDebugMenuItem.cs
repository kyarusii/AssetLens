using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	internal static class ReferenceDebugMenuItem
	{
				
#if DEBUG_ASSETLENS
		[MenuItem(ReferenceMenuName.WINDOW_SAFE_DELETE, false, 122)]
#endif
		private static void OpenSafeDeleteWindow()
		{
			ReferenceSafeDeleteWindow window = (ReferenceSafeDeleteWindow)EditorWindow.GetWindow(typeof(ReferenceSafeDeleteWindow));

			window.titleContent = new GUIContent("Safe Deleter");
			window.Show();
		}
		
#if DEBUG_ASSETLENS
		[MenuItem(ReferenceMenuName.MENU_INDEX_ALL_ASSETS)]
#endif
		private static void IndexAllAssets()
		{
			if (!EditorUtility.DisplayDialog(
				Localize.Inst.indexing_title,
				Localize.Inst.indexing_message,
				Localize.Inst.indexing_proceed,
				Localize.Inst.indexing_cancel))
			{
				return;
			}

			Task indexAssets = AssetLensCache.IndexAssetsAsync();
			Setting.IsEnabled = true;
		}
		
#if DEBUG_ASSETLENS
		[MenuItem(ReferenceMenuName.MENU_INDEX_ALL_ASSETS + "_100")]
#endif
		private static void IndexAllAssets_100()
		{
			if (!EditorUtility.DisplayDialog(
				Localize.Inst.indexing_title,
				Localize.Inst.indexing_message,
				Localize.Inst.indexing_proceed,
				Localize.Inst.indexing_cancel))
			{
				return;
			}

			Task indexAssets = AssetLensCache.IndexAssetsAsync(100);
			Setting.IsEnabled = true;
		}
		
#if DEBUG_ASSETLENS
		[MenuItem(ReferenceMenuName.WINDOW_REFERENCE_REPLACE, false, 121)]
#endif
		private static void OpenReferenceReplaceWindow()
		{
			ReferenceReplacementWindow window = (ReferenceReplacementWindow)EditorWindow.GetWindow(typeof(ReferenceReplacementWindow));

			window.titleContent = new GUIContent("Reference Replacer");
			window.Show();
		}
	}
}