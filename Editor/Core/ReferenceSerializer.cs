using UnityEditor;
using UnityEngine;

namespace RV
{
	internal static class ReferenceSerializer
	{
		private static readonly string VERSION_KEY = $"{Application.productName}.Reference.Version";

		internal static bool HasLocalVersion()
		{
			return EditorPrefs.HasKey(VERSION_KEY);
		}
		
		internal static int GetLocalVersion()
		{
			return EditorPrefs.GetInt(VERSION_KEY, -1);
		}

		internal static void SetLocalVersion(int version)
		{
			EditorPrefs.SetInt(VERSION_KEY, version);
		}

		internal static void ClearVersion()
		{
			EditorPrefs.DeleteKey(VERSION_KEY);
		}
	}
}