using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	using UI;
	
	internal static class ReferenceEntrypoint
	{
		[InitializeOnLoadMethod]
		private static void InitializeOnLoadMethod()
		{
			// this allows that EditorWindow is not closed by initializing editor layouts on project startup.
			EditorApplication.delayCall += DelayedCall;
		}
		
		private static void DelayedCall()
		{
#if UNITY_2021_2_OR_NEWER
			HandleTransitionUSS();
#endif
			HandleIndexWizard();
		}

		private static void HandleTransitionUSS()
		{
			var dir = FileSystem.StylesheetDirectory + "2021.2~";
			var fullPath = Path.GetFullPath(dir);
			var files = Directory.GetFiles(fullPath);

			var root = "Assets/Editor Default Resources/Stylesheets";
			if (!Directory.Exists(root))
			{
				Directory.CreateDirectory(root);
			}

			foreach (string file in files)
			{
				var fi = new FileInfo(file);
				fi.CopyTo(root + "/" + fi.Name, true);
			}

			AssetDatabase.ImportAsset(root);
			AssetDatabase.Refresh();
		}

		private static void HandleIndexWizard()
		{
			// Configuration 확인
			string configKey = $"{Application.productName}.AssetLens.Configuration.Session";
			if (!SessionState.GetBool(configKey, false))
			{
				SessionState.SetBool(configKey, true);

				if (!Setting.HasRootDir())
				{
					Setting.CreateRootDir();
				}
				
				// 세팅 루트 디렉터리가 없거나 꺼져있다면 마법사 열기
				if (!Setting.IsEnabled)
				{
					AssetLensIndexWizard.Open();
					return;
				}

				// 에디터 시작시 열지 확인
				string startUpKey = $"{Application.productName}.AssetLens.Configuration.ShowOnStartUp";
				if (EditorPrefs.GetBool(startUpKey, true))
				{
					AssetLensIndexWizard.Open();
				}
			}			
		}
	}
}