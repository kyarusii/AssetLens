using System.Collections.Generic;
using System.Linq;
using AssetLens.Reference;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetLens.Missing
{
	public class MissingComponentInfo
	{
		
	}

	internal static class MissingComponentUtil
	{
		private static readonly HashSet<Object> missingComponents = new HashSet<Object>();

		[MenuItem(ReferenceMenuName.TOOL + "/Missing/Find Missing Components In Opened Scenes.")]
		internal static void FindMissingComponent()
		{
			missingComponents.Clear();
			
			int sceneCount = EditorSceneManager.loadedSceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene current = SceneManager.GetSceneAt(i);

				var rootObjects = current.GetRootGameObjects();
				foreach (GameObject rootObject in rootObjects)
				{
					FindMissingComponentRecursive(rootObject);
				}
			}

			Selection.objects = missingComponents.ToArray();
			
			Debug.Log($"{missingComponents.Count} missing objects found!");
			missingComponents.Clear();
		}

		private static void FindMissingComponentRecursive(GameObject root)
		{
			// Find MissingComponents
			Component[] components = root.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component == null)
				{
					// @TODO :: Compare with guid
					missingComponents.Add(root);
				}
			}
			
			// Run Recursively.
			int childCount = root.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject childRoot = root.transform.GetChild(i).gameObject;
				FindMissingComponentRecursive(childRoot);
			}
		} 
	} 
}