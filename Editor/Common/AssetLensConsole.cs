using System;
using UnityEditor;
using UnityEngine;

namespace AssetLens
{
	[InitializeOnLoad]
	internal static class AssetLensConsole
	{
		static AssetLensConsole()
		{
#if DEBUG_ASSETLENS
			Log += Debug.Log;
#endif
		}

		public static Action<string> Log = delegate(string msg) {  };
	}
}