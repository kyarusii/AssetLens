using System.IO;
using UnityEditor;
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

		internal const string UIToolkitDir = PackageDirectory + "/Editor/UI Toolkit/";
		internal const string LayoutDirectory = UIToolkitDir + "Layouts/";
		internal const string StylesheetDirectory = UIToolkitDir + "Stylesheets/";
		internal const string ComponentDirectory = UIToolkitDir + "Components/";
		
		internal const string LocalStylesheetDirectory = SettingDirectory + "/Stylesheets/";

#if DEBUG_ASSETLENS
		[MenuItem("Tools/Asset Lens_DEV/Open/Cache Directory")]
#endif
		internal static void OpenCaheDirectory()
		{
			// 탐색기 열어서 선택해줌
			// EditorUtility.RevealInFinder(ReferenceCacheDirectory);
			
			// 탐색기로 열어줌
			Application.OpenURL($"file://{Path.GetFullPath(ReferenceCacheDirectory)}");
		}
	}
}