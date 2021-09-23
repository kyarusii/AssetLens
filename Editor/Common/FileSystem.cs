using UnityEngine;

namespace AssetLens
{
	internal static class FileSystem
	{
		internal static readonly string ReferenceCacheDirectory =
			Application.dataPath.Replace("Assets", "Library/ReferenceCache");
		
		internal const string PackageDirectory = "Packages/" + Constants.PackageName;

		internal static readonly string Manifest =
			Application.dataPath.Replace("Assets", "Packages/manifest.json");
	}
}