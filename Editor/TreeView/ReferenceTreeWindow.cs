using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetLens.Reference.TreeWindow
{
	internal class ReferenceTreeWindow : EditorWindow
	{
		[NonSerialized] private bool m_Initialized;

		[SerializeField]
		private TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading

		[SerializeField] private MultiColumnHeaderState m_MultiColumnHeaderState;
		private SearchField m_SearchField;
		private MultiColumnTreeView m_TreeView;
		// private MyTreeAsset m_MyTreeAsset;
		private Object m_targetObject;

#if DEBUG_ASSETLENS
		[MenuItem("Window/Asset Lens/Reference Tree Window", false, 112)]
		public static ReferenceTreeWindow GetWindow()
		{
			ReferenceTreeWindow window = GetWindow<ReferenceTreeWindow>();
			
			window.titleContent = new GUIContent("Reference Tree");
			window.Focus();
			window.Repaint();
			
			return window;
		}
#endif

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			MyTreeAsset myTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as MyTreeAsset;
			if (myTreeAsset != null)
			{
				ReferenceTreeWindow window = GetWindow();
				window.SetTreeAsset(myTreeAsset);
				return true;
			}

			return false; // we did not handle the open
		}

		private void SetTreeAsset(MyTreeAsset myTreeAsset)
		{
			// m_MyTreeAsset = myTreeAsset;
			m_Initialized = false;
		}

		private Rect multiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 60);

		private Rect toolbarRect => new Rect(20f, 10f, position.width - 40f, 20f);

		private Rect bottomToolbarRect => new Rect(20f, position.height - 18f, position.width - 40f, 16f);

		public MultiColumnTreeView treeView => m_TreeView;

		private void InitIfNeeded()
		{
			if (!m_Initialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if (m_TreeViewState == null)
				{
					m_TreeViewState = new TreeViewState();
				}

				bool firstInit = m_MultiColumnHeaderState == null;
				MultiColumnHeaderState headerState =
					MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
				if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
				{
					MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
				}

				m_MultiColumnHeaderState = headerState;

				MyMultiColumnHeader multiColumnHeader = new MyMultiColumnHeader(headerState);
				if (firstInit)
				{
					multiColumnHeader.ResizeToFit();
				}

				TreeModel<RefTreeElement> treeModel = new TreeModel<RefTreeElement>(GetData());

				m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

				m_SearchField = new SearchField();
				m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

				m_Initialized = true;
			}
		}

		private IList<RefTreeElement> GetData()
		{
			var list = new List<RefTreeElement>();
			if (m_targetObject != null)
			{
				string path = AssetDatabase.GetAssetPath(m_targetObject);
				string guid = AssetDatabase.AssetPathToGUID(path);

				RefData data = RefData.Get(guid);
				list.Add(new RefTreeElement(data, -1));

				var usedBy = data.referedByGuids;
				foreach (string usedByGuid in usedBy)
				{
					RefData usedRef = RefData.Get(usedByGuid);
					list.Add(new RefTreeElement(usedRef, 0));
				}
			}

			return list;
			// if (m_MyTreeAsset != null && m_MyTreeAsset.treeElements != null && m_MyTreeAsset.treeElements.Count > 0)
			// {
			// 	return m_MyTreeAsset.treeElements;
			// }
			//
			// // generate some test data
			// return MyTreeElementGenerator.GenerateRandomTree(130);
		}

		private void OnSelectionChange()
		{
			if (!m_Initialized)
			{
				return;
			}

			var selected = Selection.objects.FirstOrDefault();
			if (selected != null && selected != m_targetObject)
			{
				m_targetObject = selected;
				
				m_TreeView.treeModel.SetData(GetData());
				m_TreeView.Reload();
			}

			// MyTreeAsset myTreeAsset = Selection.activeObject as MyTreeAsset;
			// if (myTreeAsset != null && myTreeAsset != m_MyTreeAsset)
			// {
			// 	m_MyTreeAsset = myTreeAsset;
			// 	m_TreeView.treeModel.SetData(GetData());
			// 	m_TreeView.Reload();
			// }
		}

		private void OnGUI()
		{
			InitIfNeeded();

			// SearchBar(toolbarRect);
			DoTreeView(multiColumnTreeViewRect);
			BottomToolBar(bottomToolbarRect);
		}

		private void SearchBar(Rect rect)
		{
			treeView.searchString = m_SearchField.OnGUI(rect, treeView.searchString);
		}

		private void DoTreeView(Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}

		private void BottomToolBar(Rect rect)
		{
			GUILayout.BeginArea(rect);

			using (new EditorGUILayout.HorizontalScope())
			{
				string style = "miniButton";
				if (GUILayout.Button("Expand All", style))
				{
					treeView.ExpandAll();
				}

				if (GUILayout.Button("Collapse All", style))
				{
					treeView.CollapseAll();
				}

				GUILayout.FlexibleSpace();

				// GUILayout.Label(m_MyTreeAsset != null ? AssetDatabase.GetAssetPath(m_MyTreeAsset) : string.Empty);

				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Set sorting", style))
				{
					MyMultiColumnHeader myColumnHeader = (MyMultiColumnHeader)treeView.multiColumnHeader;
					myColumnHeader.SetSortingColumns(new int[] { 4, 3, 2 }, new[] { true, false, true });
					myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
				}


				GUILayout.Label("Header: ", "minilabel");
				if (GUILayout.Button("Large", style))
				{
					MyMultiColumnHeader myColumnHeader = (MyMultiColumnHeader)treeView.multiColumnHeader;
					myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
				}

				if (GUILayout.Button("Default", style))
				{
					MyMultiColumnHeader myColumnHeader = (MyMultiColumnHeader)treeView.multiColumnHeader;
					myColumnHeader.mode = MyMultiColumnHeader.Mode.DefaultHeader;
				}

				if (GUILayout.Button("No sort", style))
				{
					MyMultiColumnHeader myColumnHeader = (MyMultiColumnHeader)treeView.multiColumnHeader;
					myColumnHeader.mode = MyMultiColumnHeader.Mode.MinimumHeaderWithoutSorting;
				}

				GUILayout.Space(10);

				if (GUILayout.Button("values <-> controls", style))
				{
					treeView.showControls = !treeView.showControls;
				}
			}

			GUILayout.EndArea();
		}
	}


	internal class MyMultiColumnHeader : MultiColumnHeader
	{
		private Mode m_Mode;

		public enum Mode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state)
		{
			mode = Mode.DefaultHeader;
		}

		public Mode mode {
			get => m_Mode;
			set
			{
				m_Mode = value;
				switch (m_Mode)
				{
					case Mode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case Mode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case Mode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
				}
			}
		}

		protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			// Default column header gui
			base.ColumnHeaderGUI(column, headerRect, columnIndex);

			// Add additional info for large header
			if (mode == Mode.LargeHeader)
			{
				// Show example overlay stuff on some of the columns
				if (columnIndex > 2)
				{
					headerRect.xMax -= 3f;
					TextAnchor oldAlignment = EditorStyles.largeLabel.alignment;
					EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
					GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
					EditorStyles.largeLabel.alignment = oldAlignment;
				}
			}
		}
	}
}