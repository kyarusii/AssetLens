using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetLens.Reference
{
	internal class ReferenceReplacementWindow : EditorWindow
	{
		private DefaultAsset m_searchDirectory = default;
		private Object[] m_targets = Array.Empty<Object>();
		private List<string> m_foundGuids = new List<string>();

		private string m_selectedGuid;

		private bool isDirty = false;

		private Vector2 scroll1 = new Vector2();
		private Vector2 scroll2 = new Vector2();

		private Object newObject = default;

		private string GetRootPath()
		{
			if (m_searchDirectory)
			{
				return AssetDatabase.GetAssetPath(m_searchDirectory);
			}

			return "Assets";
		}

		private void OnGUI()
		{
			m_searchDirectory =
				(DefaultAsset)EditorGUILayout.ObjectField(m_searchDirectory, typeof(DefaultAsset), false);

			EditorGUILayout.LabelField("Path", GetRootPath());

			int size = m_targets.Length;
			int newSize = EditorGUILayout.IntField("Count", size);

			if (size != newSize)
			{
				Array.Resize(ref m_targets, newSize);
				isDirty = true;
			}

			scroll1 = EditorGUILayout.BeginScrollView(scroll1, GUILayout.Height(160));
			
			EditorGUI.indentLevel++;
			for (int i = 0; i < newSize; i++)
			{
				Object previous = m_targets[i];
				m_targets[i] = EditorGUILayout.ObjectField($"element {i}", m_targets[i], typeof(Object), false);

				if (!ReferenceEquals(previous, m_targets[i]))
				{
					isDirty = true;
				}
			}
			EditorGUI.indentLevel--;
			
			EditorGUILayout.EndScrollView();

			// search guids
			if (isDirty)
			{
				m_foundGuids.Clear();
				List<string> _guids = new List<string>();
				foreach (Object target in m_targets)
				{
					if (target == null) continue;
					
					string targetPath = AssetDatabase.GetAssetPath(target);
					string assetContent = File.ReadAllText(targetPath);

					var guids = ReferenceUtil.ParseOwnGuids(assetContent);
					_guids.AddRange(guids);
				}

				var hashSet = new HashSet<string>(_guids);
				m_foundGuids.AddRange(hashSet);
			}
			
			EditorGUILayout.LabelField("Found Guids", m_foundGuids.Count.ToString());

			scroll2 = EditorGUILayout.BeginScrollView(scroll2, GUILayout.Height(160));
			
			foreach (string guid in m_foundGuids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (!string.IsNullOrWhiteSpace(path))
				{
					Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
					
					if (asset != null)
					{
						// EditorGUILayout.ObjectField(asset.name, asset, typeof(Object), false);

						var preview = AssetPreview.GetAssetPreview(asset);
						// EditorGUI.DrawPreviewTexture(new Rect(0,0, 16,16), preview);
						if (GUILayout.Button($"[{guid}] {path}"))
						{
							m_selectedGuid = guid;
						}
						continue;
					}					
				}
				
				// EditorGUILayout.LabelField("guid", guid);
				if (GUILayout.Button(guid))
				{
					m_selectedGuid = guid;
				}
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.LabelField("Selected", m_selectedGuid);
			newObject = EditorGUILayout.ObjectField("New Object", newObject, typeof(Object), false);

			if (GUILayout.Button("Run", GUILayout.Height(32)))
			{
				var newPath = AssetDatabase.GetAssetPath(newObject);
				var newGuid = AssetDatabase.AssetPathToGUID(newPath); 
				
				foreach (Object target in m_targets)
				{
					string path = AssetDatabase.GetAssetPath(target);
					string assetContent = File.ReadAllText(path);

					string newAssetContent = assetContent.Replace(m_selectedGuid, newGuid);
					
					File.WriteAllText(path, newAssetContent);
					AssetDatabase.ImportAsset(path);
				}
				
				AssetDatabase.Refresh();
				AssetDatabase.SaveAssets();
			}
		}
	}
}