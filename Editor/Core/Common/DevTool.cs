using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

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
			File.WriteAllText(Path.GetFullPath($"Packages/kr.seonghwan.reference/Languages/newLanguage.json"), json);
		}

		[MenuItem(Constants.TOOL + "DEV/Update LocalizeContext")]
		private static void UpdateLocalizeContext()
		{
			Type tLocal = typeof(Localize);
			
			Assert.IsNotNull(tLocal);

			var fields = tLocal.GetFields();
			foreach (FieldInfo field in fields)
			{
				Debug.Log(field.Name);
			}

			var languagesRoot = FileSystem.PackageDirectory + "/Languages";
			var files = Directory.GetFiles(languagesRoot, "*.json");

			foreach (string file in files)
			{
				string json = File.ReadAllText(file);
				Localize obj = JsonUtility.FromJson<Localize>(json);
				
				json = JsonUtility.ToJson(json, true);
				
				File.WriteAllText(file, json);
			}
		}
	}
#endif
}