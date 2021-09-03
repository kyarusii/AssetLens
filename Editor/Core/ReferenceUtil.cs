using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RV
{
	internal static class ReferenceUtil
	{
		/// <see href="https://gist.github.com/sr4dev/20958d4d102ffb05479afc8e01aef416"/>
		/// <summary>
		/// return enumerable path that contains specific guid.
		/// </summary>
		/// <param name="root">root directory for searching</param>
		/// <param name="pattern">patterns (*.prefab)</param>
		/// <param name="guid">target guid to search</param>
		/// <returns></returns>
		public static IEnumerable<string> ExplicitSearchGuid(string root, string pattern, string guid)
		{
			return Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories)
				.AsParallel()
				.Where(path =>
				{
					using (var sr = new StreamReader(path))
					{
						var line = sr.ReadToEnd();

						if (line.IndexOf(guid, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							return true;
						}

						return false;
					}
				});
		}

		/// <summary>
		/// 인덱싱된 캐시를 사용하지 않고, 명시적으로 파일 시스템을 검색합니다.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static IEnumerable<string> ExplicitSearchInPrefab(string guid)
		{
			return ExplicitSearchGuid(Application.dataPath, "*.prefab", guid);
		}
	}
}