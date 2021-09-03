using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RV
{
	internal static class MenuItems
	{
		[MenuItem("Tools/Reference/Index All Assets")]
		private static void IndexAllAssets()
		{
			if (!EditorUtility.DisplayDialog("주의", "이 작업은 시간이 오래 소요될 수 있습니다.\n계속하시겠습니까?", "계속", "취소"))
			{
				return;
			}

			Task indexAssets = ReferenceCache.IndexAssets();
			Config.IsEnabled = true;
		}

		[MenuItem("Tools/Reference/Log Selection %&r")]
		private static void LogReferences()
		{
			Object obj = Selection.activeObject;
			if (obj == null) return;
			
			string path = AssetDatabase.GetAssetPath(obj);
			string guid = AssetDatabase.AssetPathToGUID(path);

			FileInfo fi = new FileInfo(path);
			
			RefData asset = RefData.Get(guid);
			foreach (string foundGuid in asset.ownGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				Debug.Log($"<color=#7FFF00>{fi.Name}이 사용하고 있는 에셋 : {foundPath}</color>",
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
			
			foreach (string foundGuid in asset.referedByGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				
				Debug.Log($"<color=#1E90FF>{fi.Name}을 사용하고 있는 에셋 : {foundPath}</color>", 
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
		}
		
		[MenuItem("Window/Reference Viewer")]
		private static void Init()
		{
			ReferenceWindow window = (ReferenceWindow)EditorWindow.GetWindow(typeof(ReferenceWindow));
			
			window.titleContent = new GUIContent("Reference Viewer");
			window.Show();
		}

		private const string FindReferenceInMenuName = "Assets/Find References In Project";
		private const string FindExplicitReferenceInProjectMenuName = "Assets/Find Explicit References In Project (slow)";
		
		private const int order = 28;
		
		[MenuItem(FindReferenceInMenuName, false, order)]
		private static void FindInProjects()
		{
			ReferenceWindow window = (ReferenceWindow)EditorWindow.GetWindow(typeof(ReferenceWindow));
			
			window.titleContent = new GUIContent("Reference Viewer");
			window.Show();
		}

		[MenuItem(FindExplicitReferenceInProjectMenuName, true, order+1)]
		[MenuItem(FindReferenceInMenuName, true, order)]
		private static bool ValidateFindInProject()
		{
			return Selection.activeObject != null && Selection.objects.Length == 1;
		}
		
		[MenuItem(FindExplicitReferenceInProjectMenuName, false, order+1)]
		private static void FindInProjectsExplicit()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			
			Object obj = Selection.activeObject;
			if (obj == null) return;
			
			string path = AssetDatabase.GetAssetPath(obj);
			string guid = AssetDatabase.AssetPathToGUID(path);

			string[] foundPaths = ReferenceUtil.ExplicitSearchGuid(Application.dataPath, "*.*", guid).ToArray();

			sw.Stop();
			
			if (EditorUtility.DisplayDialog("Done!", $"{foundPaths.Count()} assets found! \nPrint on console?", "Print", "Close"))
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
	}
}