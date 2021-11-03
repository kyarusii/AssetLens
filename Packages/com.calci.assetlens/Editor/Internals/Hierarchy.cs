using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Internals
{
	internal static class Hierarchy
	{
		private static SearchableEditorWindow hierarchy;
		private static MethodInfo setSearchFilterMethodInfo;

		/// <summary>
		/// initialize hierarchy window
		/// </summary>
		private static void FindSceneHierarchyWindow()
		{
			SearchableEditorWindow[] windows =
				(SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));
			foreach (SearchableEditorWindow window in windows)
			{
				if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
				{
					hierarchy = window;
					break;
				}
			}

			if (hierarchy == null)
				return;

			setSearchFilterMethodInfo =
				typeof(SearchableEditorWindow).GetMethod("SetSearchFilter",
					BindingFlags.NonPublic | BindingFlags.Instance);
		}

		/// <summary>
		/// set search filter in hierarchy
		/// </summary>
		/// <param name="searchFilter"></param>
		/// <param name="searchMode"></param>
		internal static bool SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode)
		{
			if (setSearchFilterMethodInfo == null)
			{
				FindSceneHierarchyWindow();
			}

			if (setSearchFilterMethodInfo == null || hierarchy == null)
			{
				return false;
			}

			setSearchFilterMethodInfo.Invoke(hierarchy, new object[] { searchFilter, searchMode, false, false });
			return true;
		}
	}
}