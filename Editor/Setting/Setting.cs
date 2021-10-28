using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	internal partial class Setting : ScriptableObject
	{
		[SerializeField] private bool enabled = false;
		
		[SerializeField] private bool pauseInPlaymode = true;
		[SerializeField] private bool traceSceneObject = false;
		[SerializeField] private bool useEditorUtilityWhenSearchDependencies = false;
		[SerializeField] private bool displayIndexerVersion = false;
		[SerializeField] private string localization = "English";
		
		[SerializeField] private bool useUIElementsWindow = false;
		
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
	}
}