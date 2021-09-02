using UnityEditor;
using UnityEngine;

namespace RV
{
	internal static class Config
	{
		public static bool IsEnabled {
			get
			{
				return EditorPrefs.GetBool($"{Application.productName}.Reference", false);
			}
			set
			{
				EditorPrefs.SetBool($"{Application.productName}.Reference", value);
			}
		}
	}
}