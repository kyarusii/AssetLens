using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if !DEBUG_ASSETLENS
#pragma warning disable CS0168
#endif

namespace AssetLens.Reference
{
	[InitializeOnLoad]
	internal static class ReferenceInspectorGUI
	{
		static ReferenceInspectorGUI()
		{
			UnityEditor.Editor.finishedDefaultHeaderGUI += EditorOnfinishedDefaultHeaderGUI;
		}

		private static void EditorOnfinishedDefaultHeaderGUI(UnityEditor.Editor editor)
		{
			if (!Setting.IsEnabled)
			{
				return;
			}

			if (editor.targets.Length == 1)
			{
				Object target = editor.target;
				if (!Setting.TraceSceneObject && target is GameObject go)
				{
					if (go.IsSceneObject())
					{
						return;
					}
				}

				if (!ReferenceEquals(Selection.activeObject, target)) return;
				
				string path = AssetDatabase.GetAssetPath(target);

				// Exclude empty path
				if (string.IsNullOrWhiteSpace(path)) return;
				// Exclude folder
				if (Directory.Exists(path)) return;
				// Exclude Invalid Path
				if (!File.Exists(path)) return;
				
				string guid = AssetDatabase.AssetPathToGUID(path);


				try
				{
					RefData refData = RefData.Get(guid);
				
					List<string> usedBy = refData.referedByGuids;
					
					// usage or usages in default
					string textContent = usedBy.Count < 2
						? string.Format(Localize.Inst.fmt_inspector_usageCount_singular, usedBy.Count)
						: string.Format(Localize.Inst.fmt_inspector_usageCount_multiple, usedBy.Count);
					
					Rect totalRect = EditorGUILayout.GetControlRect();
					// @TODO :: convert to button like prefab
					Rect controlRect = EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent(textContent));

					EditorGUI.SelectableLabel(controlRect, guid);	
				}
				catch (Exception e)
				{
#if DEBUG_ASSETLENS
					Debug.LogException(e);
#endif
				}
			}
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0168
#endif