using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetLens.Reference
{
	public sealed class ReferenceWindow : EditorWindow
	{
		public static bool isDirty = false;

		private Object selected = default;
		private Object previous = default;

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
			if (!ReferenceSetting.IsEnabled)
			{
				EditorGUILayout.HelpBox(Localize.Inst.inspector_notInitializeHelpBox, MessageType.Error);
				EditorGUILayout.Space(10);

				if (GUILayout.Button(Localize.Inst.inspector_generateIndex, new[] { GUILayout.Height(24) }))
				{
					OpenDialog();
				}

				return false;
			}

			return true;

			async void OpenDialog()
			{
				await ReferenceDialog.OpenIndexAllAssetDialog();
				ReferenceSetting.IsEnabled = true;
			}
		}

		private bool ValidateAllowInPlaymode()
		{
			if (Application.isPlaying && ReferenceSetting.PauseInPlaymode)
			{
				EditorGUILayout.HelpBox(Localize.Inst.inspector_playmodeHelpBox, MessageType.Info);

				return false;
			}

			return true;
		}

		private void DrawHeaderIMGUI()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
#if DEBUG_ASSETLENS
				if (GUILayout.Button("Select by guid in clipboard"))
				{
					string buffer = EditorGUIUtility.systemCopyBuffer;

					if (ReferenceUtil.IsGuid(buffer))
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
						ReferenceConsole.Log($"{buffer} is not guid!");
					}
				}
#endif
				GUILayout.FlexibleSpace();

				isLocked = EditorGUILayout.Toggle(Localize.Inst.inspector_lockSelect, isLocked);
			}
		}

		private bool RefreshSelectedTarget()
		{
			if (!isLocked)
			{
				Object current = Selection.activeObject;
				if (!ReferenceSetting.TraceSceneObject && current is GameObject go)
				{
					if (go.IsSceneObject())
					{
						EditorGUILayout.HelpBox(Localize.Inst.inspector_sceneObjectHelpbox, MessageType.Info);
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

			if (selected == null)
			{
				EditorGUILayout.HelpBox(Localize.Inst.inspector_nothing_selected, MessageType.Info, true);
			}
			else
			{
				EditorGUILayout.ObjectField(Localize.Inst.inspector_selected, selected, typeof(Object), true, Array.Empty<GUILayoutOption>());
			}

			// display indexer version
			if (ReferenceSetting.DisplayIndexerVersion)
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
				EditorGUILayout.LabelField(string.Format(Localize.Inst.fmt_inspector_dependencies, dependencies.Length), EditorStyles.boldLabel);
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
						if (ReferenceSetting.UseEditorUtilityWhenSearchDependencies)
						{
							// cannot trace what was that
							if (!drawedHelpBox)
							{
								EditorGUILayout.HelpBox(Localize.Inst.inspector_editorUtilityMissingObjectHelpBox,
									MessageType.Info);

								drawedHelpBox = true;
							}

							EditorGUILayout.LabelField(Localize.Inst.inspector_missingObject);
						}
						else
						{
							string guid = dependencyGuids[i];
							string path = dependencyPaths[i];

							if (string.IsNullOrWhiteSpace(path))
							{
								EditorGUILayout.LabelField(Localize.Inst.guid, guid);
							}
							else
							{
								if (string.CompareOrdinal(path, ReferenceUtil.UNITY_DEFAULT_RESOURCE) == 0
								|| string.CompareOrdinal(path, ReferenceUtil.UNITY_BUILTIN_EXTRA) == 0)
								{
									EditorGUILayout.LabelField(Localize.Inst.inspector_buildInResources, path);
								}
								else
								{
									EditorGUILayout.LabelField(Localize.Inst.inspector_missingObject, path);
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
				EditorGUILayout.LabelField(string.Format(Localize.Inst.fmt_inspector_referencedBy, referenced.Length), EditorStyles.boldLabel);
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