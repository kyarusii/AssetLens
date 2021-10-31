using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	internal class GhostRefData : RefData
	{
		public GhostRefData(string guid) : base(guid, Setting.INDEX_VERSION)
		{
#if DEBUG_ASSETLENS
			var path = AssetDatabase.GUIDToAssetPath(guid);
			Debug.LogWarning($"GhostData Created! ({path})\n{guid}");
#endif
		}
	}
}