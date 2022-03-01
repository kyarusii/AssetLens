using System;
using AssetLens.Reference;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AssetLens.UI.Component
{
	internal class AssetReference : VisualElement
	{
		private static readonly string ussClassName = "asset-reference";
		private string guid;

		private Button button;
		private Image image;

		public AssetReference() : this(string.Empty)
		{
		}

		public AssetReference(string guid)
		{
			this.AddToClassList(AssetReference.ussClassName);
			this.guid = guid;

			this.button = new Button();
			this.image = new Image();
			
			this.Add(button);
			this.button.Add(image);
			
			EAssetCategory assetCategory = ReferenceUtil.GUID.GetAssetCategory(guid);

			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
			
			switch (assetCategory)
			{
				case EAssetCategory.Object:
					// @TODO : use uss style instead space
					if (obj != null)
					{
						button.text = $"     {obj.name} ({ReferenceUtil.AddSpacesToSentence(obj.GetType().Name)})";
						button.tooltip = assetPath;
						Texture img = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
						image.image = img;
						image.AddToClassList("reference-view-image");    
					}
					else
					{
						button.text = $"     (null) (guid:{guid})";
						button.tooltip = assetPath;
						Texture img = EditorGUIUtility.ObjectContent(null, typeof(Object)).image;
						image.image = img;
						image.AddToClassList("reference-view-image"); 
					}
                        
					break;
                    
				case EAssetCategory.DefaultResource:
					button.text = "Default Resource";
					button.SetEnabled(false);
					break;
                    
				case EAssetCategory.BuiltInExtra:
					button.text = "Built-in Resource";
					button.SetEnabled(false);
					break;
                    
				case EAssetCategory.Others:
					button.text = "Other Internals";
					button.SetEnabled(false);
					break;
                    
				default:
					throw new ArgumentOutOfRangeException();
			}

			// stylesheets
			AddToClassList("reference-view-container");
			this.button.AddToClassList("reference-view-button");

			this.button.clickable.clicked += () =>
			{
				ReferenceUtil.Focus(obj);
			};
		}

		public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<AssetReference, AssetReference.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}
	}
}