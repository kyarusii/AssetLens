using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if !DEBUG_ASSETLENS
#pragma warning disable CS0168
#endif

namespace AssetLens.Reference
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
		/// <summary>
		/// last modified date time
		/// </summary>
		public long objectModifiedTime;

		public List<string> ownGuids = new List<string>();
		public List<string> referedByGuids = new List<string>();

		private Version version;

		public DateTime GetLastEditTime()
		{
			return DateTime.FromBinary(objectModifiedTime);
		}

		public uint GetVersion()
		{
			return version;
		}

		public bool IsOutdatedVersion()
		{
			return version < Setting.INDEX_VERSION;
		}

		public string GetVersionText()
		{
			return version.ToString();
		}

		protected RefData()
		{
		}

		public RefData(string guid, uint version)
		{
			this.guid = guid;
			this.version = version;

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
#if DEBUG_ASSETLENS
				Debug.LogException(e);
#endif
			}
		}

		public bool IsDirty()
		{
			string guidToPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.CompareOrdinal(guidToPath, objectPath) == 0)
			{
				return false;
			}
			
			return true;
		}

		public void UpdateObjectData()
		{
			objectPath = AssetDatabase.GUIDToAssetPath(guid);
			FileInfo fi = new FileInfo(objectPath);

			objectName = fi.Name.Replace(fi.Extension, "");
			
			Save();
		}

		public void Save(uint serializerVersion = Setting.INDEX_VERSION)
		{
			ReferenceSerializer.Serialize(this, serializerVersion);
		}

		public void Remove()
		{
			string path = FileSystem.ReferenceCacheDirectory + $"/{guid}.ref";
			File.Delete(path);
		}

		public static RefData Get(string guid)
		{
			string path = FileSystem.ReferenceCacheDirectory + $"/{guid}.ref";
			
			if (!File.Exists(path))
			{
				// 없으면 새로 만듦
				return New(guid);
			}

			RefData loaded = ReferenceSerializer.Deseriallize(guid);
			if (loaded.IsDirty())
			{
				loaded.UpdateObjectData();
				AssetLensConsole.Log($"CacheUpdated : {loaded.objectPath}");
			}

			return loaded;
		}

		public static RefData New(string guid)
		{
			RefData asset = new RefData(guid, Setting.INDEX_VERSION);

			string assetPath = AssetDatabase.GUIDToAssetPath(guid);

			if (!File.Exists(assetPath))
			{
				return GhostData(guid);
			}

			string assetContent= File.ReadAllText(assetPath);
			List<string> owningGuids = ReferenceUtil.ParseOwnGuids(assetContent);

			// 보유한 에셋에다 레퍼런스 밀어넣기
			foreach (string owningGuid in owningGuids)
			{
				if (CacheExist(owningGuid))
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
					if (ReferenceUtil.GUID.IsBuiltInExtra(owningGuid)
					|| ReferenceUtil.GUID.IsDefaultResource(owningGuid))
					// if (owningGuid == UNITY_BUILTIN_EXTRA || owningGuid == UNITY_DEFAULT_RESOURCE)
					{
						RefData builtinExtra = new RefData(owningGuid, Setting.INDEX_VERSION);
						
						builtinExtra.referedByGuids.Add(guid);
						builtinExtra.Save();
					}
					else
					{
						RefData ownAsset = New(owningGuid);
						ownAsset.referedByGuids.Add(guid);
						ownAsset.Save();
					}
				}
			}

			asset.ownGuids = owningGuids;
			asset.objectModifiedTime = DateTime.Now.ToBinary();

			return asset;
		}

		public static bool CacheExist(string guid)
		{
			string path = FileSystem.ReferenceCacheDirectory + $"/{guid}.ref";
			return File.Exists(path);
		}

		public static RefData GhostData(string guid)
		{
			return new GhostRefData(guid);
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0168
#endif