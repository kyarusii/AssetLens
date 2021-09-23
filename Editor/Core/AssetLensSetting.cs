using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetLens
{
	internal sealed class AssetLensSetting : ScriptableObject
	{
		private static AssetLensSetting instance = default;
		
		internal static AssetLensSetting Inst {
			get => GetOrCreateSettings();
		}
		
		private static AssetLensSetting GetOrCreateSettings()
		{
			if (instance != null)
			{
				return instance;
			}

			const string settingPath = FileSystem.SettingDirectory + "/AssetLens.asset";
			instance = EditorGUIUtility.Load(settingPath) as AssetLensSetting;

			if (instance == null)
			{
				instance = CreateInstance<AssetLensSetting>();

				if (!Directory.Exists(FileSystem.SettingDirectory))
				{
					Directory.CreateDirectory(FileSystem.SettingDirectory);
					AssetDatabase.ImportAsset(FileSystem.SettingDirectory);
				}

				AssetDatabase.CreateAsset(instance, settingPath);
				AssetDatabase.SaveAssets();
			}

			return instance;
		}
		
		internal class AssetLensSettingsProviderRegister
		{
			[SettingsProvider]
			public static SettingsProvider CreateFromSettingsObject()
			{
				Object settingsObj = EditorGUIUtility.Load(FileSystem.SettingDirectory + "/AssetLens.asset");
				
				AssetSettingsProvider provider =
					AssetSettingsProvider.CreateProviderFromObject($"Project/Asset Lens", settingsObj);

				provider.keywords =
					SettingsProvider.GetSearchKeywordsFromSerializedObject(
						new SerializedObject(settingsObj));
				return provider;
			}
		}
	}

	[CustomEditor(typeof(AssetLensSetting))]
	internal sealed class AssetLensSettingInspector : Editor
	{
		
		
		private void OnEnable()
		{
			
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			
			// GUILayout.Label("Text");
			
			if (EditorGUI.EndChangeCheck())
			{
				
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}