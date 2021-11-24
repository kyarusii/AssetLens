using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
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

#if UNITY_2021_1_OR_NEWER
		internal static List<string> GetChoices(this DropdownField dropdownField)
		{
#if UNITY_2021_2_OR_NEWER
			return dropdownField.choices;
#else
			var fields = typeof(DropdownField).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.Name == "m_Choices")
				{
					return fieldInfo.GetValue(dropdownField) as List<string>;
				}
			}
			
			return new List<string>();
#endif
		}

		internal static void SetChoices(this DropdownField dropdownField, List<string> choices)
		{
#if UNITY_2021_2_OR_NEWER
			dropdownField.choices = choices;
#else
			var fields = typeof(DropdownField).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.Name == "m_Choices")
				{
					fieldInfo.SetValue(dropdownField, choices);
					Debug.Log("Set Value");
					return;
				}
			}
#endif
		}
		
#endif
	}
}