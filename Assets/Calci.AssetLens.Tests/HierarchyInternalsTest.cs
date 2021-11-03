using NUnit.Framework;
using UnityEditor;

namespace AssetLens.Tests
{
	public class HierarchyInternalsTest
	{
		[Test]
		public void SetFilterTest()
		{
			Assert.True(Internals.Hierarchy.SetSearchFilter("t:GameObject", SearchableEditorWindow.SearchMode.All));
		}
	}
}