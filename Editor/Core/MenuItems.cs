using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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

			RefData asset = RefData.Get(guid);
			foreach (string foundGuid in asset.ownGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				Debug.Log($"<color=#7FFF00>레퍼런스하고 있는 에셋 : {foundPath}</color>",
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
			
			foreach (string foundGuid in asset.referedByGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				
				Debug.Log($"<color=#FF0F8F>이 에셋을 레퍼런스하고 있는 에셋 : {foundPath}</color>", 
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

		private const string FinInProjectsMenuName = "Assets/Find References In Project";
		private const int order = 28;
		
		[MenuItem(FinInProjectsMenuName, false, order)]
		private static void FindInProjects()
		{
			ReferenceWindow window = (ReferenceWindow)EditorWindow.GetWindow(typeof(ReferenceWindow));
			
			window.titleContent = new GUIContent("Reference Viewer");
			window.Show();
		}

		[MenuItem(FinInProjectsMenuName, true, order)]
		private static bool ValidateFindInProject()
		{
			return Selection.activeObject != null && Selection.objects.Length == 1;
		}
	}
}