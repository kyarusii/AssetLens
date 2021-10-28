using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace AssetLens.Reference
{
	internal partial class Setting
	{
		public const uint INDEX_VERSION = 100;
		private const string k_editorCustomSettingsPath = FileSystem.SettingDirectory + "/AssetLens Setting.asset";
		
		private static Setting instance = default;
		
		internal static Setting Inst {
			get => GetOrCreateSettings();
		}

		public static bool IsEnabled {
			get => GetOrCreateSettings().enabled;
			set
			{
				GetOrCreateSettings().enabled = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}
		
		

		private static Setting GetOrCreateSettings()
		{
			if (instance != null)
			{
				return instance;
			}

			instance = EditorGUIUtility.Load(FileSystem.SettingDirectory + "/ReferenceSetting.asset") as Setting;
			if (instance != null)
			{
				AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(instance), k_editorCustomSettingsPath);
				AssetDatabase.SaveAssets();
				
				return instance;
			}

			instance = EditorGUIUtility.Load(k_editorCustomSettingsPath) as Setting;

			if (instance == null)
			{
				instance = CreateInstance<Setting>();
				instance.enabled = false;

				if (!Directory.Exists(FileSystem.SettingDirectory))
				{
					Directory.CreateDirectory(FileSystem.SettingDirectory);
					AssetDatabase.ImportAsset(FileSystem.SettingDirectory);
				}

				AssetDatabase.CreateAsset(instance, k_editorCustomSettingsPath);
				AssetDatabase.SaveAssets();
			}

			return instance;
		}

		internal class AssetDataSettingsProviderRegister
		{
			[SettingsProvider]
			public static SettingsProvider CreateFromSettingsObject()
			{
				Object settingsObj = GetOrCreateSettings();
				
				AssetSettingsProvider provider =
					AssetSettingsProvider.CreateProviderFromObject($"Project/Asset Lens", settingsObj);

				provider.keywords =
					SettingsProvider.GetSearchKeywordsFromSerializedObject(
						new SerializedObject(settingsObj));
				return provider;
			}
		}
	}
}