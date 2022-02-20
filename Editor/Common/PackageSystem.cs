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
		
		/// <summary>
		/// 패키지가 Library/PackageCache 디렉터리에 존재하는지 확인 (읽기 전용)
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal static bool IsReadOnlyPackage(string path)
		{
			return Path.GetFullPath(path).Contains("PackageCache");
		}

		internal static bool IsUnderDevelopment(string path)
		{
			return !PackageSystem.IsReadOnlyPackage(path);
		}
	}
}