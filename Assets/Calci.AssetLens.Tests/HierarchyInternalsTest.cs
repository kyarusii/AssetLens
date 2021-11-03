using NUnit.Framework;
using UnityEditor;

namespace AssetLens.Tests
{
	public class HierarchyInternalsTest
	{
		[Test]
		public void SetFilterTest()
		{
			// unity-test-runner don't have hierarchy window.
			// must be added creating hierarchy editor
			// Assert.True(Internals.Hierarchy.SetSearchFilter("t:GameObject", SearchableEditorWindow.SearchMode.All));
		}
	}
}