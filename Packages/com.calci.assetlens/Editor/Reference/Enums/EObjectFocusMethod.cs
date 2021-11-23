namespace AssetLens.Reference
{
	public enum EObjectFocusMethod
	{
		/// <summary>
		/// Setting 설정을 따름
		/// </summary>
		Default = 0,
		/// <summary>
		/// PropertyWindow 사용
		/// </summary>
		PropertyWindow,
		/// <summary>
		/// Selection.objects 사용
		/// </summary>
		Selection,
		/// <summary>
		/// EditorGUIUtility.PingObject 사용
		/// </summary>
		Ping,
	}
}