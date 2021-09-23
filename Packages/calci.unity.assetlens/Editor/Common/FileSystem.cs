using UnityEngine;

namespace AssetLens
{
	internal static class FileSystem
	{
		internal static readonly string CacheDirectory =
			Application.dataPath.Replace("Assets", "Library/" + Constants.DisplayName + "Cache");
		
		internal const string PackageDirectory = "Packages/" + Constants.PackageName;
	}
}