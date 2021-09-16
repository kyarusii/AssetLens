using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RV
{
	internal sealed class ReferenceSetting : ScriptableObject
	{
		public const uint INDEX_VERSION = 100;
		
		private static ReferenceSetting instance = default;
		private const string k_editorCustomSettingsPath = "Assets/Editor Default Resources/Reference Setting.asset";

		[SerializeField] private bool enabled = false;
		[SerializeField] private bool pauseInPlaymode = true;
		[SerializeField] private bool traceSceneObject = false;
		[SerializeField] private bool useEditorUtilityWhenSearchDependencies = false;

		[SerializeField] private string localization = "English";

		public static bool IsEnabled {
			get
			{
				return GetOrCreateSettings().enabled;
			}
			set
			{
				GetOrCreateSettings().enabled = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}

		public static bool PauseInPlaymode {
			get => GetOrCreateSettings().pauseInPlaymode;
		}
		
		public static bool TraceSceneObject {
			get => GetOrCreateSettings().traceSceneObject;
		}

		public static bool UseEditorUtilityWhenSearchDependencies {
			get => GetOrCreateSettings().useEditorUtilityWhenSearchDependencies;
		}

		public static Localize LoadLocalization {
			get
			{
				string locale = GetOrCreateSettings().localization;
				string fullPath = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages/{locale}.json");

				var json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<Localize>(json);
			}
		}

		public static string Localization {
			set
			{
				GetOrCreateSettings().localization = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}

		private static ReferenceSetting GetOrCreateSettings()
		{
			if (instance != null)
			{
				return instance;
			}
			
			instance = EditorGUIUtility.Load(k_editorCustomSettingsPath) as ReferenceSetting;

			if (instance == null)
			{
				instance = CreateInstance<ReferenceSetting>();
				instance.enabled = false;
				
				// 예전 경로에 존재
				if (!Directory.Exists(k_editorCustomSettingsPath))
				{
					Directory.CreateDirectory(k_editorCustomSettingsPath);
					UnityEditor.AssetDatabase.ImportAsset(k_editorCustomSettingsPath);
				}

				UnityEditor.AssetDatabase.CreateAsset(instance, k_editorCustomSettingsPath);
				UnityEditor.AssetDatabase.SaveAssets();
			}

			return instance;
		}
		
		internal class AssetDataSettingsProviderRegister
		{
			[UnityEditor.SettingsProvider]
			public static UnityEditor.SettingsProvider CreateFromSettingsObject()
			{
				Object settingsObj = EditorGUIUtility.Load(ReferenceSetting.k_editorCustomSettingsPath);
				
				AssetSettingsProvider provider =
					UnityEditor.AssetSettingsProvider.CreateProviderFromObject("Project/Reference", settingsObj);

				provider.keywords =
					UnityEditor.SettingsProvider.GetSearchKeywordsFromSerializedObject(
						new UnityEditor.SerializedObject(settingsObj));
				return provider;
			}
		}
	}
}