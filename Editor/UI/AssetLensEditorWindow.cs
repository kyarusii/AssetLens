using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	/// <summary>
	/// Shared EditorWindow for UI Toolkit
	/// </summary>
	internal abstract class AssetLensEditorWindow : EditorWindow
	{
		protected VisualElement root = default;
		protected VisualElement clonedLayout = default;
		
		protected void CreateGUI()
		{
			root = rootVisualElement;
			
			this.SetAntiAliasing(4);
			Constructor();
		}

		protected abstract void Constructor();

		protected virtual void LoadLayout(string filename)
		{
			VisualTreeAsset visualTree = GetLayout(filename);
			clonedLayout = visualTree.CopyTree();
			
// #if UNITY_2020_3_OR_NEWER
// 			clonedLayout = visualTree.Instantiate();
// #else
//             clonedLayout = visualTree.CloneTree();
// #endif
			
			root.Add(clonedLayout);
		}

		protected VisualTreeAsset GetLayout(string filename)
		{
			string path = FileSystem.LayoutDirectory + filename + ".uxml";
			return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
		}

		protected virtual void LoadStylesheet(string filename)
		{
			string path = FileSystem.StylesheetDirectory + filename + ".uss";
			StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
			
			root.styleSheets.Add(styleSheet);
		}
	}
}