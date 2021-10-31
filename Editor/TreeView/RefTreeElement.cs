using System;

namespace AssetLens.Reference.TreeWindow
{
	[Serializable]
	internal class RefTreeElement : TreeElement
	{
		public string guid;
		public string path;
		public string type;
		public int size;

		public RefTreeElement(string guid, string path, string type, int size, string name, int depth, int id) : base (name, depth, id)
		{
			this.guid = guid;
			this.path = path;
			this.type = type;
			this.size = size;
		}

		private static int idCounter = 2000;

		public RefTreeElement(RefData refData, int depth) : base(refData.objectName, depth, idCounter)
		{
			idCounter++;
			
			this.guid = refData.guid;
			this.path = refData.objectPath;
			this.type = refData.objectType;
			this.size = 0;
		}
	}
}
