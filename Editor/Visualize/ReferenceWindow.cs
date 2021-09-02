using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RV
{
	public class ReferenceWindow : EditorWindow
	{


		private Object selected = default;
		private Object previous = default;

		private Object[] dependencies = Array.Empty<Object>();
		private Object[] referenced = Array.Empty<Object>();

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
			}
			
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
			
			previous = selected;
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
}