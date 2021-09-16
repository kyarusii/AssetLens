using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace RV
{
	internal class RefData
	{
		internal struct Version
		{
			// 010000
			public uint major;
			// 0100
			public uint minor;
			// 01
			public uint maintenance;

			public Version(uint version)
			{
				this.major = version / 10000;
				this.minor = version % 10000 / 100;
				this.maintenance = version % 100;
			}

			public static implicit operator uint(Version version)
			{
				return version.major * 10000 + version.minor * 100 + version.maintenance;
			}

			public static implicit operator Version(uint version)
			{
				return new Version(version);
			}
		}
		
		public string guid;
		
		public List<string> ownGuids = new List<string>();
		public List<string> referedByGuids = new List<string>();

		private Version version;

		public RefData(string guid)
		{
			this.guid = guid;
		}
		
		public void Save()
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			
			BinaryWriter w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
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

			// 맨 앞에 인덱싱 해야하는데... 방법을 고민
			w.Write(version);
					
			w.Close();
		}

		public void Remove()
		{
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			File.Delete(path);
		}

		public static RefData Get(string guid)
		{
			RefData asset = new RefData(guid);
			string path = FileSystem.CacheDirectory + $"/{guid}.ref";
			if (!File.Exists(path))
			{
				return asset;
			}

			asset.ownGuids ??= new List<string>();
			asset.referedByGuids ??= new List<string>();

			BinaryReader r = new BinaryReader(File.OpenRead(path));
			
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

			bool isEndOfStream = r.BaseStream.Position == r.BaseStream.Length;
			if (isEndOfStream)
			{
				// 0.0.x 버전
				asset.version = 0;
			}
			else
			{
				// 버전 파싱
				asset.version = r.ReadUInt32();
			}
			
			r.Close();

			return asset;
		}

		public static RefData New(string guid)
		{
			RefData asset = new RefData(guid);
			
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			string assetContent = File.ReadAllText(assetPath);

			var owningGuids = ParseOwnGuids(assetContent);

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
			var owningGuids = new HashSet<string>();
			
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