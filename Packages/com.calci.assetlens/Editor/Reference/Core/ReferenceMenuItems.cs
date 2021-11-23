using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AssetLens.Reference
{
	using UI;
	
	internal static class ReferenceMenuItems
	{
		private const int FindInProjectsOrder = 28;
		
		[MenuItem("Window/Asset Lens/Index Wizard", false, 110)]
		private static void OpenIndexWizard()
		{
			AssetLensIndexWizard.Open();
		}

		[MenuItem(ReferenceMenuName.MENU_LOG_REFERENCE + " %&r", false, 27)]
		private static void LogReferences()
		{
			Object obj = Selection.activeObject;
			if (obj == null)
			{
				return;
			}

			string path = AssetDatabase.GetAssetPath(obj);
			string guid = AssetDatabase.AssetPathToGUID(path);

			FileInfo fi = new FileInfo(path);

			RefData asset = RefData.Get(guid);
			foreach (string foundGuid in asset.ownGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				
				Debug.Log(string.Format(Localize.Inst.assets_has_dependencies_format, fi.Name, foundPath),
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}

			foreach (string foundGuid in asset.referedByGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);

				Debug.Log(string.Format(Localize.Inst.assets_is_referenced_by_format, fi.Name, foundPath), 
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
		}

		[MenuItem("Tools/Asset Lens/Settings...", false, 110)]
		private static void OpenSetting()
		{
			ReferenceUtil.Focus(Setting.Inst, EObjectFocusMethod.Selection);
		}

		[MenuItem(ReferenceMenuName.WINDOW_VIEWER, false, 110)]
		private static void OpenReferenceViewerWindow()
		{
#if UNITY_2020_3_OR_NEWER
			ReferenceViewer.GetWindow();
			return;
#else
			ReferenceWindow window = (ReferenceWindow)EditorWindow.GetWindow(typeof(ReferenceWindow));

			window.titleContent = new GUIContent("Reference Viewer");
			window.Show();
#endif
		}

		[MenuItem(ReferenceMenuName.ASSETMENU_FindReferenceIn, false, FindInProjectsOrder)]
		public static void FindInProjects()
		{
#if UNITY_2020_3_OR_NEWER
			ReferenceViewer.GetWindow();
			return;
#else
			ReferenceWindow window = (ReferenceWindow)EditorWindow.GetWindow(typeof(ReferenceWindow));

			window.titleContent = new GUIContent("Reference Viewer");
			window.Show();
			window.Focus();
#endif
		}

		[MenuItem(ReferenceMenuName.ASSETMENU_FindExplicitReferenceInProject, true, FindInProjectsOrder + 1)]
		[MenuItem(ReferenceMenuName.ASSETMENU_FindReferenceIn, true, FindInProjectsOrder)]
		private static bool ValidateFindInProject()
		{
			return Selection.activeObject != null && Selection.objects.Length == 1;
		}

		[MenuItem(ReferenceMenuName.ASSETMENU_FindExplicitReferenceInProject, false, FindInProjectsOrder + 1)]
		private static void FindInProjectsExplicit()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			Object obj = Selection.activeObject;
			if (obj == null)
			{
				return;
			}

			string path = AssetDatabase.GetAssetPath(obj);
			string guid = AssetDatabase.AssetPathToGUID(path);

			string[] foundPaths = ReferenceUtil.ExplicitSearchGuid(Application.dataPath, "*.*", guid).ToArray();

			sw.Stop();

			if (EditorUtility.DisplayDialog("Done!", $"{foundPaths.Count()} assets found! \nPrint on console?", "Print",
				"Close"))
			{
				sw.Start();
				foreach (string foundPath in foundPaths)
				{
					Debug.Log(foundPath, AssetDatabase.LoadAssetAtPath<Object>(foundPath));
				}

				sw.Stop();
			}

			Debug.Log($"{foundPaths.Length} files, {sw.ElapsedMilliseconds * 0.001f:N2}s elapsed!");
		}
		
#if !DEBUG_ASSETLENS
		[MenuItem("Help/Asset Lens/Enter Debug Mode", false, 120)]
#endif
		private static void ExitDebugMode()
		{
			BuildTargetGroup currentBuildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTarget);

			if (!symbols.Contains("DEBUG_ASSETLENS"))
			{
				symbols = "DEBUG_ASSETLENS;" + symbols;
			}
			
			PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTarget, symbols);
			
			AssetDatabase.SaveAssets();
			CompilationPipeline.RequestScriptCompilation();
		}

		[MenuItem("Help/Asset Lens/Open README", false, 131)]
		private static void OpenReadme()
		{
			Application.OpenURL("https://github.com/seonghwan-dev/AssetLens/blob/main/README.md");
		}
	}
}