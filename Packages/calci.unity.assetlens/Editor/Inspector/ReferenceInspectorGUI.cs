using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if !DEBUG_ASSETLENS
#pragma warning disable CS0168
#endif

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

				if (!ReferenceEquals(Selection.activeObject, target)) return;
				
				string path = AssetDatabase.GetAssetPath(target);
				
				// Exclude folder
				if (Directory.Exists(path)) return;
				string guid = AssetDatabase.AssetPathToGUID(path);

				try
				{
					RefData refData = RefData.Get(guid);
				
					List<string> usedBy = refData.referedByGuids;
				
					Rect totalRect = EditorGUILayout.GetControlRect();
					Rect controlRect =
						EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent($"{usedBy.Count} usage"));
				
					EditorGUI.SelectableLabel(controlRect, guid);	
				}
				catch (Exception e)
				{
#if DEBUG_ASSETLENS
					Debug.LogError(e.Message, target);
#endif
				}
			}
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0168
#endif