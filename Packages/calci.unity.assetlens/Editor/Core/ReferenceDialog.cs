using System.Threading.Tasks;
using UnityEditor;

namespace RV
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
				await ReferenceCache.IndexAssets();
					
				ReferenceSetting.IsEnabled = true;
			}
			else
			{
				ReferenceSetting.IsEnabled = false;
			}
		}
	}
}