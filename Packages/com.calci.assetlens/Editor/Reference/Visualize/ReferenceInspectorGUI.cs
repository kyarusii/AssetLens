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
					
					EditorGUILayout.BeginHorizontal(new GUIStyle { fixedHeight = 17, margin = new RectOffset { top = 1, bottom = 1 } });
					
					GUIContent prefix = new GUIContent(textContent);
					float prefixWidth = 24f + EditorStyles.boldLabel.CalcSize(prefix).x;
					
					EditorGUILayout.BeginHorizontal(GUILayout.Width(prefixWidth));
					DoPrefixLabel(prefix, EditorStyles.label);
					EditorGUILayout.EndHorizontal();
					
					if (GUILayout.Button("Detail", EditorStyles.miniButtonLeft))
					{
						OnDetail();
					}

					if (GUILayout.Button("Refresh", EditorStyles.miniButtonRight))
					{
						OnRefresh(guid);
					}
					
					GUIContent guidGUI = new GUIContent($"guid: {guid}");
					if (EditorGUILayout.LinkButton(guidGUI))
					{
						OnGUIDClick(guid);
					}
					
					EditorGUILayout.EndHorizontal();
				}
				catch (Exception e)
				{
#if DEBUG_ASSETLENS
					Debug.LogException(e);
#endif
				}
			}
		}

		private static void DoPrefixLabel(GUIContent label, GUIStyle style)
		{
			Rect rect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(false));
			rect.height = Math.Max(18f, rect.height);
			GUI.Label(rect, label, style);
		}

		private static void OnDetail()
		{
			ReferenceMenuItems.FindInProjects();
		}

		private static void OnRefresh(string guid)
		{
			RefData refData = RefData.Get(guid);
			refData.Save();
		}

		private static void OnGUIDClick(string guid)
		{
			EditorGUIUtility.systemCopyBuffer = guid;

			foreach (SceneView sceneView in SceneView.sceneViews)
			{
				sceneView.ShowNotification(new GUIContent($"GUID copied to clipboard!\n{guid}"));
				sceneView.Focus();
			}
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0168
#endif