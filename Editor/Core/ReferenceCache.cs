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

		public static async Task<int> CleanUpAssets()
		{
			await Task.Delay(2000);

			string[] cacheFiles = Directory.GetFiles(FileSystem.CacheDirectory, "*.ref");
			foreach (string cacheFile in cacheFiles)
			{
				File.Delete(cacheFile);
			}

			return cacheFiles.Length;
		}
	}
}