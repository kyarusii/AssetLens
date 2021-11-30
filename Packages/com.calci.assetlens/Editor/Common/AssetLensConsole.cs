using System;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

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
			Verbose += Debug.Log;
			Ping += Debug.Log;
		}

#if DEBUG_ASSETLENS
		public static Action<string> Log = delegate(string msg) {  };
#else
		[Conditional("DEBUG_ASSETLENS")]
		public static void Log(string msg)
		{
		}
#endif
		
		public static Action<string> Verbose = delegate(string msg) {  };
		public static Action<string, UnityEngine.Object> Ping = delegate(string s, Object o) {  };
	}

	internal static class R
	{
		public static string D(string msg)
		{
			return $"<color=#D99090FF>[AssetLens:Dev]</color> {msg}";
		}
		
		public static string L(string msg)
		{
			return $"<color=#D99090FF>[AssetLens]</color> {msg}";
		}
	}
}