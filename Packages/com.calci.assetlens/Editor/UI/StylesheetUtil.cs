using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

#pragma warning disable CS0162

namespace AssetLens.UI
{
	internal static class StylesheetUtil
	{
		internal static void AddSquareRoundButton(this VisualElement root)
		{
			root.AddStylesheet("RoundSquareButton");
		}
		
		internal static void AddHeader(this VisualElement root)
		{
			root.AddStylesheet("Header");
		}
		
		internal static void AddSwitchToggle(this VisualElement root)
		{
			root.AddStylesheet("SwitchToggle");
		}

		internal static void AddStylesheet(this VisualElement root, string name)
		{
#if UNITY_2021_2_OR_NEWER
			// transitions
			var path = FileSystem.LocalStylesheetDirectory  + $"{name}.uss";
			
			if (File.Exists(Path.GetFullPath(path)))
			{
				var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
				root.styleSheets.Add(styleSheet);
			}

			return;
#endif
			string path2 = FileSystem.StylesheetDirectory  + $"{name}.uss";
			var styleSheet2 = AssetDatabase.LoadAssetAtPath<StyleSheet>(path2);
			root.styleSheets.Add(styleSheet2);
		}
		
		internal static void AddTopBar(this VisualElement root)
		{
			string path = FileSystem.ComponentDirectory  + "Stylesheets/TopBar.uss";
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
			root.styleSheets.Add(styleSheet);
		}
	}
}

#pragma warning restore CS0162