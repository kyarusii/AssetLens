using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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
		internal static IEnumerable<string> ExplicitSearchGuid(string root, string pattern, string guid)
		{
			return Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories)
				.AsParallel()
				.Where(path =>
				{
					using (StreamReader sr = new StreamReader(path))
					{
						string line = sr.ReadToEnd();

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
		internal static IEnumerable<string> ExplicitSearchInPrefab(string guid)
		{
			return ExplicitSearchGuid(Application.dataPath, "*.prefab", guid);
		}

		internal static bool IsSceneObject(this GameObject target)
		{
			if (target == null)
			{
				return false;
			}

			return !string.IsNullOrWhiteSpace(target.scene.name);
		}

		internal static bool IsPersistent(this UnityEngine.Object target)
		{
			return !EditorUtility.IsPersistent(target);
		}

		internal static bool IsGuid(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (
					!(c >= '0' && c <= '9' ||
					  c >= 'a' && c <= 'z')
				)
				{
					return false;
				}
			}

			return true;
		}

		internal static bool IsReadOnlyPackage(this string path)
		{
			return Path.GetFullPath(path).Contains("PackageCache");
		}
	}
}