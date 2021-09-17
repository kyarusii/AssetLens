using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if !DEBUG_REFERENCE
#pragma warning disable CS0168
#endif

namespace RV
{
	internal class RefData
	{
		/// <summary>
		/// from file name
		/// </summary>
		public string guid;
		
		/// <summary>
		/// from unity object type
		/// </summary>
		public string objectType;
		
		/// <summary>
		/// from unity object name
		/// </summary>
		public string objectName;

		/// <summary>
		/// from asset database path
		/// </summary>
		public string objectPath;

		public List<string> ownGuids = new List<string>();
		public List<string> referedByGuids = new List<string>();

		private Version version;

		public string GetVersion()
		{
			return version.ToString();
		}

		public RefData(string guid)
		{
			this.guid = guid;

			try
			{
				objectPath = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrWhiteSpace(objectPath))
				{
					objectType = "INVALID";
					objectPath = "NO_PATH_DATA";
					objectName = "NO_PATH_DATA";
				}
				else
				{
					Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(objectPath);
				
					objectType = obj == null ? "NULL" : obj.GetType().FullName;
					objectName = obj == null ? "NULL" : obj.name;
				}	
			}
			catch (Exception e)
			{
#if DEBUG_REFERENCE
				Debug.LogException(e);
#endif
			}
		}

		public void Save()
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";

			BinaryWriter w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));

			// version for integration
			version = ReferenceSetting.INDEX_VERSION;
			
			w.Write(version);
			w.Write(objectType);
			w.Write(objectName);
			w.Write(objectPath);

			int ownCount = ownGuids.Count;
			w.Write(ownCount);

			for (int i = 0; i < ownCount; i++)
			{
				w.Write(ownGuids[i]);
			}

			int byCount = referedByGuids.Count;
			w.Write(byCount);

			for (int i = 0; i < byCount; i++)
			{
				w.Write(referedByGuids[i]);
			}

			w.Close();
		}

		public void Remove()
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			File.Delete(path);
		}

		public static RefData Get(string guid)
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			
			if (!File.Exists(path))
			{
				// 없으면 새로 만듦
				return New(guid);
			}

			RefData asset = new RefData(guid);

			if (asset.ownGuids == null)
			{
				asset.ownGuids = new List<string>();
			}

			if (asset.referedByGuids == null)
			{
				asset.referedByGuids = new List<string>();
			}

			BinaryReader r = new BinaryReader(File.OpenRead(path));

			asset.version = r.ReadUInt32();
			asset.objectType = r.ReadString();
			asset.objectName = r.ReadString();
			asset.objectPath = r.ReadString();

			int ownCount = r.ReadInt32();
			for (int i = 0; i < ownCount; i++)
			{
				asset.ownGuids.Add(r.ReadString());
			}

			int byCount = r.ReadInt32();
			for (int i = 0; i < byCount; i++)
			{
				asset.referedByGuids.Add(r.ReadString());
			}

			r.Close();

			return asset;
		}

		public static RefData New(string guid)
		{
			RefData asset = new RefData(guid);

			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			string assetContent = File.ReadAllText(assetPath);

			List<string> owningGuids = ParseOwnGuids(assetContent);

			// 보유한 에셋에다 레퍼런스 밀어넣기
			foreach (string owningGuid in owningGuids)
			{
				if (Exist(owningGuid))
				{
					RefData ownAsset = Get(owningGuid);
					if (!ownAsset.referedByGuids.Contains(guid))
					{
						ownAsset.referedByGuids.Add(guid);
						ownAsset.Save();
					}
				}
				else
				{
					RefData ownAsset = New(owningGuid);
					ownAsset.referedByGuids.Add(guid);
					ownAsset.Save();
				}
			}

			asset.ownGuids = owningGuids;
			asset.version = ReferenceSetting.INDEX_VERSION;

			return asset;
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

			return owningGuids.ToList();
		}

		public static bool Exist(string guid)
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			return File.Exists(path);
		}
	}
}

#if !DEBUG_REFERENCE
#pragma warning restore CS0168
#endif