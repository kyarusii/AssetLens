using UnityEngine.UIElements;

namespace AssetLens.UI
{
	internal static class UIToolkitExtension
	{
		internal static void SetHorizontalVisibility(this ScrollView scrollView, bool visible)
		{
#if UNITY_2021_1_OR_NEWER
			scrollView.horizontalScrollerVisibility = visible ? ScrollerVisibility.Auto : ScrollerVisibility.Hidden;
#else
			scrollView.horizontalScroller.visible = visible;
#endif
		}
		
		internal static void SetVerticalVisibility(this ScrollView scrollView, bool visible)
		{
#if UNITY_2021_1_OR_NEWER
			scrollView.verticalScrollerVisibility = visible ? ScrollerVisibility.Auto : ScrollerVisibility.Hidden;
#else
			scrollView.verticalScroller.visible = visible;
#endif
		}
	}
}