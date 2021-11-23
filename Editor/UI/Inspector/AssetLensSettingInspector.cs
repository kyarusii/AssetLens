using UnityEditor;

namespace AssetLens.UI
{
	using Reference;
	
	// [CustomEditor(typeof(Setting))]
	public class AssetLensSettingInspector : AssetLensInspector
	{
		protected override void Constructor()
		{
			LoadLayout("Setting");
		}
	}
}