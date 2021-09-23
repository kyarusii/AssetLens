using System;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	[InitializeOnLoad]
	internal static class ReferenceConsole
	{
		static ReferenceConsole()
		{
#if DEBUG_ASSETLENS
			Log += Debug.Log;
#endif
		}

		public static Action<string> Log = delegate(string msg) {  };
	}
}