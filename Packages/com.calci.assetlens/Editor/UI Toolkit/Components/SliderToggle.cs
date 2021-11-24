// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace AssetLens.UI.Component
// {
// 	public class SliderToggle : VisualElement
// 	{
// 		protected readonly VisualElement m_Slider;
// 		
// 		internal Clickable m_Clickable;
// 		protected bool m_Value;
//
// 		public SliderToggle() : base()
// 		{
// 			AddToClassList("sliderToggle");
//
// 			m_Slider = new VisualElement()
// 			{
// 				name = "sliderToggle-stick",
// 				pickingMode = PickingMode.Position,
// 			};
// 			
// 			Add(m_Slider);
// 			
// 			m_Slider.AddToClassList("sliderToggle-slider");
// 			
// 			this.AddManipulator((this.m_Clickable = new Clickable(new Action<EventBase>(this.OnClickEvent))));
// 			
// 			this.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit));
// 			this.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown));
// 		}
//
// 		private void OnKeyDown(KeyDownEvent evt)
// 		{
// 			IPanel panel = this.panel;
// 			if (panel == null || panel.contextType != ContextType.Editor || evt.keyCode != KeyCode.KeypadEnter && evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.Space)
// 				return;
// 			this.ToggleValue();
// 			evt.StopPropagation();
// 		}
//
// 		private void OnClickEvent(EventBase evt)
// 		{
// 			if (evt.eventTypeId == EventBase<MouseUpEvent>.TypeId())
// 			{
// 				if (((IMouseEvent) evt).button != 0)
// 					return;
// 				this.ToggleValue();
// 			}
// 			else
// 			{
// 				if (evt.eventTypeId != EventBase<PointerUpEvent>.TypeId() && evt.eventTypeId != EventBase<ClickEvent>.TypeId() || ((IPointerEvent) evt).button != 0)
// 					return;
// 				this.ToggleValue();
// 			}
// 		}
//
// 		private void OnNavigationSubmit(NavigationSubmitEvent evt)
// 		{
// 			this.ToggleValue();
// 			evt.StopPropagation();
// 		}
//
// 		protected void ToggleValue() => this.value = !this.value;
//
// 		public bool value {
// 			get
// 			{
// 				return this.m_Value;
// 			}
// 			set
// 			{
// 				if (m_Value == value) return;
// 				if (panel != null)
// 				{
// 					using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(this.m_Value, value))
// 					{
// 						pooled.target = this;
// 						this.SetValueWithoutNotify(value);
// 						this.SendEvent(pooled);
// 					}
// 				}
// 				else
// 				{
// 					this.SetValueWithoutNotify(value);
// 				}
// 			}
// 		}
//
// 		public virtual void SetValueWithoutNotify(bool newValue)
// 		{
// 			m_Value = newValue;
// 			this.MarkDirtyRepaint();
// 			
// 			Debug.Log("Clicked");
// 		}
//
// 		#region Constructor
// 		
// 		public new class UxmlFactory : UxmlFactory<SliderToggle, UxmlTraits> { }
//
// 		public new class UxmlTraits : VisualElement.UxmlTraits
// 		{
// 			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
// 				get { yield break; }
// 			}
//
// 			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
// 			{
// 				base.Init(ve, bag, cc);
// 			}
// 		}
// 		
// 		#endregion
// 	}
// }