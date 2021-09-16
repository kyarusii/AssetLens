using UnityEngine;

namespace RV
{
	internal static class FileSystem
	{
		internal static readonly string CacheDirectory = Application.dataPath.Replace("Assets", "Library/ReferenceCache");
		internal const string PackageDirectory = "Packages/kr.seonghwan.reference";
	}
}