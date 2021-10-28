using UnityEngine.UIElements;

namespace AssetLens.Reference.Component
{
	public class AssetReference : TextElement
	{
		public new static readonly string ussClassName = "asset-reference";
		private string guid;

		public AssetReference() : this(string.Empty)
		{
		}

		public AssetReference(string guid)
		{
			this.AddToClassList(AssetReference.ussClassName);
			this.guid = guid;
		}

		public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<AssetReference, AssetReference.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
		}
	}
}