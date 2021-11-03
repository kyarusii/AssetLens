using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if !DEBUG_ASSETLENS
#pragma warning disable CS0414
#endif

namespace AssetLens.Reference
{
	internal partial class Setting : ScriptableObject
	{
		[SerializeField] private bool enabled = false;
		
		[SerializeField] private bool pauseInPlaymode = true;
		[SerializeField] private bool useEditorUtilityWhenSearchDependencies = false;
		[SerializeField] private bool displayIndexerVersion = false;
		[SerializeField] private string localization = "English";
		
		[SerializeField] private bool useUIElementsWindow = false;
		[SerializeField] private bool autoUpgradeCachedData = false;

		[SerializeField] private bool traceSceneObject = false;
		[SerializeField] private bool displaySceneObjectInstanceId = false;

		public static event Action<Setting> onSettingChange = delegate(Setting setting) {  };

		public static void SetSettingDirty()
		{
			onSettingChange(Setting.Inst);
		}

		public static bool DisplaySceneObjectId => GetOrCreateSettings().displaySceneObjectInstanceId;
		
		public static bool PauseInPlaymode => GetOrCreateSettings().pauseInPlaymode;
		
		public static bool TraceSceneObject => GetOrCreateSettings().traceSceneObject;
		
		public static bool UseEditorUtilityWhenSearchDependencies =>
			GetOrCreateSettings().useEditorUtilityWhenSearchDependencies;

		public static bool DisplayIndexerVersion => GetOrCreateSettings().displayIndexerVersion;
		
		public static string Localization {
			set
			{
				GetOrCreateSettings().localization = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}
		
		internal static Localize LoadLocalization {
			get
			{
				string locale = GetOrCreateSettings().localization;
				string fullPath = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages/{locale}.json");

				string json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<Localize>(json);
			}
		}

		public static bool UseUIElements {
			get => GetOrCreateSettings().useUIElementsWindow;
		}

		public static bool AutoUpgradeCachedData {
			get
			{
#if DEBUG_ASSETLENS
				return GetOrCreateSettings().autoUpgradeCachedData;
#else
				return false;
#endif
			}
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0414
#endif