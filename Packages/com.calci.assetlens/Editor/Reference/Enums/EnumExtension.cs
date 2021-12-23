using System;

namespace AssetLens.Reference
{
	internal static class EnumExtension
	{
		internal static float AsSecond(this EViewRefreshRate rate)
		{
			switch (rate)
			{
				case EViewRefreshRate.NEVER:
					return float.MaxValue;
				case EViewRefreshRate.RARELY:
					return 1000f;
				case EViewRefreshRate.SELDOM:
					return 334f;
				case EViewRefreshRate.SOMETIME:
					return 100f;
				case EViewRefreshRate.OFTEN:
					return 50f;
				case EViewRefreshRate.USUALLY:
					return 20f;
				default:
					return 1000f;
			}
		}
	}
}