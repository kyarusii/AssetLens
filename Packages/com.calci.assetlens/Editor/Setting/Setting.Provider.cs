using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace AssetLens.Reference
{
	internal partial class Setting
	{
		public const uint INDEX_VERSION = 206;
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

		internal static class AssetDataSettingsProviderRegister
		{
			[SettingsProvider]
			public static SettingsProvider CreateFromSettingsObject()
			{
				Object settingsObj = GetOrCreateSettings();

				var sp = new SettingsProvider("Project/Asset Lens", SettingsScope.Project)
				{
					activateHandler = (ActivateHandler)
				};

				return sp;
			}

			private static void ActivateHandler(string arg1, VisualElement arg2)
			{
				var editor = Editor.CreateEditor(GetOrCreateSettings());
				arg2.Add(editor.CreateInspectorGUI());
				arg2.style.paddingLeft = 12;
				arg2.style.paddingRight = 12;
			}
		}
	}
}