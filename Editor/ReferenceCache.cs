using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace RV
{
	internal static class ReferenceCache
	{
		[MenuItem("Tools/Reference/Index All Assets")]
		private static void IndexAllAssets()
		{
			if (!EditorUtility.DisplayDialog("주의", "이 작업은 시간이 오래 소요될 수 있습니다.\n계속하시겠습니까?", "계속", "취소"))
			{
				return;
			}

			Task indexAssets = IndexAssets();
		}

		[MenuItem("Tools/Reference/Log Selection %&r")]
		private static void LogReferences()
		{
			Object obj = Selection.activeObject;
			if (obj == null) return;
			
			string path = AssetDatabase.GetAssetPath(obj);
			string guid = AssetDatabase.AssetPathToGUID(path);

			RefData asset = RefData.Get(guid);
			foreach (string foundGuid in asset.ownGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				Debug.Log($"<color=#7FFF00>레퍼런스하고 있는 에셋 : {foundPath}</color>",
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
			
			foreach (string foundGuid in asset.referedByGuids)
			{
				string foundPath = AssetDatabase.GUIDToAssetPath(foundGuid);
				
				Debug.Log($"<color=#FF0F8F>이 에셋을 레퍼런스하고 있는 에셋 : {foundPath}</color>", 
					AssetDatabase.LoadAssetAtPath<Object>(foundPath));
			}
		}

		public static async Task IndexAssets()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			
			var assets = AssetDatabase.FindAssets("", new[] { "Assets" })
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(File.Exists);
			
			var queue = new Queue<string>(assets);
			int allCount = queue.Count;
			
			string msg = await ReadWork(20);
			
			Debug.Log(msg);

			async Task<string> ReadWork(int threadCount)
			{
				// file-text contents
				var fileMap = new Dictionary<string, string>(allCount);
				// guid-reference guids
				var guidRefOwnerMap = new Dictionary<string, HashSet<string>>();
				var guidRefByMap = new Dictionary<string, HashSet<string>>();
				
				for (int i = 0; i < threadCount; i++)
				{
					Indexing();
				}

				while (fileMap.Keys.Count != allCount)
				{
					EditorUtility.DisplayProgressBar("인덱싱...", 
						$"Worker : {threadCount} : ({fileMap.Count}/{allCount})", fileMap.Count / (float)allCount);
					
					// refresh rate
					await Task.Delay(10);
				}
				
				SetGuidMap();
				
				Dictionary<string, RefData> dataMap = new Dictionary<string, RefData>();
				foreach (string key in guidRefByMap.Keys)
				{
					dataMap[key] = new RefData(key)
					{
						guid = key,
						referedByGuids = guidRefByMap[key].ToList(),
					};
				}

				foreach (string key in guidRefOwnerMap.Keys)
				{
					if (!dataMap.TryGetValue(key, out var asset))
					{
						asset = new RefData(key)
						{
							guid = key,
						};
					}

					asset.ownGuids = guidRefOwnerMap[key].ToList();
					dataMap[key] = asset;
				}

				// 파일로 저장
				foreach (string guid in dataMap.Keys)
				{
					RefData asset = dataMap[guid];
					asset.Save();
				}

				stopwatch.Stop();
				EditorUtility.ClearProgressBar();
				
				async void Indexing()
				{
					while (queue.Count > 0)
					{
						string path = queue.Dequeue();
						StreamReader reader = new StreamReader(File.OpenRead(path));
						string value = await reader.ReadToEndAsync();
						reader.Close();
						
						fileMap[path] = value;

						// 가지고있는 레퍼런스 저장
						string guid = AssetDatabase.AssetPathToGUID(path);

						var owningGuids = RefData.ParseOwnGuids(value);
						guidRefOwnerMap[guid] = new HashSet<string>(owningGuids);
					}
				}

				void SetGuidMap()
				{
					int guidCount = guidRefOwnerMap.Count;
					int currentCount = 0;
					
					foreach (string guidOwner in guidRefOwnerMap.Keys)
					{
						var referenceGuids = guidRefOwnerMap[guidOwner];
						foreach (string referedGuid in referenceGuids)
						{
							if (!guidRefByMap.TryGetValue(referedGuid, out var hashSet))
							{
								hashSet = new HashSet<string>();
							}

							hashSet.Add(guidOwner);
							guidRefByMap[referedGuid] = hashSet;
						}

						currentCount++;
						
						EditorUtility.DisplayProgressBar("CreateGuidMap...", 
							$"({currentCount}/{guidCount})", currentCount / (float)guidCount);
					}
				}

				return ($"{allCount} files in Assets ({stopwatch.ElapsedMilliseconds * 0.001f:N1}s)");
			}
		}
	}
}