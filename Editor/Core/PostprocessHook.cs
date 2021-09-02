using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace RV
{
	internal sealed class PostprocessHook : AssetPostprocessor {
		private static void OnPostprocessAllAssets(
			string[] importedAssets, string[] deletedAssets, 
			string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (!Config.IsEnabled) return;
			
			foreach (string asset in importedAssets)
			{
				OnAssetImport(asset);
			}
			
			foreach (string asset in deletedAssets)
			{
				OnAssetDelete(asset);
			}
			
			foreach (string asset in movedAssets)
			{
				
			}
			
			foreach (string asset in movedFromAssetPaths)
			{
				
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
				OnAssetCreate(path, guid);
			}
		}

		private static void OnAssetCreate(string path, string guid)
		{
			if (path.Contains("Packages/Reference")) return;
			
			// 새로 만들었으면 이 에셋을 레퍼런스된게 있을 수 없으므로 그냥 프로필만 생성 ctrl-z로 복구하는거면 문제생길수있음...
			RefData.New(guid).Save();
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
	}
}