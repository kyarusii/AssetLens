using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

#pragma warning disable CS1998

namespace AssetLens
{
	[CustomEditor(typeof(AssetLensSetting))]
	internal sealed class AssetLensSettingInspector : Editor
	{
		private SerializedProperty enabled = default;
		private SerializedProperty pauseInPlaymode = default;
		private SerializedProperty traceSceneObject = default;
		private SerializedProperty useEditorUtilityWhenSearchDependencies = default;
		private SerializedProperty displayIndexerVersion = default;
		private SerializedProperty localization = default;

		private bool unlockDangerZone = false;
		private bool isInProgress = false;

		private void OnEnable()
		{
			enabled = serializedObject.FindProperty(nameof(enabled));
			pauseInPlaymode = serializedObject.FindProperty(nameof(pauseInPlaymode));
			traceSceneObject = serializedObject.FindProperty(nameof(traceSceneObject));
			useEditorUtilityWhenSearchDependencies =
				serializedObject.FindProperty(nameof(useEditorUtilityWhenSearchDependencies));
			displayIndexerVersion = serializedObject.FindProperty(nameof(displayIndexerVersion));
			localization = serializedObject.FindProperty(nameof(localization));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(enabled, new GUIContent(Localize.Inst.setting_enabled));
			EditorGUILayout.Space(12);

			EditorGUI.BeginDisabledGroup(!enabled.boolValue);
			{
				// EditorGUI.indentLevel++;
				{
					EditorGUILayout.LabelField(Localize.Inst.setting_workflow, EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");

					EditorGUILayout.PropertyField(pauseInPlaymode,
						new GUIContent(Localize.Inst.setting_pauseInPlaymode));
					EditorGUILayout.PropertyField(traceSceneObject,
						new GUIContent(Localize.Inst.setting_traceSceneObjects));
					EditorGUILayout.PropertyField(displayIndexerVersion, new GUIContent("Display Indexer Version"));
					EditorGUILayout.PropertyField(useEditorUtilityWhenSearchDependencies,
						new GUIContent(Localize.Inst.setting_useEditorUtilityWhenSearchDependencies));
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(10);
				}
				// EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();
			
			EditorGUILayout.LabelField(Localize.Inst.setting_miscellaneous, EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("HelpBox");

			string root = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages");
			string currentLocale = localization.stringValue;

			List<string> localNames = new List<string>();
			string[] languageFiles = Directory.GetFiles(root, "*.json");
			foreach (string file in languageFiles)
			{
				FileInfo fi = new FileInfo(file);
				localNames.Add(fi.Name.Replace(".json", ""));
			}

			int selected = localNames.IndexOf(currentLocale);
			int changed = EditorGUILayout.Popup(Localize.Inst.setting_language, selected, localNames.ToArray());

			if (selected != changed)
			{
				AssetLensSetting.Localization = localNames[changed];
				Localize.Inst = AssetLensSetting.LoadLocalization;
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space(20);

			using (new EditorGUILayout.HorizontalScope(GUILayout.Height(26)))
			{
				GUILayout.FlexibleSpace();

				if (!unlockDangerZone)
				{
					if (GUILayout.Button(Localize.Inst.setting_unlockDangerzone, new GUILayoutOption[]
					{
						GUILayout.Width(200), GUILayout.Height(24)
					}))
					{
						unlockDangerZone = true;
					}
				}
				else
				{
					GUILayout.Label(Localize.Inst.setting_dangerzone, GUILayout.Height(24));
				}

				GUILayout.FlexibleSpace();
			}

			using (new EditorGUILayout.VerticalScope("HelpBox"))
			{
				EditorGUILayout.Space(4);

				using (new EditorGUI.DisabledGroupScope(!unlockDangerZone || isInProgress))
				{
					if (GUILayout.Button(Localize.Inst.setting_cleanUpCache,
						new[] { GUILayout.Height(30) }))
					{
						AssetLensConsole.Log("Delete...");
						isInProgress = true;

						CleanUpCache();
					}
					
					if (GUILayout.Button(Localize.Inst.setting_uninstall,
						new[] { GUILayout.Height(30) }))
					{
						AssetLensConsole.Log("Uninstall...");
						isInProgress = true;

						CleanUninstall();
					}
				}

				EditorGUILayout.Space(4);
			}

			if (EditorGUI.EndChangeCheck())
			{
				ReferenceWindow.isDirty = true;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private async void CleanUpCache()
		{
			int processedAssetCount = await AssetLensCache.CleanUpAssets();
			AssetLensConsole.Log($"{processedAssetCount} asset caches removed!");

			isInProgress = false;
		}

		private async void CleanUninstall()
		{
			AssetLensSetting.IsEnabled = false;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssets();
			AssetLensConsole.Log($"{processedAssetCount} asset caches removed!");
			
			Directory.Delete(FileSystem.ReferenceCacheDirectory);

#if DEBUG_ASSETLENS
			
			string projectManifest = File.ReadAllText(FileSystem.Manifest);
			if (!projectManifest.Contains(Constants.PackageName))
			{
				AssetLensConsole.Log("Cannot be uninstalled under development.");
				return;
			}
#endif

			RemoveRequest request = Client.Remove(Constants.PackageName);
			EditorApplication.update += RemoveProgress;

			void RemoveProgress()
			{
				if (request.IsCompleted)
				{
					if (request.Status == StatusCode.Success)
					{
						Debug.Log("Removed: " + request.PackageIdOrName);
					}
					else if (request.Status >= StatusCode.Failure) {
						Debug.Log(request.Error.message);
					}

					isInProgress = false;
					EditorApplication.update -= RemoveProgress;
				}
			}
		}
	}
}

#pragma warning restore CS1998