using System;

namespace AssetLens.Reference
{
	internal static class EnumExtension
	{
		internal static float AsSecond(this EViewRefreshRate rate)
		{
			return rate switch
			{
				EViewRefreshRate.NEVER => float.MaxValue,
				EViewRefreshRate.RARELY => 1000f,
				EViewRefreshRate.SELDOM => 334f,
				EViewRefreshRate.SOMETIME => 100f,
				EViewRefreshRate.OFTEN => 50f,
				EViewRefreshRate.USUALLY => 20f,
				_ => 50f
			};
		}
	}
}