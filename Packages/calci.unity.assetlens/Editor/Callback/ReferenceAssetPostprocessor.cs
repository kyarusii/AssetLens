using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;

#pragma warning disable CS0168

namespace RV
{
	internal sealed class ReferenceAssetPostprocessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(
			string[] importedAssets, string[] deletedAssets,
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (!ReferenceSetting.IsEnabled)
			{
				return;
			}

			if (EditorApplication.timeSinceStartup < 60)
			{
				return;
			}

			try
			{
				foreach (string asset in importedAssets)
				{
#if DEBUG_ASSETLENS
					try
					{
#endif
						if (asset.Contains("ProjectSettings")) continue;
						OnAssetImport(asset);

#if DEBUG_ASSETLENS
					}
					catch (Exception e)
					{
						Debug.LogError(asset);
						Debug.LogException(e);
					}
#endif
				}

				foreach (string asset in deletedAssets)
				{
#if DEBUG_ASSETLENS
					try
					{
#endif
						OnAssetDelete(asset);

#if DEBUG_ASSETLENS
					}
					catch (Exception e)
					{
						Debug.LogError(asset);
						Debug.LogException(e);
					}
#endif
				}

				foreach (string asset in movedAssets)
				{
#if DEBUG_ASSETLENS
					try
					{
#endif
						// 에셋 이동은 guid 변경과 큰 관계가 없음
						OnAssetMoved(asset);
#if DEBUG_ASSETLENS
					}
					catch (Exception e)
					{
						Debug.LogError(asset);
						Debug.LogException(e);
					}
#endif
				}

				foreach (string asset in movedFromAssetPaths)
				{
#if DEBUG_ASSETLENS
					try
					{
#endif
						// 에셋 이동은 guid 변경과 큰 관계가 없음
						OnAssetMoved(asset);
#if DEBUG_ASSETLENS
					}
					catch (Exception e)
					{
						Debug.LogError(asset);
						Debug.LogException(e);
					}
#endif
				}
			}
			catch (Exception e)
			{
#if DEBUG_ASSETLENS
				Debug.LogException(e);
#endif
			}
		}

		private static void OnAssetImport(string path)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			if (RefData.Exist(guid))
			{
				OnAssetModify(path, guid);
			}
			else
			{
				// 폴더이면 패스
				if (!File.Exists(path))
				{
					return;
				}

				OnAssetCreate(path, guid);
			}
		}

		internal static void OnAssetDelete(string path)
		{
			string guid = AssetDatabase.AssetPathToGUID(path);
			RefData refAsset = RefData.Get(guid);

			// 이 에셋을 레퍼런스 하는 에셋정보들 편집
			foreach (string referedByGuid in refAsset.referedByGuids)
			{
				// 문제는 에셋 파일에는 미싱 상태로 남아있다는 점
				RefData referedAsset = RefData.Get(referedByGuid);

				referedAsset.ownGuids.Remove(guid);
				referedAsset.Save();
			}

			// 이 에셋이 레퍼런스하는 에셋 정보들 편집
			foreach (string ownGuid in refAsset.ownGuids)
			{
				RefData referedAsset = RefData.Get(ownGuid);

				referedAsset.referedByGuids.Remove(guid);
				referedAsset.Save();
			}

			refAsset.Remove();
		}

		internal static void OnAssetMoved(string path) { }

		private static void OnAssetCreate(string path, string guid)
		{
			// @TODO :: 커스텀 패키지도 관리되게 
			
// 			if (path.Contains("Packages/Reference"))
// 			{
// 				return;
// 			}
//
// #if !DEBUG_ASSETLENS
// 			if (path.Contains("Packages/")) return;
// #endif

			// 새로 만들었으면 이 에셋을 레퍼런스된게 있을 수 없으므로 그냥 프로필만 생성 ctrl-z로 복구하는거면 문제생길수있음...
			if (string.IsNullOrWhiteSpace(path))
			{
#if DEBUG_ASSETLENS
				Debug.LogError($"Something wrong in OnAssetCreate : ({path}, guid:{guid})");
#endif
				return;
			}

			if (!File.Exists(path))
			{
#if DEBUG_ASSETLENS
				Debug.LogError($"Something wrong in OnAssetCreate : ({path}, guid:{guid})");
#endif
				return;
			}

#if DEBUG_ASSETLENS
			string tempPath = AssetDatabase.GUIDToAssetPath(guid);
			if (tempPath != path)
			{
				Debug.LogError($"{tempPath} : {path} (guid: {guid})");
			}
#endif

			try
			{
				RefData.New(guid).Save();
			}
			catch (Exception e)
			{
#if DEBUG_ASSETLENS
				Debug.LogError(tempPath);
				Debug.LogError(path);

				Debug.LogException(e);
#endif
			}
		}

		private static void OnAssetModify(string path, string guid)
		{
			// 수정이면 이미 존재해야함.
			RefData asset = RefData.Get(guid);
			string assetContent = File.ReadAllText(path);
			List<string> newGuids = RefData.ParseOwnGuids(assetContent);

			// 갖고있는거중에 변경되었을 수 있음
			foreach (string previous in asset.ownGuids)
			{
				if (!newGuids.Contains(previous))
				{
					// 삭제됨!
					RefData lostRefAsset = RefData.Get(previous);
					lostRefAsset.referedByGuids.Remove(guid);
					lostRefAsset.Save();

					string assetPath = AssetDatabase.GUIDToAssetPath(previous);

					Debug.Log("레퍼런스 삭제", AssetDatabase.LoadAssetAtPath<Object>(assetPath));
				}
			}

			foreach (string current in newGuids)
			{
				if (!asset.ownGuids.Contains(current))
				{
					// 새로 생김!
					RefData newRefAsset = RefData.Get(current);
					newRefAsset.referedByGuids.Add(guid);
					newRefAsset.Save();

					string assetPath = AssetDatabase.GUIDToAssetPath(current);
					Debug.Log("레퍼런스 추가", AssetDatabase.LoadAssetAtPath<Object>(assetPath));
				}
			}

			asset.ownGuids = newGuids;
			asset.Save();
		}
	}
}

#pragma warning restore CS0168