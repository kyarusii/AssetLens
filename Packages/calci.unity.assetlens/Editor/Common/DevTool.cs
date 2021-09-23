using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RV
{
#if DEBUG_ASSETLENS
	internal class DevTool
	{
		[MenuItem(LanguageConstants.TOOL + "_DEV/Add New Language")]
		private static void CreateLocalizeProfile()
		{
			Localize ctx = new Localize();

			string json = JsonUtility.ToJson(ctx, true);
			string selectedPath = EditorUtility.SaveFilePanel("Create Localization",
				FileSystem.PackageDirectory + "/Languages",
				"NewLanguage.json",
				"json");

			if (!string.IsNullOrWhiteSpace(selectedPath))
			{
				File.WriteAllText(selectedPath, json);
			}
		}

		/// <summary>
		/// Localize 클래스의 필드에 따라 값이 없는 데이터만 밀어 넣어줍니다.
		/// </summary>
		[MenuItem(LanguageConstants.TOOL + "_DEV/Update Language profiles")]
		private static void UpdateLocalizeContext()
		{
			string languagesRoot = FileSystem.PackageDirectory + "/Languages";
			string[] files = Directory.GetFiles(languagesRoot, "*.json");

			foreach (string file in files)
			{
				string json = File.ReadAllText(file);
				Localize obj = JsonUtility.FromJson<Localize>(json);

				json = JsonUtility.ToJson(obj, true);

				File.WriteAllText(file, json);
			}
		}

		[MenuItem(LanguageConstants.TOOL + "_DEV/Find Assets In Packages")]
		private static void FindAssetsInPackages()
		{
			IEnumerable<string> assets = AssetDatabase.FindAssets("", new[] { "Packages" })
				.Select(AssetDatabase.GUIDToAssetPath)
				// 실제 패키지 경로에 있는 경우만
				.Where(path => !Path.GetFullPath(path).Contains("PackageCache"))
				.Where(File.Exists);

			foreach (string asset in assets)
			{
				Debug.Log(asset);
			}
		}
		
		[MenuItem(LanguageConstants.TOOL + "_DEV/Index Include Packages")]
		private static void IndexingIncludePackages()
		{
			var indexAssets = ReferenceCache.IndexAssets();
		}
	}
#endif
}