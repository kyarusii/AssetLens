using System.IO;
using UnityEditor;
using UnityEngine;

namespace RV
{
#if DEBUG_REFERENCE
	internal class DevTool
	{
		[MenuItem(Constants.TOOL + "DEV/Create Localize Profile")]
		private static void CreateLocalizeProfile()
		{
			Localize ctx = new Localize();
			
			string json = JsonUtility.ToJson(ctx, true);
			string selectedPath = EditorUtility.SaveFilePanel("Create Localization", FileSystem.PackageDirectory + "/Languages",
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
		[MenuItem(Constants.TOOL + "DEV/Update LocalizeContext")]
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
	}
#endif
}