using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace AssetLens.Reference.Component
{
	public class TopBar : VisualElement
	{
		public Button closeButton { get; set; }
		public Button questionButton { get; set; }
		public string text { get; set; }

		public TopBar() : this((Action) null, (Action)null)
		{
		}
		
		public TopBar(Action closeAction, Action questionAction)
		{
			AddToClassList("top-bar");

			closeButton = new Button();
			closeButton.text = "X";
			closeButton.AddToClassList("btn-close");
			closeButton.AddToClassList("btn");
			closeButton.name = "closeButton";
			Add(closeButton);
			
			if (closeAction != null)
			{
				closeButton.clicked += closeAction;
			}

			questionButton = new Button();
			questionButton.text = "?";
			questionButton.AddToClassList("btn-question");
			questionButton.AddToClassList("btn");
			questionButton.name = "questionButton";
			Add(questionButton);

			if (questionAction != null)
			{
				questionButton.clicked += questionAction;
			}
		}
		
		public new class UxmlFactory : UxmlFactory<TopBar, UxmlTraits> { }

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			private UxmlStringAttributeDescription m_String = new UxmlStringAttributeDescription()
			{
				name = "text",
				defaultValue = "X",
			};

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var ate = ve as TopBar;

				ate.text = m_String.GetValueFromBag(bag, cc);
			}
		}
	}
}