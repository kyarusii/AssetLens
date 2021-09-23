using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RV
{
	internal sealed class ReferenceAssetModificationProcessor : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			if (!ReferenceSetting.IsEnabled)
			{
				return AssetDeleteResult.DidNotDelete;
			}

			try
			{
				string guid = AssetDatabase.AssetPathToGUID(assetPath);

				RefData assetReference = RefData.Get(guid);
				if (assetReference.referedByGuids.Count > 0)
				{
					StringBuilder sb = new StringBuilder();

					sb.AppendLine("이 에셋은 다음 에셋으로부터 사용되고 있습니다.");
					sb.AppendLine("그래도 삭제하시겠습니까?");
					sb.AppendLine();

					foreach (string referedGuid in assetReference.referedByGuids)
					{
						string referedAssetPath = AssetDatabase.GUIDToAssetPath(referedGuid);
						sb.AppendLine(referedAssetPath);
					}

					bool allowDelete = EditorUtility.DisplayDialog("경고!", sb.ToString(), "삭제", "취소");
					if (!allowDelete)
					{
						Debug.Log("삭제가 취소되었습니다.");
						return AssetDeleteResult.DidDelete;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return AssetDeleteResult.FailedDelete;
			}

			return AssetDeleteResult.DidNotDelete;
		}
	}
}