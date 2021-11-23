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

	internal static class R
	{
		public static string L(string msg)
		{
			return $"<color=#D99090FF>[AssetLens]</color> {msg}";
		}
	}
}