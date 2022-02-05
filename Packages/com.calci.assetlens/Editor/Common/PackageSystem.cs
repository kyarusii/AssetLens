using System.IO;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace AssetLens
{
	internal static class PackageSystem
	{
		/// <summary>
		/// 패키지 버전 가져옴
		/// </summary>
		/// <returns></returns>
		internal static async Task<string> GetVersion()
		{
			ListRequest list = Client.List(false);

			while (!list.IsCompleted)
			{
				await Task.Delay(10);
			}

			if (list.Status == StatusCode.Success)
			{
				var result = list.Result;
				foreach (PackageInfo info in result)
				{
					if (info.name == Constants.PackageName)
					{
						return info.version;
					}
				}

				return string.Empty;
			}

			Debug.LogError($"{list.Status} : {list.Error}");
			return string.Empty;
		}

		internal static async Task<string> Uninstall()
		{
			RemoveRequest request = Client.Remove(Constants.PackageName);
			
			while (!request.IsCompleted)
			{
				await Task.Delay(10);
			}

			if (request.Status == StatusCode.Success)
			{
				return $"Removed : {request.PackageIdOrName}";
			}

			return $"Failed : {request.Error.message}";
		}
		
		internal static bool IsReadOnlyPackage(this string path)
		{
			return Path.GetFullPath(path).Contains("PackageCache");
		}
	}
}