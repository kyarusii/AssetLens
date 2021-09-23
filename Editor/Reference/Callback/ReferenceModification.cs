using UnityEditor;

namespace AssetLens.Reference
{
	internal sealed class ReferenceModification : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			return ReferenceCallback.OnWillDeleteAsset(assetPath, options);
		}
	}
}