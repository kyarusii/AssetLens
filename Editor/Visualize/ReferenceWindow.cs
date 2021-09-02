using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RV
{
	public class ReferenceWindow : EditorWindow
	{


		private Object selected = default;
		private Object previous = default;

		private Object[] dependencies = default;
		private Object[] referenced = default;

		private void OnGUI()
		{
			selected = Selection.activeObject;
			
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
			// EditorGUI.indentLevel++;
			if (dependencies.Length > 0)
			{
				EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel);
				EditorGUILayout.Space(2);
				
				EditorGUI.indentLevel++;
				foreach (Object dependency in dependencies)
				{
					EditorGUILayout.ObjectField(dependency, dependency.GetType(), true, Array.Empty<GUILayoutOption>());
				}

				EditorGUI.indentLevel--;

				EditorGUILayout.Space(10);
			}

			if (referenced.Length > 0)
			{
				EditorGUILayout.LabelField("Referenced By", EditorStyles.boldLabel);
				EditorGUILayout.Space(2);
				
				EditorGUI.indentLevel++;
				foreach (Object dependency in referenced)
				{
					EditorGUILayout.ObjectField(dependency, dependency.GetType(), true, Array.Empty<GUILayoutOption>());
				}

				EditorGUI.indentLevel--;
			}
			// EditorGUI.indentLevel--;
			
			previous = selected;
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
}