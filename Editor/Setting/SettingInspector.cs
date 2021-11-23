using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

#pragma warning disable CS1998

namespace AssetLens.Reference
{
	[CustomEditor(typeof(Setting))]
	internal sealed class SettingInspector : UnityEditor.Editor
	{
		
		private static bool isCompiling;
		
		[InitializeOnLoadMethod]
		private static void RegisterCallback()
		{
			CompilationPipeline.compilationStarted += CompilationPipelineOncompilationStarted;
			CompilationPipeline.compilationFinished += CompilationPipelineOncompilationFinished;
		}

		private static void CompilationPipelineOncompilationStarted(object obj)
		{
			isCompiling = true;
		}
		
		private static void CompilationPipelineOncompilationFinished(object obj)
		{
			isCompiling = false;
		}

		private SerializedProperty enabled = default;
		private SerializedProperty pauseInPlaymode = default;
		private SerializedProperty useEditorUtilityWhenSearchDependencies = default;
		private SerializedProperty displayIndexerVersion = default;
		private SerializedProperty localization = default;
		private SerializedProperty useUIElementsWindow = default;
		private SerializedProperty autoUpgradeCachedData = default;

		/* scene object */
		private SerializedProperty traceSceneObject = default;
		private SerializedProperty displaySceneObjectInstanceId = default;
		
		private bool unlockDangerZone = false;
		private bool isInProgress = false;

		private int managedAssetCount = -1;
		private bool isCalculating = false;

		private void OnEnable()
		{
			enabled = serializedObject.FindProperty(nameof(enabled));
			pauseInPlaymode = serializedObject.FindProperty(nameof(pauseInPlaymode));
			
			useEditorUtilityWhenSearchDependencies =
				serializedObject.FindProperty(nameof(useEditorUtilityWhenSearchDependencies));
			displayIndexerVersion = serializedObject.FindProperty(nameof(displayIndexerVersion));
			localization = serializedObject.FindProperty(nameof(localization));
			useUIElementsWindow = serializedObject.FindProperty(nameof(useUIElementsWindow));
			autoUpgradeCachedData = serializedObject.FindProperty(nameof(autoUpgradeCachedData));
			
			traceSceneObject = serializedObject.FindProperty(nameof(traceSceneObject));
			displaySceneObjectInstanceId = serializedObject.FindProperty(nameof(displaySceneObjectInstanceId));
			
			CountCacheAsync();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			if (isCompiling)
			{
				EditorGUILayout.LabelField("Status", "Compiling");
			}

			EditorGUI.BeginDisabledGroup(isCompiling);
			
			EditorGUILayout.BeginVertical(GUILayout.Height(200));

			if (enabled.boolValue)
			{
				EditorGUILayout.PropertyField(enabled, new GUIContent(Localize.Inst.setting_enabled));
				EditorGUILayout.Space(12);

				EditorGUI.BeginDisabledGroup(!enabled.boolValue);
				{
					EditorGUILayout.LabelField(Localize.Inst.setting_workflow, EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");

					{
						EditorGUILayout.PropertyField(pauseInPlaymode,
							new GUIContent(Localize.Inst.setting_pauseInPlaymode));
						
						EditorGUILayout.PropertyField(displayIndexerVersion, new GUIContent("Display Indexer Version"));
						EditorGUILayout.PropertyField(useEditorUtilityWhenSearchDependencies,
							new GUIContent(Localize.Inst.setting_useEditorUtilityWhenSearchDependencies));
						EditorGUILayout.PropertyField(useUIElementsWindow,
							new GUIContent("Use UI Elements"));
						EditorGUILayout.PropertyField(autoUpgradeCachedData,
							new GUIContent("Auto Update Cached Data On Read"));

						if (managedAssetCount < 0)
						{
							EditorGUILayout.LabelField($"Managed Asset Count", "calculating...");
						}
						else
						{
							EditorGUILayout.LabelField("Managed Asset Count", managedAssetCount.ToString());
						}
					}

					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(10);
					
#if DEBUG_ASSETLENS
					EditorGUILayout.LabelField("Scene Objects", EditorStyles.boldLabel);
					EditorGUILayout.BeginVertical("HelpBox");
					EditorGUILayout.PropertyField(traceSceneObject,
						new GUIContent(Localize.Inst.setting_traceSceneObjects));
					EditorGUILayout.PropertyField(displaySceneObjectInstanceId,
						new GUIContent("Display Instance ID"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space(10);
#endif
				}
				EditorGUI.EndDisabledGroup();
				
			}
			else
			{
				EditorGUILayout.HelpBox(Localize.Inst.setting_initInfo, MessageType.Info);
				
				if (GUILayout.Button("Open Wizard", GUILayout.Height(40)))
				{
					ConfigurationWizard.Open();
					// OpenDialog();
				}
			}
			
			// async void OpenDialog()
			// {
			// 	await AssetLensCache.IndexAssetsAsync();
			// 	Setting.IsEnabled = true;
			// 	
			// 	// await ReferenceDialog.OpenIndexAllAssetDialog();
			// 	// Setting.IsEnabled = true;
			// 	
			// 	// DialogWindow.OpenDialog(Localize.Inst.dialog_titleContent,
			// 	// 	Localize.Inst.dialog_noIndexedData, 
			// 	// 	Localize.Inst.dialog_enablePlugin, 
			// 	// 	Localize.Inst.dialog_disablePlugin,
			// 	// 	OnAccept, OnCancel, OnClose);
			// 	//
			// 	// async void OnAccept()
			// 	// {
			// 	// 	await AssetLensCache.IndexAssetsAsync();
			// 	// 	Setting.IsEnabled = true;
			// 	// }
			// 	//
			// 	// void OnCancel()
			// 	// {
			// 	// 	Setting.IsEnabled = false;
			// 	// }
			// 	//
			// 	// void OnClose()
			// 	// {
			// 	// 	Setting.IsEnabled = false;
			// 	// }
			// }

			EditorGUILayout.EndVertical();
			
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
				Setting.Localization = localNames[changed];
				Localize.Inst = Setting.LoadLocalization;
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
			
			// isCompiling scope
			EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck())
			{
				ReferenceWindow.isDirty = true;
				Setting.SetSettingDirty();
			}

			serializedObject.ApplyModifiedProperties();
		}

		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}

		private async void CleanUpCache()
		{
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Log($"{processedAssetCount} asset caches removed!");

			isInProgress = false;
		}

		private async void CleanUninstall()
		{
			Setting.IsEnabled = false;
			
			int processedAssetCount = await AssetLensCache.CleanUpAssetsAsync();
			AssetLensConsole.Log($"{processedAssetCount} asset caches removed!");
			
			Directory.Delete(FileSystem.ReferenceCacheDirectory);

#if DEBUG_ASSETLENS
			
			string projectManifest = await File.ReadAllTextAsync(FileSystem.Manifest);
			if (!projectManifest.Contains(Constants.PackageName))
			{
				AssetLensConsole.Log("Cannot be uninstalled under development.");
				return;
			}
#endif

			// @TODO :: NEED TEST
			string resultMessage = await PackageSystem.Uninstall();
			Debug.Log(resultMessage);
		}

		private async void CountCacheAsync()
		{
			if (isCalculating)
			{
				return;
			}
			
			isCalculating = true;
			managedAssetCount = await GetCount();
			isCalculating = false;

			async Task<int> GetCount()
			{
				DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.ReferenceCacheDirectory);
				var files = rootDirectory.GetFiles("*.ref");

				await Task.Delay(10);
				
				return files.Length;
			}
		}
	}
}

#pragma warning restore CS1998