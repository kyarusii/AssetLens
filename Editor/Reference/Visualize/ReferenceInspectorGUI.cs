using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
		private static SearchableEditorWindow hierarchy;
		private static MethodInfo setSearchFilterMethodInfo;
		
		static ReferenceInspectorGUI()
		{
			FindSceneHierarchyWindow();
			UnityEditor.Editor.finishedDefaultHeaderGUI += EditorOnfinishedDefaultHeaderGUI;
		}

		private static void EditorOnfinishedDefaultHeaderGUI(UnityEditor.Editor editor)
		{
			// don't draw if disabled
			if (!Setting.IsEnabled)
			{
				return;
			}

			// don't draw multi objects selected
			if (editor.targets.Length != 1)
			{
				return;
			}

			Object target = editor.target;

			// scene object
			if (target is GameObject gameObject && gameObject.IsSceneObject())
			{
				if (Setting.DisplaySceneObjectId)
				{
					DrawSceneObject(target);
				}

				return;
			}
			
			// persistent object
			if (target != null)
			{
				DrawPersistentObject(target);
				return;
			}
		}

		/// <summary>
		/// initialize hierarchy window
		/// </summary>
		private static void FindSceneHierarchyWindow()
		{
			SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll (typeof(SearchableEditorWindow));
			foreach (SearchableEditorWindow window in windows) {

				if(window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow") {

					hierarchy = window;
					break;
				}
			}
			
			if (hierarchy == null)
				return;

			setSearchFilterMethodInfo = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);         
		}

		/// <summary>
		/// set search filter in hierarchy
		/// </summary>
		/// <param name="searchFilter"></param>
		/// <param name="searchMode"></param>
		private static void SetHierarchyFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode)
		{
			if (setSearchFilterMethodInfo != null && hierarchy != null)
			{
				setSearchFilterMethodInfo.Invoke(hierarchy, new object[] { searchFilter, searchMode, false, false });
			}
		}

		#region DRAW

		private static void DoPrefixLabel(GUIContent label, GUIStyle style)
		{
			Rect rect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(false));
			rect.height = Math.Max(18f, rect.height);
			GUI.Label(rect, label, style);
		}

		private static void DrawPersistentObject(UnityEngine.Object target)
		{
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

				EditorGUILayout.BeginHorizontal(new GUIStyle
					{ fixedHeight = 17, margin = new RectOffset { top = 1, bottom = 1 } });

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

		private static void DrawSceneObject(UnityEngine.Object target)
		{
			EditorGUILayout.BeginHorizontal();
			Rect totalRect = EditorGUILayout.GetControlRect();
			totalRect.x += 10f;
			Rect controlRect = EditorGUI.PrefixLabel(totalRect, EditorGUIUtility.TrTempContent("InstanceID"));

			EditorGUI.SelectableLabel(controlRect, target.GetInstanceID().ToString());	
			
			if (GUILayout.Button("Find In Scene", GUILayout.Width(120)))
			{
				// OnDetail();
				if (target is GameObject gameObject)
				{
					var components = gameObject.GetComponents(typeof(UnityEngine.Component));
					foreach (UnityEngine.Component component in components)
					{
						int instanceId = component.GetInstanceID();
						SetHierarchyFilter($"ref:{instanceId}:", SearchableEditorWindow.SearchMode.All);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#region CALLBACK

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

		#endregion
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0168
#endif