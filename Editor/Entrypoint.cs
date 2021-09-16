using System.IO;
using UnityEditor;
using UnityEngine;

namespace RV
{
	internal static class Entrypoint
	{
		[InitializeOnLoadMethod]
		private static async void InitializeOnLoadMethod()
		{
			int version = EditorPrefs.GetInt($"{Application.productName}.Reference.Version", 0);
			if (version < 100)
			{
				const string title = "Reference";
				if (EditorUtility.DisplayDialog(title,
					Localize.Inst.dialog_indexedAssetExpired,
					Localize.Inst.dialog_enablePlugin,
					Localize.Inst.dialog_disablePlugin))
				{
					await ReferenceCache.CleanUpAssets();
					await ReferenceCache.IndexAssets();
					ReferenceSetting.IsEnabled = true;
				}
				else
				{
					ReferenceSetting.IsEnabled = false;
				}
			}

			DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.CacheDirectory);
			if (!rootDirectory.Exists)
			{ 
				rootDirectory.Create();

				const string title = "Reference";

				if (EditorUtility.DisplayDialog(title,
					Localize.Inst.dialog_noIndexedData,
					Localize.Inst.dialog_enablePlugin,
					Localize.Inst.dialog_disablePlugin))
				{
					await ReferenceCache.IndexAssets();
					ReferenceSetting.IsEnabled = true;
				}
				else
				{
					ReferenceSetting.IsEnabled = false;
				}
			}
		}
	}
}