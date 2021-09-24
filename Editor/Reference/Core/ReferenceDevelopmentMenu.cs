using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
#if DEBUG_ASSETLENS
	internal class ReferenceDevelopmentMenu
	{
		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Add New Language")]
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
		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Update Language profiles")]
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

		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Find Assets In Packages")]
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
		
		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Index Include Packages")]
		private static void IndexingIncludePackages()
		{
			var indexAssets = AssetLensCache.IndexAssets();
		}

		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Get Version")]
		private static async void GetVersion()
		{
			string result = await PackageSystem.GetVersion();
			Debug.Log(result);
		}

		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Try Parse Selection")]
		private static async void TryParseGuids()
		{
			Object target = Selection.activeObject;

			string path = AssetDatabase.GetAssetPath(target);
			StreamReader reader = new StreamReader(File.OpenRead(path));
			string value = await reader.ReadToEndAsync();

			List<string> parsed = RefData.ParseOwnGuids(value);
			foreach (string guid in parsed)
			{
				Debug.Log($"{AssetDatabase.GUIDToAssetPath(guid)}, {guid}");
			}
		}
		
		[MenuItem(ReferenceMenuName.TOOL + "_DEV/Try Parse Selection 2")]
		private static async void TryParseGuids2()
		{
			HashSet<string> owningGuids = new HashSet<string>();
			
			Object target = Selection.activeObject;

			string path = AssetDatabase.GetAssetPath(target);
			StreamReader reader = new StreamReader(File.OpenRead(path));
			string value = await reader.ReadToEndAsync();
			
			Regex guidRegx = new Regex("GUID:\\s?([a-fA-F0-9]+)");
			MatchCollection matches = guidRegx.Matches(value);
			foreach (Match match in matches)
			{
				if (match.Success)
				{
					owningGuids.Add(match.Groups[1].Value);
				}
			}

			foreach (string guid in owningGuids)
			{
				Debug.Log($"{AssetDatabase.GUIDToAssetPath(guid)}, {guid}");
			}
		}
	}
#endif
}