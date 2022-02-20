using UnityEditor;

namespace Sample
{
	internal static class MenuItemSample
	{
		[MenuItem("Sample/Run")]
		private static void Run()
		{
			LensAPI.Initialize();
		}
	}
}