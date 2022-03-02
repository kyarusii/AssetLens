using System.Collections.Generic;
using UnityEngine;

namespace AssetLens.Reference.TreeWindow
{
	
	// [CreateAssetMenu (fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
	internal class MyTreeAsset : ScriptableObject
	{
		[SerializeField] List<RefTreeElement> m_TreeElements = new List<RefTreeElement> ();

		internal List<RefTreeElement> treeElements
		{
			get { return m_TreeElements; }
			set { m_TreeElements = value; }
		}

		void Awake ()
		{
			// if (m_TreeElements.Count == 0)
			// 	m_TreeElements = MyTreeElementGenerator.GenerateRandomTree(160);
		}
	}
}
