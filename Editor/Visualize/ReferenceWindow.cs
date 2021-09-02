using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RV
{
	public sealed class ReferenceWindow : EditorWindow
	{
		private Object selected = default;
		private Object previous = default;

		private Object[] dependencies = Array.Empty<Object>();
		private Object[] referenced = Array.Empty<Object>();

		private Vector2 dependencyScrollPos = default;
		private Vector2 referenceScrollPos = default;

		private bool isLocked = false;

		private void OnGUI()
		{
			if (!Config.IsEnabled)
			{
				EditorGUILayout.HelpBox("Reference is not initialized!", MessageType.Error);
				EditorGUILayout.Space(10);

				if (GUILayout.Button("Initialize", new[] { GUILayout.Height(16) }))
				{
					if (!EditorUtility.DisplayDialog("주의", "이 작업은 시간이 오래 소요될 수 있습니다.\n계속하시겠습니까?", "계속", "취소"))
					{
						return;
					}

					Task indexAssets = ReferenceCache.IndexAssets();
					Config.IsEnabled = true;
				}

				return;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				isLocked = EditorGUILayout.Toggle("Lock", isLocked);
			}

			if (!isLocked)
			{
				selected = Selection.activeObject;
			}
			
			if (!ReferenceEquals(previous, selected))
			{
				if (selected)
				{
					Object[] target = new[] { selected };
				
					dependencies = EditorUtility.CollectDependencies(target);

					string path = AssetDatabase.GetAssetPath(selected);
					string guid = AssetDatabase.AssetPathToGUID(path);
					
					RefData data = RefData.Get(guid);
					
					var referedByGuids = data.referedByGuids;
					referenced = new Object[referedByGuids.Count];

					for (int i = 0; i < referedByGuids.Count; i++)
					{
						string referedByGuid = referedByGuids[i];
						string referedPath = AssetDatabase.GUIDToAssetPath(referedByGuid);
						referenced[i] = AssetDatabase.LoadAssetAtPath<Object>(referedPath);
					}
				}
				else
				{
					dependencies = Array.Empty<Object>();
					referenced = Array.Empty<Object>();
				}
			}

			EditorGUILayout.Space(4);
			
			EditorGUILayout.ObjectField($"Selected", selected, typeof(Object), true, Array.Empty<GUILayoutOption>());
			EditorGUILayout.Space(5);
			
			if (dependencies.Length > 0)
			{
				EditorGUILayout.LabelField($"Dependencies : {dependencies.Length}", EditorStyles.boldLabel);
				EditorGUILayout.Space(2);
				
				EditorGUI.indentLevel++;
				
				if (dependencies.Length > 8)
				{
					EditorGUILayout.BeginVertical();
					dependencyScrollPos = EditorGUILayout.BeginScrollView(dependencyScrollPos,  GUILayout.Height(160));
				}
				
				foreach (Object dependency in dependencies)
				{
					EditorGUILayout.ObjectField(dependency, dependency.GetType(), true, Array.Empty<GUILayoutOption>());
				}
				
				if (dependencies.Length > 8)
				{
					EditorGUILayout.EndScrollView();
					EditorGUILayout.EndVertical();
				}

				EditorGUI.indentLevel--;

				EditorGUILayout.Space(10);
			}

			if (referenced.Length > 0)
			{
				EditorGUILayout.LabelField($"Referenced By : {referenced.Length}", EditorStyles.boldLabel);
				EditorGUILayout.Space(2);
				
				EditorGUI.indentLevel++;
				
				if (referenced.Length > 8)
				{
					EditorGUILayout.BeginVertical();
					referenceScrollPos = EditorGUILayout.BeginScrollView(referenceScrollPos,  GUILayout.Height(160));
				}
				
				foreach (Object dependency in referenced)
				{
					EditorGUILayout.ObjectField(dependency, dependency.GetType(), true, Array.Empty<GUILayoutOption>());
				}
				
				if (referenced.Length > 8)
				{
					EditorGUILayout.EndScrollView();
					EditorGUILayout.EndVertical();
				}


				EditorGUI.indentLevel--;
			}
			
			previous = selected;
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
}