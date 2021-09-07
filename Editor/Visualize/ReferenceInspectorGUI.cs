using UnityEditor;
using UnityEngine;

namespace RV
{
	[InitializeOnLoad]
	internal static class ReferenceInspectorGUI
	{
		static ReferenceInspectorGUI()
		{
			Editor.finishedDefaultHeaderGUI += EditorOnfinishedDefaultHeaderGUI;
		}

		private static void EditorOnfinishedDefaultHeaderGUI(Editor editor)
		{
			if (editor.targets.Length == 1)
			{
				Object target = editor.target;

				string path = AssetDatabase.GetAssetPath(target);
				string guid = AssetDatabase.AssetPathToGUID(path);
				
				RefData refData = RefData.Get(guid);

				var usedBy = refData.referedByGuids;
				GUILayout.Label($"{usedBy.Count} assets are using this asset.");
			}
		}
	}
}