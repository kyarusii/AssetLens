using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetLens.Reference
{
	internal static class ReferenceSerializer
	{
		private static readonly string VERSION_KEY = $"{Application.productName}.Reference.Version";

		internal static bool HasLocalVersion()
		{
			return EditorPrefs.HasKey(VERSION_KEY);
		}
		
		internal static int GetLocalVersion()
		{
			return EditorPrefs.GetInt(VERSION_KEY, -1);
		}

		internal static void SetLocalVersion(int version)
		{
			EditorPrefs.SetInt(VERSION_KEY, version);
		}

		internal static void ClearVersion()
		{
			EditorPrefs.DeleteKey(VERSION_KEY);
		}

		internal static RefData Deseriallize(string guid)
		{
			string path = FileSystem.ReferenceCacheDirectory + $"/{guid}.ref";
		
			BinaryReader r = new BinaryReader(File.OpenRead(path));
			uint version = r.ReadUInt32();
			
			RefData asset = new RefData(guid, version);
			DeserializeBody(ref asset, ref r, version);

			r.Close();
			return asset;
		}

		private static void DeserializeBody(ref RefData data, ref BinaryReader r, uint version)
		{
			switch (version)
			{
				case 100:
					Deserialize_100(ref data, ref r);
					break;
				default:
					break;
			}
		}

		private static void Deserialize_100(ref RefData cacheData, ref BinaryReader r)
		{
			cacheData.objectType = r.ReadString();
			cacheData.objectName = r.ReadString();
			cacheData.objectPath = r.ReadString();

			int ownCount = r.ReadInt32();
			for (int i = 0; i < ownCount; i++)
			{
				cacheData.ownGuids.Add(r.ReadString());
			}

			int byCount = r.ReadInt32();
			for (int i = 0; i < byCount; i++)
			{
				cacheData.referedByGuids.Add(r.ReadString());
			}
		}

		internal static void Serialize(RefData data)
		{
			string path = FileSystem.ReferenceCacheDirectory + $"/{data.guid}.ref";
			
			BinaryWriter w = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
			
			SerializeBody(data, ref w, Setting.INDEX_VERSION);
			w.Close();
		}

		private static void SerializeBody(RefData data, ref BinaryWriter w, uint version)
		{
			switch (version)
			{
				case 100:
					Serialize_100(data, ref w);
					break;
				
				default:
					break;
			}
		}

		private static void Serialize_100(RefData data, ref BinaryWriter w)
		{
			w.Write((uint)100);
			w.Write(data.objectType);
			w.Write(data.objectName);
			w.Write(data.objectPath);
			
			int ownCount = data.ownGuids.Count;
			w.Write(ownCount);

			for (int i = 0; i < ownCount; i++)
			{
				w.Write(data.ownGuids[i]);
			}

			int byCount = data.referedByGuids.Count;
			w.Write(byCount);

			for (int i = 0; i < byCount; i++)
			{
				w.Write(data.referedByGuids[i]);
			}
		}
	}
}