using System.Threading.Tasks;
using UnityEditor;

namespace AssetLens
{
	internal static class AssetLensDialog
	{
		internal static async Task OpenIndexAllAssetDialog()
		{
			if (EditorUtility.DisplayDialog(
				Localize.Inst.dialog_titleContent,
				Localize.Inst.dialog_noIndexedData,
				Localize.Inst.dialog_enablePlugin,
				Localize.Inst.dialog_disablePlugin))
			{
				await AssetLensCache.IndexAssets();
					
				AssetLensSetting.IsEnabled = true;
			}
			else
			{
				AssetLensSetting.IsEnabled = false;
			}
		}
	}
}