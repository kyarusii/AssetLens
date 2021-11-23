using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if !DEBUG_ASSETLENS
#pragma warning disable CS0414
#else
#pragma warning disable CS0612
#endif

namespace AssetLens.Reference
{
	internal partial class Setting : ScriptableObject
	{
		/*
		 * 리팩터링 플랜
		 *
		 * 옵션 세분화
		 * 1. 인덱싱 옵션
		 * - GUID 정규식 기반 탐지 / 유니티 EditorUtility 디펜던시 탐지
		 * - 씬 오브젝트 별도 인덱싱 / 안 함
		 * - 패키지 하위 디렉터리 전체 인덱싱 / 안 함
		 * 2. 뷰 옵션
		 * 2.1 뷰어 옵션
		 * - 씬 오브젝트 레퍼런스 뷰어 활성화 / 비활성화
		 * - PlayMode 레퍼런스 뷰어 활성화 / 비활성화
		 * 2.2 렌즈 옵션
		 * - 인스펙터 렌즈 사용여부
		 * - 링크가 없을 때 인스펙터 렌즈 보이기
		 * 3. 데이터 옵션
		 * - Deprecated 된 이전 버전 캐시 데이터 자동 업데이트
		 * - 인덱서 버전 오버라이딩
		 * - 커스텀 정규식
		 * 4. Safe Delete 옵션
		 * - 활성화
		 * - 자동으로 레퍼런스 리플레이서 연결
		 * - 링크가 있는 경우 따로 히스토리 저장
		 * 5. Reference Replacer
		 * - 작업 후 작업내역 표시
		 */

		#region Field

		[Header("글로벌 옵션")]
		[SerializeField] private bool enabled = false;
		[SerializeField] private string localization = "English";
		
		[Header("1. 인덱싱 옵션")]
#if DEBUG_ASSETLENS
		[Obsolete]
#endif
		[SerializeField] private bool useEditorUtilityWhenSearchDependencies = false;

		/// <summary>
		/// GUID 정규식에 따라 인덱싱
		/// </summary>
		[HideInInspector] public bool IndexByGuidRegEx = true;
		/// <summary>
		/// 씬 오브젝트 계층 인스턴스 별 인덱싱
		/// </summary>
		[HideInInspector] public bool IndexSceneObject = true;
		/// <summary>
		/// 패키지 하위 디렉터리 전체 인덱싱
		/// </summary>
		[HideInInspector] public bool IndexPackageSubDir = true;
		
		[Header("2.1 뷰어 옵션")]
#if DEBUG_ASSETLENS
		[Obsolete]
#endif
		[SerializeField] private bool traceSceneObject = false;
#if DEBUG_ASSETLENS
		[Obsolete]
#endif
		[SerializeField] private bool pauseInPlaymode = true;
#if DEBUG_ASSETLENS
		[Obsolete]
#endif
		[SerializeField] private bool displayIndexerVersion = false;

		/// <summary>
		/// 씬 오브젝트 선택 시 뷰어 활성화
		/// </summary>
		[HideInInspector] public bool ViewSceneObject = false;

		/// <summary>
		/// 플레이모드에서 뷰어 활성화
		/// </summary>
		[HideInInspector] public bool ViewInPlayMode = false;

		[Header("2.2 렌즈 옵션")]
#if DEBUG_ASSETLENS
		[Obsolete]
#endif
		[SerializeField] private bool displaySceneObjectInstanceId = false;
	
		[HideInInspector] public bool InspectorLensEnable = true;
		[HideInInspector] public bool InspectorHideWithNoLink = false;
		[HideInInspector] public bool InspectorDrawObjectInstanceId = true;
		
		[Header("3. 데이터 관리")]
		[SerializeField] private bool autoUpgradeCachedData = false;


		[Header("4. SafeDelete 옵션")] 
		[HideInInspector] public bool SafeDeleteEnabled = true;
		[HideInInspector] public bool SafeDeleteOpenReferenceReplacer = true;
		[HideInInspector] public bool SafeDeleteSaveHistory = true;

		/// <summary>
		/// 작업 후 로그 내역 보이기
		/// </summary>
		[Header("5. Ref Replacer")]
		[HideInInspector] public bool RefReplacerDisplayLog = true;

		/// <summary>
		/// 이전 버전으로 인덱싱 된 캐시 자동 업데이트
		/// </summary>
		[HideInInspector] public bool DataAutoUpdate = true;
		
		/// <summary>
		/// 인덱서 버전 오버라이드
		/// </summary>
		[HideInInspector] public bool DataOverrideIndexer = false;
		
		/// <summary>
		/// 오버라이드 한 인덱선 버전 정보
		/// </summary>
		[HideInInspector] public uint DataOverrideIndexerVersion = INDEX_VERSION;
		
		[Header("99. 개발자 옵션")]
		[SerializeField] private bool useUIElementsWindow = false;

		[HideInInspector] public string SuccessColorCode = "#50FF00FF";
		[HideInInspector] public string ErrorColorCode = "#FF5000FF";
		
		#endregion

		public static event Action<Setting> onSettingChange = delegate(Setting setting) {  };

		public static void SetSettingDirty()
		{
			onSettingChange(Setting.Inst);
		}

		public static bool DisplaySceneObjectId => GetOrCreateSettings().displaySceneObjectInstanceId;
		
		public static bool PauseInPlaymode => GetOrCreateSettings().pauseInPlaymode;
		
		public static bool TraceSceneObject => GetOrCreateSettings().traceSceneObject;
		
		public static bool UseEditorUtilityWhenSearchDependencies =>
			GetOrCreateSettings().useEditorUtilityWhenSearchDependencies;

		public static bool DisplayIndexerVersion => GetOrCreateSettings().displayIndexerVersion;
		
		public static string Localization {
			get
			{
				return GetOrCreateSettings().localization;
			}
			set
			{
				GetOrCreateSettings().localization = value;
				EditorUtility.SetDirty(GetOrCreateSettings());
			}
		}
		
		internal static Localize LoadLocalization {
			get
			{
				string locale = GetOrCreateSettings().localization;
				string fullPath = Path.GetFullPath($"{FileSystem.PackageDirectory}/Languages/{locale}.json");

				string json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<Localize>(json);
			}
		}

		public static bool UseUIElements {
			get => GetOrCreateSettings().useUIElementsWindow;
		}

		public static bool AutoUpgradeCachedData {
			get
			{
#if DEBUG_ASSETLENS
				return GetOrCreateSettings().autoUpgradeCachedData;
#else
				return false;
#endif
			}
		}

		public static bool HasRootDir()
		{
			DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.ReferenceCacheDirectory);
			return rootDirectory.Exists;
		}

		public static void CreateRootDir()
		{
			DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.ReferenceCacheDirectory);
			if (!rootDirectory.Exists)
			{
				rootDirectory.Create();
			}
		}
	}
}

#if !DEBUG_ASSETLENS
#pragma warning restore CS0414
#else
#pragma warning restore CS0612
#endif