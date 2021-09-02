using UnityEngine;

namespace RV
{
	public class FileSystem
	{
		public static readonly string CacheDirectory = Application.dataPath.Replace("Assets", "Library/ReferenceCache");
	}
}