using UnityEditor;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace AssetLens.Reference
{
	internal static class AssetLensCache
	{
		internal const string CacheSearchPattern = "*.ref";
		internal static async Task IndexAssetsAsync(uint version = Setting.INDEX_VERSION, bool indexCustomPackages = true, int taskCount = 20)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// var assets = AssetDatabase.FindAssets("", new[] { "Assets" })
			// 	.Select(AssetDatabase.GUIDToAssetPath)
			// 	.Where(File.Exists).ToList();

			var allAssets = AssetDatabase.GetAllAssetPaths()
				.Where(e => !Directory.Exists(e))
				.Where(File.Exists)
				.ToList();

			if (indexCustomPackages)
			{
				allAssets.AddRange(AssetDatabase.FindAssets("", new[] { "Packages" })
					.Select(AssetDatabase.GUIDToAssetPath)
					// 실제 패키지 경로에 있는 경우만
					.Where(path => !path.IsReadOnlyPackage())
					.Where(e => !Directory.Exists(e))
					.Where(File.Exists)
				);
			}

			var assets = new HashSet<string>(allAssets);
			
			Queue<string> queue = new Queue<string>(assets);
			int allCount = queue.Count;

			string msg = await ReadWork(taskCount);

			AssetLensConsole.Verbose(R.L(msg));
			
			ReferenceSerializer.SetLocalVersion((int)Setting.INDEX_VERSION);

			async Task<string> ReadWork(int threadCount)
			{
				// file-text contents
				Dictionary<string, string> fileMap = new Dictionary<string, string>(allCount);
				// guid-reference guids
				Dictionary<string, HashSet<string>> guidRefOwnerMap = new Dictionary<string, HashSet<string>>();
				Dictionary<string, HashSet<string>> guidRefByMap = new Dictionary<string, HashSet<string>>();

				for (int i = 0; i < threadCount; i++)
				{
					Indexing();
				}

				while (fileMap.Keys.Count != allCount)
				{
					EditorUtility.DisplayProgressBar(L.Inst.progressbar_readAssets,
						$"Worker : {threadCount} : ({fileMap.Count}/{allCount})", fileMap.Count / (float)allCount);

					// refresh rate
					await Task.Delay(10);
				}

				SetGuidMap();

				Dictionary<string, RefData> dataMap = new Dictionary<string, RefData>();
				foreach (string key in guidRefByMap.Keys)
				{
					dataMap[key] = new RefData(key, Setting.INDEX_VERSION)
					{
						guid = key,
						referedByGuids = guidRefByMap[key].ToList()
					};
				}

				foreach (string key in guidRefOwnerMap.Keys)
				{
					if (!dataMap.TryGetValue(key, out RefData asset))
					{
						// asset = RefData.New(key);
						asset = new RefData(key, Setting.INDEX_VERSION)
						{
							guid = key
						};
					}

					asset.ownGuids = guidRefOwnerMap[key].ToList();
					dataMap[key] = asset;
				}

				// 파일로 저장
				foreach (string guid in dataMap.Keys)
				{
					RefData asset = dataMap[guid];
					asset.Save(version);
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

						List<string> owningGuids = ReferenceUtil.ParseOwnGuids(value);
						guidRefOwnerMap[guid] = new HashSet<string>(owningGuids);
					}
				}

				void SetGuidMap()
				{
					int guidCount = guidRefOwnerMap.Count;
					int currentCount = 0;

					foreach (string guidOwner in guidRefOwnerMap.Keys)
					{
						HashSet<string> referenceGuids = guidRefOwnerMap[guidOwner];
						foreach (string referedGuid in referenceGuids)
						{
							if (!guidRefByMap.TryGetValue(referedGuid, out HashSet<string> hashSet))
							{
								hashSet = new HashSet<string>();
							}

							hashSet.Add(guidOwner);
							guidRefByMap[referedGuid] = hashSet;
						}

						currentCount++;

						EditorUtility.DisplayProgressBar(L.Inst.progressbar_createGuids,
							$"({currentCount}/{guidCount})", currentCount / (float)guidCount);
					}
				}

				return $"{allCount} files in Assets ({stopwatch.ElapsedMilliseconds * 0.001f:N1}s)";
			}
		}

		internal static async Task<int> CleanUpAssetsAsync()
		{
			await Task.Delay(10);

			string[] cacheFiles = GetIndexedFiles();
			foreach (string cacheFile in cacheFiles)
			{
				File.Delete(cacheFile);
			}

			return cacheFiles.Length;
		}
		
		internal static void IndexAssets(uint version = Setting.INDEX_VERSION, bool indexCustomPackages = true, int taskCount = 20)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// var assets = AssetDatabase.FindAssets("", new[] { "Assets" })
			// 	.Select(AssetDatabase.GUIDToAssetPath)
			// 	.Where(File.Exists).ToList();

			var allAssets = AssetDatabase.GetAllAssetPaths()
				.Where(e => !Directory.Exists(e))
				.Where(File.Exists)
				.ToList();

			if (indexCustomPackages)
			{
				allAssets.AddRange(AssetDatabase.FindAssets("", new[] { "Packages" })
					.Select(AssetDatabase.GUIDToAssetPath)
					// 실제 패키지 경로에 있는 경우만
					.Where(path => !path.IsReadOnlyPackage())
					.Where(e => !Directory.Exists(e))
					.Where(File.Exists)
				);
			}

			var assets = new HashSet<string>(allAssets);
			
			Queue<string> queue = new Queue<string>(assets);
			int allCount = queue.Count;

			string msg = ReadWork(taskCount);

			Debug.Log(msg);
			
			ReferenceSerializer.SetLocalVersion((int)Setting.INDEX_VERSION);

			string ReadWork(int threadCount)
			{
				// file-text contents
				Dictionary<string, string> fileMap = new Dictionary<string, string>(allCount);
				// guid-reference guids
				Dictionary<string, HashSet<string>> guidRefOwnerMap = new Dictionary<string, HashSet<string>>();
				Dictionary<string, HashSet<string>> guidRefByMap = new Dictionary<string, HashSet<string>>();

				// for (int i = 0; i < threadCount; i++)
				{
					Indexing();
				}

				// while (fileMap.Keys.Count != allCount)
				// {
				// 	EditorUtility.DisplayProgressBar(Localize.Inst.progressbar_readAssets,
				// 		$"Worker : {threadCount} : ({fileMap.Count}/{allCount})", fileMap.Count / (float)allCount);
				// }

				SetGuidMap();

				Dictionary<string, RefData> dataMap = new Dictionary<string, RefData>();
				foreach (string key in guidRefByMap.Keys)
				{
					dataMap[key] = new RefData(key, Setting.INDEX_VERSION)
					{
						guid = key,
						referedByGuids = guidRefByMap[key].ToList()
					};
				}

				foreach (string key in guidRefOwnerMap.Keys)
				{
					if (!dataMap.TryGetValue(key, out RefData asset))
					{
						// asset = RefData.New(key);
						asset = new RefData(key, Setting.INDEX_VERSION)
						{
							guid = key
						};
					}

					asset.ownGuids = guidRefOwnerMap[key].ToList();
					dataMap[key] = asset;
				}

				// 파일로 저장
				foreach (string guid in dataMap.Keys)
				{
					RefData asset = dataMap[guid];
					asset.Save(version);
				}

				stopwatch.Stop();
				EditorUtility.ClearProgressBar();

				void Indexing()
				{
					while (queue.Count > 0)
					{
						string path = queue.Dequeue();
						StreamReader reader = new StreamReader(File.OpenRead(path));
						string value = reader.ReadToEnd();
						reader.Close();

						fileMap[path] = value;

						// 가지고있는 레퍼런스 저장
						string guid = AssetDatabase.AssetPathToGUID(path);

						List<string> owningGuids = ReferenceUtil.ParseOwnGuids(value);
						guidRefOwnerMap[guid] = new HashSet<string>(owningGuids);
					}
				}

				void SetGuidMap()
				{
					int guidCount = guidRefOwnerMap.Count;
					int currentCount = 0;

					foreach (string guidOwner in guidRefOwnerMap.Keys)
					{
						HashSet<string> referenceGuids = guidRefOwnerMap[guidOwner];
						foreach (string referedGuid in referenceGuids)
						{
							if (!guidRefByMap.TryGetValue(referedGuid, out HashSet<string> hashSet))
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

				return $"{allCount} files in Assets ({stopwatch.ElapsedMilliseconds * 0.001f:N1}s)";
			}
		}

		internal static int CleanupCaches()
		{
			string[] cacheFiles = Directory.GetFiles(FileSystem.ReferenceCacheDirectory, "*.ref");
			foreach (string cacheFile in cacheFiles)
			{
				File.Delete(cacheFile);
			}
			
			return cacheFiles.Length;
		}

		internal static string[] GetIndexedFiles()
		{
			return Directory.GetFiles(FileSystem.ReferenceCacheDirectory, CacheSearchPattern);
		}
	}
}