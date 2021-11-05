using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
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

			return !string.IsNullOrWhiteSpace(target.scene.name) && !target.IsPersistent();
		}

		internal static bool IsPersistent(this UnityEngine.Object target)
		{
			return EditorUtility.IsPersistent(target);
		}

		internal static bool IsGuid(string text)
		{
			foreach (char c in text)
			{
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
		
		public static List<string> ParseOwnGuids(string assetContent)
		{
			HashSet<string> owningGuids = new HashSet<string>();

			// 정규식으로 보유한 에셋 검색
			Regex guidRegx = new Regex("guid:\\s?([a-fA-F0-9]+)");
			MatchCollection matches = guidRegx.Matches(assetContent);
			foreach (Match match in matches)
			{
				if (match.Success)
				{
					owningGuids.Add(match.Groups[1].Value);
				}
			}

			// 어드레서블
			guidRegx = new Regex("m_GUID:\\s?([a-fA-F0-9]+)");
			matches = guidRegx.Matches(assetContent);
			foreach (Match match in matches)
			{
				if (match.Success)
				{
					owningGuids.Add(match.Groups[1].Value);
				}
			}
			
			return owningGuids.ToList();
		}
		
		internal static class Path
		{
			internal const string UNITY_DEFAULT_RESOURCE = "Library/unity default resources";
			internal const string UNITY_BUILTIN_EXTRA = "Resources/unity_builtin_extra";	
		}
		
		internal static class GUID
		{
			internal const string UNITY_DEFAULT_RESOURCE = "0000000000000000e000000000000000";
			internal const string UNITY_BUILTIN_EXTRA    = "0000000000000000f000000000000000";

			internal static readonly string[] OTHER_INTERNALS = new[]
			{
				"0000000000000000a100000000000000",
				"0000000000000000b000000000000000",
				"0000000000000000b100000000000000",
				"0000000000000000c000000000000000",
				"0000000000000000c100000000000000",
				"0000000000000000d000000000000000",
				"0000000000000000d100000000000000",
				"0000000000000000f100000000000000",
			};

			internal static bool IsDefaultResource(string guid)
			{
				return guid == UNITY_DEFAULT_RESOURCE;
			}

			internal static bool IsBuiltInExtra(string guid)
			{
				return guid == UNITY_BUILTIN_EXTRA;
			}

			internal static bool IsOtherInternals(string guid)
			{
				return OTHER_INTERNALS.Contains(guid);
			}

			internal static EAssetCategory GetAssetCategory(string guid)
			{
				if (IsDefaultResource(guid))
				{
					return EAssetCategory.DefaultResource;
				}

				if (IsBuiltInExtra(guid))
				{
					return EAssetCategory.BuiltInExtra;
				}

				if (IsOtherInternals(guid))
				{
					return EAssetCategory.Others;
				}

				return EAssetCategory.Object;
			}
		}
		
		public static string AddSpacesToSentence(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;
			
			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]) && text[i - 1] != ' ')
					newText.Append(' ');
				newText.Append(text[i]);
			}
			
			return newText.ToString();
		}

		internal static string GetGuid(UnityEngine.Object obj)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			return AssetDatabase.AssetPathToGUID(path);
		}

		/// <summary>
		/// Find assets that don't have any reference links with other asset.
		/// </summary>
		/// <returns></returns>
		internal static string[] GetIsolatedAssets()
		{
			return null;
		}

		/// <summary>
		/// Validate the asset is placed in resources folder.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal static bool WillBePackAsResources(string path)
		{
#if UNITY_2021_2_OR_NEWER
			return path.Contains("Resources", StringComparison.OrdinalIgnoreCase);
#else
			return path.ToUpper().Contains("RESOURCES");
#endif
		}

		/// <summary>
		/// Validate the asset is related with the scene in build setting.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		internal static bool IsManagedBySceneInBuild(string path)
		{
			throw new NotImplementedException();
		}
	}
}