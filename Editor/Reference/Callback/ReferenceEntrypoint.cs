using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	internal static class ReferenceEntrypoint
	{
		[InitializeOnLoadMethod]
		private static async void InitializeOnLoadMethod()
		{
			string key = $"{Application.productName}.AssetLens.InitCheck";
			if (!SessionState.GetBool(key, false))
			{
				// when there is no directory for cached object.
				DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.ReferenceCacheDirectory);
				if (!rootDirectory.Exists)
				{ 
					rootDirectory.Create();

					// force re-indexing
					if (ReferenceSerializer.HasLocalVersion())
					{
						int version = ReferenceSerializer.GetLocalVersion();
						if (version < 100)
						{
							if (EditorUtility.DisplayDialog(
								Localize.Inst.dialog_titleContent,
								Localize.Inst.dialog_indexedAssetExpired,
								Localize.Inst.dialog_enablePlugin,
								Localize.Inst.dialog_disablePlugin))
							{
								await AssetLensCache.CleanUpAssets();
								await AssetLensCache.IndexAssets();
						
								Setting.IsEnabled = true;
							}
							else
							{
								Setting.IsEnabled = false;
							}
						}
					
						return;
					}

					// first indexing
					await ReferenceDialog.OpenIndexAllAssetDialog();
				}
				
				SessionState.SetBool(key, true);
			}
		}
	}
}