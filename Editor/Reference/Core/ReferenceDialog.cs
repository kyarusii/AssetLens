using System.Threading.Tasks;
using UnityEditor;

namespace AssetLens.Reference
{
	internal static class ReferenceDialog
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
					
				Setting.IsEnabled = true;
			}
			else
			{
				Setting.IsEnabled = false;
			}
		}
	}
}