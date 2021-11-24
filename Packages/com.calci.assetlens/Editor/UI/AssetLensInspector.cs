using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	/// <summary>
	/// Shared Editor(Inspector) for UI Toolkit
	/// </summary>
	public abstract class AssetLensInspector : Editor
	{
		protected VisualElement root = default;
		
		public override VisualElement CreateInspectorGUI()
		{
			Constructor();
			
			return root ?? base.CreateInspectorGUI();
		}
		
		protected abstract void Constructor();
		
		protected virtual void LoadLayout(string filename)
		{
			VisualTreeAsset visualTree = GetLayout(filename);
			
			UnityEngine.Assertions.Assert.IsNotNull(visualTree);
			
#if UNITY_2020_3_OR_NEWER
			var clonedLayout = visualTree.Instantiate();
#else
            var clonedLayout = visualTree.CloneTree();
#endif

			root = clonedLayout;
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