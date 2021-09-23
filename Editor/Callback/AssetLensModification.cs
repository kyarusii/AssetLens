using UnityEditor;

namespace AssetLens
{
	internal sealed class AssetLensModification : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			return AssetLensCallback.OnWillDeleteAsset(assetPath, options);
		}
	}
}