using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace RV
{
	public sealed class ReferenceWindow : EditorWindow
	{
		private Object selected = default;
		private Object previous = default;

		private Object[] dependencies = Array.Empty<Object>();
		private Object[] referenced = Array.Empty<Object>();

		private string[] dependencyPaths = Array.Empty<string>();
		private string[] dependencyGuids = Array.Empty<string>();

		private Vector2 dependencyScrollPos = default;
		private Vector2 referenceScrollPos = default;

		private bool isLocked = false;
		public static bool isDirty = false;

		// private bool useUIElement = true;

		private void OnGUI()
		{
			// if (useUIElement) return;
			// useUIElement = EditorGUILayout.Toggle("Use UI Elements", useUIElement);
			
			if (!ReferenceSetting.IsEnabled)
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
					ReferenceSetting.IsEnabled = true;
				}

				return;
			}

			if (Application.isPlaying && ReferenceSetting.PauseInPlaymode)
			{
				EditorGUILayout.HelpBox("Disabled in Playmode.", MessageType.Info);
				
				return;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();

				isLocked = EditorGUILayout.Toggle("Lock", isLocked);
			}

			if (!isLocked)
			{
				Object current  = Selection.activeObject;
				if (!ReferenceSetting.TraceSceneObject && current is GameObject go)
				{
					if (go.IsSceneObject())
					{
						EditorGUILayout.HelpBox("Disabled on Scene Object.", MessageType.Info);
						return;
					}					
				}

				selected = current;
			}
			
			if (!ReferenceEquals(previous, selected) || isDirty)
			{
				if (selected)
				{
					Object[] target = new[] { selected };
				
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
					
					if (ReferenceSetting.UseEditorUtilityWhenSearchDependencies)
					{
						dependencies = EditorUtility.CollectDependencies(target);
					}
					else
					{
						int count = data.ownGuids.Count;
						
						dependencies = new Object[count];
						dependencyGuids = new string[count];
						dependencyPaths = new string[count];

						for (int i = 0; i < count; i++)
						{
							dependencyGuids[i] = data.ownGuids[i];
							dependencyPaths[i] = AssetDatabase.GUIDToAssetPath(dependencyGuids[i]);
							dependencies[i] = AssetDatabase.LoadAssetAtPath<Object>(dependencyPaths[i]);
						}
					}
				}
				else
				{
					dependencies = Array.Empty<Object>();
					referenced = Array.Empty<Object>();
				}

				isDirty = false;
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

				bool drawedHelpBox = false;
				for (int i = 0; i < dependencies.Length; i++)
				{
					Object dependency = dependencies[i];
					if (dependency == null)
					{
						if (ReferenceSetting.UseEditorUtilityWhenSearchDependencies)
						{
							// cannot trace what was that
							if (!drawedHelpBox)
							{
								EditorGUILayout.HelpBox(
									"Missing object cannot be tracked in EditorUtility dependency mode.\nTurn off EditorUtilityOnSearch option in ProjectSetting/Reference",
									MessageType.Info);
								
								drawedHelpBox = true;
							}
							
							EditorGUILayout.LabelField("Missing Object");
						}
						else
						{
							var guid = dependencyGuids[i];
							var path = dependencyPaths[i];

							if (string.IsNullOrWhiteSpace(path))
							{
								EditorGUILayout.LabelField($"guid",guid);
							}
							else
							{
								EditorGUILayout.LabelField($"Missing Object",path);
							}
							
						}
					}
					else
					{
						EditorGUILayout.ObjectField(dependency, dependency.GetType(), true,
							Array.Empty<GUILayoutOption>());
					}
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

		private void OnEnable()
		{
			// if (!useUIElement) return;
			//
			// VisualElement root = rootVisualElement;
			//
			// Toggle useUIElementsToggle = new Toggle("Use UI Elements");
			//
			// useUIElementsToggle.RegisterValueChangedCallback((evt =>
			// {
			// 	useUIElement = evt.newValue;
			// }));
			//
			// root.Add(useUIElementsToggle);
			//
			// Toggle lockToggle = new Toggle("Lock");
			// lockToggle.RegisterValueChangedCallback(evt =>
			// {
			// 	isLocked = evt.newValue;
			// });
			//
			// root.Add(lockToggle);
			//
			// ObjectField selectedObject = new ObjectField("Selected");
			// root.Add(selectedObject);


			// var myButton = new Button() { text = "New Button" };
			// myButton.style.width = 160;
			// myButton.style.height = 60;
			//
			// root.Add(myButton);
		}
	}
}