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

		internal const string SettingDirectory = "Assets/Editor Default Resources";
		
		internal const string LayoutDirectory = PackageDirectory + "/Editor/UI/Layouts/";
		internal const string StylesheetDirectory = PackageDirectory + "/Editor/UI/Stylesheets/";

	}
}