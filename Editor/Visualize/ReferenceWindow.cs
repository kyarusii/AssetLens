using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetLens
{
	public sealed class ReferenceWindow : EditorWindow
	{
		public static bool isDirty = false;

		private Object selected = default;
		private Object previous = default;

		// private RefData data = default;

		private Object[] dependencies = Array.Empty<Object>();
		private Object[] referenced = Array.Empty<Object>();

		private string[] dependencyPaths = Array.Empty<string>();
		private string[] dependencyGuids = Array.Empty<string>();

		private Vector2 dependencyScrollPos = default;
		private Vector2 referenceScrollPos = default;

		private bool isLocked = false;

		private void OnGUI()
		{
			if (!ValidateEnabled())
			{
				return;
			}

			if (!ValidateAllowInPlaymode())
			{
				return;
			}

			DrawHeaderIMGUI();

			if (!RefreshSelectedTarget())
			{
				return;
			}

			if (NeedCollectData())
			{
				CollectData();
			}

			DrawIMGUI();

			previous = selected;
		}

		private bool ValidateEnabled()
		{
			if (!AssetLensSetting.IsEnabled)
			{
				EditorGUILayout.HelpBox("Reference is not initialized!", MessageType.Error);
				EditorGUILayout.Space(10);

				if (GUILayout.Button("Initialize", new[] { GUILayout.Height(16) }))
				{
					if (!EditorUtility.DisplayDialog("주의", "이 작업은 시간이 오래 소요될 수 있습니다.\n계속하시겠습니까?", "계속", "취소"))
					{
						return false;
					}

					Task indexAssets = AssetLensCache.IndexAssets();
					AssetLensSetting.IsEnabled = true;
				}

				return false;
			}

			return true;
		}

		private bool ValidateAllowInPlaymode()
		{
			if (Application.isPlaying && AssetLensSetting.PauseInPlaymode)
			{
				EditorGUILayout.HelpBox("Disabled in Playmode.", MessageType.Info);

				return false;
			}

			return true;
		}

		private void DrawHeaderIMGUI()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Select by guid in clipboard"))
				{
					string buffer = EditorGUIUtility.systemCopyBuffer;

					if (AssetLensUtil.IsGuid(buffer))
					{
						string path = AssetDatabase.GUIDToAssetPath(buffer);

						if (string.IsNullOrWhiteSpace(path))
						{
							Debug.Log($"Cannot find an asset from guid:{buffer}");
						}
						else
						{
							Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
							if (obj == null)
							{
								Debug.Log($"Cannot find an asset from guid:{buffer}, path:{path}");
							}
							else
							{
								Selection.objects = new[] { obj };
							}
						}
					}
					else
					{
						Debug.Log($"{buffer} is not guid!");
					}
				}

				GUILayout.FlexibleSpace();

				isLocked = EditorGUILayout.Toggle("Lock", isLocked);
			}
		}

		private bool RefreshSelectedTarget()
		{
			if (!isLocked)
			{
				Object current = Selection.activeObject;
				if (!AssetLensSetting.TraceSceneObject && current is GameObject go)
				{
					if (go.IsSceneObject())
					{
						EditorGUILayout.HelpBox("Disabled on Scene Object.", MessageType.Info);
						return false;
					}
				}

				selected = current;
			}

			return true;
		}

		private bool NeedCollectData()
		{
			return !ReferenceEquals(previous, selected) || isDirty;
		}

		private string objectType;
		private string objectName;
		private string objectPath;
		private string version;

		private void CollectData()
		{
			if (selected)
			{
				Object[] target = new[] { selected };

				string path = AssetDatabase.GetAssetPath(selected);
				string guid = AssetDatabase.AssetPathToGUID(path);

				// directory
				if (Directory.Exists(path))
				{
					return;
				}

				RefData data = RefData.Get(guid);
				
				objectType = data.objectType;
				objectName = data.objectName;
				objectPath = data.objectPath;
				version = data.GetVersionText();

				List<string> referedByGuids = data.referedByGuids;
				referenced = new Object[referedByGuids.Count];

				for (int i = 0; i < referedByGuids.Count; i++)
				{
					string referedByGuid = referedByGuids[i];
					string referedPath = AssetDatabase.GUIDToAssetPath(referedByGuid);
					referenced[i] = AssetDatabase.LoadAssetAtPath<Object>(referedPath);
				}

				if (AssetLensSetting.UseEditorUtilityWhenSearchDependencies)
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
				
				objectType = string.Empty;
				objectName = string.Empty;
				objectPath = string.Empty;
				version = string.Empty;
			}

			isDirty = false;
		}

		private void DrawIMGUI()
		{
			EditorGUILayout.Space(4);

			EditorGUILayout.ObjectField($"Selected", selected, typeof(Object), true, Array.Empty<GUILayoutOption>());

			// display indexer version
			if (AssetLensSetting.DisplayIndexerVersion)
			{
				bool typeAndVersion = !string.IsNullOrWhiteSpace(objectType) && !string.IsNullOrWhiteSpace(version);
				bool nameAndPath = !string.IsNullOrWhiteSpace(objectName) && !string.IsNullOrWhiteSpace(objectPath);

				if (typeAndVersion || nameAndPath)
				{
					EditorGUILayout.BeginVertical("HelpBox");
				}
				
				if (typeAndVersion)
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(version);
						EditorGUILayout.LabelField(objectType);
					}
				}
				
				if (nameAndPath)
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(objectName);
						EditorGUILayout.LabelField(objectPath);
					}
				}

				if (typeAndVersion || nameAndPath)
				{
					EditorGUILayout.EndVertical();
				}
			}

			EditorGUILayout.Space(5);

			if (dependencies.Length > 0)
			{
				EditorGUILayout.LabelField($"Dependencies : {dependencies.Length}", EditorStyles.boldLabel);
				EditorGUILayout.Space(2);

				EditorGUI.indentLevel++;

				if (dependencies.Length > 8)
				{
					EditorGUILayout.BeginVertical();
					dependencyScrollPos = EditorGUILayout.BeginScrollView(dependencyScrollPos, GUILayout.Height(160));
				}

				bool drawedHelpBox = false;
				for (int i = 0; i < dependencies.Length; i++)
				{
					Object dependency = dependencies[i];
					if (dependency == null)
					{
						if (AssetLensSetting.UseEditorUtilityWhenSearchDependencies)
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
							string guid = dependencyGuids[i];
							string path = dependencyPaths[i];

							if (string.IsNullOrWhiteSpace(path))
							{
								EditorGUILayout.LabelField($"guid", guid);
							}
							else
							{
								if (string.CompareOrdinal(path, "Library/unity default resources") == 0
								|| string.CompareOrdinal(path, "Resources/unity_builtin_extra") == 0)
								{
									EditorGUILayout.LabelField($"Built-in Resources", path);
								}
								else
								{
									EditorGUILayout.LabelField("Missing Object", path);
								}
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
					referenceScrollPos = EditorGUILayout.BeginScrollView(referenceScrollPos, GUILayout.Height(160));
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
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}
	}
}