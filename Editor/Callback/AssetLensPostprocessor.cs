using UnityEditor;

#pragma warning disable CS0168

namespace AssetLens
{
	internal sealed class AssetLensPostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets, string[] deletedAssets,
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			AssetLensCallback.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets,
				movedFromAssetPaths);
		}
	}
}

#pragma warning restore CS0168