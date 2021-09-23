using UnityEditor;

#pragma warning disable CS0168

namespace AssetLens.Reference
{
	internal sealed class ReferencePostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets, string[] deletedAssets,
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			ReferenceCallback.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets,
				movedFromAssetPaths);
		}
	}
}

#pragma warning restore CS0168