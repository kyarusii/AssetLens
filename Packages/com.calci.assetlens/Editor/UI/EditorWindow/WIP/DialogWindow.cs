using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AssetLens.UI
{
	internal sealed class DialogWindow : AssetLensEditorWindow
	{
		private event Action OnAccept =delegate {  };
		private event Action OnCancel =delegate {  };
		private event Action OnClose = delegate {  };
		
		private bool buttonInteracted;
		
		private string titleText;
		private string contentText;
		private string acceptText;
		private string cancelText;

		protected override void Constructor()
		{
			LoadLayout("DialogBox");
			LoadStylesheet("DialogBox");

			root.Q<Label>("title-label").text = titleText;
			root.Q<Label>("content-label").text = contentText;

			var btnOk = root.Q<Button>("btn-primary");
			var btnCancel = root.Q<Button>("btn-secondary");
			
			btnOk.text = acceptText;
			btnCancel.text = cancelText;


			OnAccept += () =>
			{
				buttonInteracted = true;
			};

			OnCancel += () =>
			{
				buttonInteracted = true;
			};

			btnOk.clicked += Accept;
			btnCancel.clicked += Cancel;
		}

		private void Accept()
		{
			OnAccept();
			Close();
		}

		private void Cancel()
		{
			OnCancel();
			Close();
		}

		private void Shutdown()
		{
			OnClose();
		}

		private void OnDestroy()
		{
			if (!buttonInteracted)
			{
				Shutdown();
			}
		}

#if DEBUG_ASSETLENS
		[MenuItem("Window/Asset Lens/Dialog Box", false, 140)]
#endif
		private static void DialogBoxTest()
		{
			OpenDialogModal("Caution!", "There is no indexed data. Generate?", "Generate", "Later",
				() => Debug.Log("오케이"),
				() => Debug.Log("취소"),
				() => Debug.Log("종료"));
		}

#if DEBUG_ASSETLENS
		[MenuItem("Window/Asset Lens/Test Box", false, 140)]
#endif
		private static void Test()
		{
			OpenDialog("Caution!", "There is no indexed data. Generate?", "Generate", "Later",
				() => Debug.Log("오케이"),
				() => Debug.Log("취소"),
				() => Debug.Log("종료"));
		}
		
		public static DialogWindow OpenDialog(string title, string content, string ok, string cancel,
			Action onAccept, Action onCancel, Action onClose)
		{
			DialogWindow wnd = CreateInstance<DialogWindow>();

			// wnd.titleText = title;
			// wnd.contentText = content;
			// wnd.acceptText = ok;
			// wnd.cancelText = cancel;
			//
			// wnd.OnAccept += onAccept;
			// wnd.OnCancel += onCancel;
			// wnd.OnClose += onClose;
			//
			// var size = new Vector2(360, 140);
			// wnd.minSize = wnd.maxSize = size;
			//
			// Rect main = EditorGUIUtility.GetMainWindowPosition();
			// Rect pos = wnd.position;
			//
			// float centerWidth = (main.width - pos.width) * 0.5f;
			// float centerHeight = (main.height - pos.height) * 0.5f;
			// pos.x = main.x + centerWidth;
			// pos.y = main.y + centerHeight;
			//
			// wnd.position = pos;
			//
			// wnd.titleContent = new GUIContent("Dialog");
			// // wnd.Focus();
			// // wnd.Repaint();
			// wnd.ShowPopup();
			// // wnd.ShowModal();

			return wnd;
		}
		
		public static void OpenDialogModal(string title, string content, string ok, string cancel,
			Action onAccept, Action onCancel, Action onClose)
		{
			DialogWindow wnd = CreateInstance<DialogWindow>();

			// wnd.titleText = title;
			// wnd.contentText = content;
			// wnd.acceptText = ok;
			// wnd.cancelText = cancel;
			//
			// wnd.OnAccept += onAccept;
			// wnd.OnCancel += onCancel;
			// wnd.OnClose += onClose;
			//
			// var size = new Vector2(360, 140);
			// wnd.minSize = wnd.maxSize = size;
			//
			// Rect main = EditorGUIUtility.GetMainWindowPosition();
			// Rect pos = wnd.position;
			//
			// float centerWidth = (main.width - pos.width) * 0.5f;
			// float centerHeight = (main.height - pos.height) * 0.5f;
			// pos.x = main.x + centerWidth;
			// pos.y = main.y + centerHeight;
			//
			// wnd.position = pos;
			//
			// wnd.titleContent = new GUIContent("Dialog");
			// wnd.Focus();
			// wnd.Repaint();
			// wnd.ShowModal();
		}
	}
}