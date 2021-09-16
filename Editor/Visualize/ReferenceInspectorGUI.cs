using System.Collections.Generic;
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
			if (!ReferenceSetting.IsEnabled)
			{
				return;
			}

			if (editor.targets.Length == 1)
			{
				Object target = editor.target;
				if (!ReferenceSetting.TraceSceneObject && target is GameObject go)
				{
					if (go.IsSceneObject())
					{
						return;
					}
				}

				string path = AssetDatabase.GetAssetPath(target);
				string guid = AssetDatabase.AssetPathToGUID(path);

				RefData refData = RefData.Get(guid);

				List<string> usedBy = refData.referedByGuids;

				Rect totalRect = EditorGUILayout.GetControlRect();
				Rect controlRect =
					EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent($"{usedBy.Count} usage"));

				// if (EditorGUI.LinkButton(controlRect, guid))
				// {
				// Debug.Log("Copied");
				// }

				EditorGUI.SelectableLabel(controlRect, guid);
			}
		}
	}
}