namespace AssetLens
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
			major = version / 10000;
			minor = version % 10000 / 100;
			maintenance = version % 100;
		}

		public static implicit operator uint(Version version)
		{
			return version.major * 10000 + version.minor * 100 + version.maintenance;
		}

		public static implicit operator Version(uint version)
		{
			return new Version(version);
		}

		public override string ToString()
		{
			return $"{major}.{minor}.{maintenance}";
		}
	}
}