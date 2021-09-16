using UnityEngine;

namespace RV
{
	public class FileSystem
	{
		public static readonly string CacheDirectory = Application.dataPath.Replace("Assets", "Library/ReferenceCache");
		public static readonly string SettingPath =
			Application.dataPath.Replace("Assets", "ProjectSettings/ReferenceSettings.json");

		public static readonly string PackageDirectory = "Packages/kr.seonghwan.reference";
	}
}