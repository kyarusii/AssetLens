namespace AssetLens.Reference
{
	public enum EViewRefreshRate
	{
		/// <summary>
		/// 10 times in a second
		/// </summary>
		SOMETIME = 0,
		
		/// <summary>
		/// never
		/// </summary>
		NEVER = 1,
		
		/// <summary>
		/// 1 time in a second
		/// </summary>
		RARELY = 2,
		
		/// <summary>
		/// 3 times in a second
		/// </summary>
		SELDOM = 3,
		
		/// <summary>
		/// 20 times in a second
		/// </summary>
		OFTEN = 4,
		
		/// <summary>
		/// 50 times in a second
		/// </summary>
		USUALLY = 5,
	}
}