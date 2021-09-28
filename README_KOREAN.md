# Asset Lens

[<p align="center"><img src="https://discordapp.com/api/guilds/889046470655893574/widget.png?style=banner2"></p>](https://discord.gg/h9WPFRNFBY)  


## **에셋 렌즈**에 관해서
**에셋 렌즈**는 각 에셋이 다른 에셋에서 몇 곳이나 사용되고 있는지와 같은 추가적인 정보 제공하는 종속성 추적 플러그인입니다.

이 플러그인은 미리 캐시된 guid 맵을 기반으로 어던 에셋이 다른 특정한 에셋에 종속성을 가지는치 추적할 수 있습니다.
유니티 엔진을 이용해 개발할 때 주로 고려되는 고통스러운 점 중에 하나인, 어던 에셋을 지울 때 어떤 에셋이 그 에셋을 사용하고 있는지 알 수 없다는 불편함을 해소해줍니다.

## 요구사항
- 모든 에셋은 `force-text` 옵션으로 직렬화되어야 합니다. (`ProjectSetting/Editor` 옵션에 위치)  

### 구동이 확인된 유니티 버전
<p align="center">
<img src="https://img.shields.io/badge/unity-2019.4f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2020.3f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2021.1f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2021.2b_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2022.1a_LTS-brightgreen.svg?style=flat-square&logo=unity">
</p>

## 설치
### 유니티 패키지 매니저 (Unity Package Manager)
```json
{
    "dependencies": {
        "com.calci.assetlens": "0.2.3"
    }
}
```

```json
{
    "scopedRegistries": [
        {
            "name": "npm",
            "url": "https://registry.npmjs.org",
            "scopes": [
                "com.calci"
            ]
        }
    ]
}
```

### OpenUPM
[![openupm](https://img.shields.io/npm/v/com.calci.assetlens?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.calci.assetlens/)
```bash
openupm add com.calci.assetlens
```

### 직접 다운로드 (권장되지 않음)
- Move to `ProjectRoot\Packages`
- Run command in terminal.
```bash
git clone https://github.com/seonghwan-dev/AssetLens
```

## 시작하기
- Execute `Tools/Asset Lens/Index All Assets` in MenuItem and wait until complete.  
- Configure `Asset Lens` settings in `Edit/Project Settings...` in MenuItem.
- Select an asset you want to know which asset references it and run `Find References In Project` context menu.

## Fundamentals
- Create a cache file per a asset file, see also [RefData.cs](Editor/Model/RefData.cs)
- Detect asset changes from `AssetPostprocessor`, see also [AssetLensPostprocessor.cs](Editor/Callback/AssetLensPostprocessor.cs)
- Detect an attempt to delete an asset from `AssetModificationProcessor`, see also [AssetLensModification.cs](Editor/Callback/AssetLensModification.cs)

## Features
- Display asset usage count in inspector.
- Find References In Project

### Reference Viewer Window
<p align="center">
<img src="https://user-images.githubusercontent.com/79823287/134523257-28173dc7-4fd5-406e-8ac9-56b148debedb.png" width="460">
</p>
<p align="center">
<img src="https://user-images.githubusercontent.com/79823287/134523437-166bf30b-ccdd-42ea-90ae-3084e0f013f6.png" width="460">
</p>


## Roadmap
- Safer Asset Delete ([#8](/../../issues/8))  
- Reference replacement wizard ([#9](/../../issues/9))  
- Reference dependency map visualizer  
- Detect references indexed in [Addressable](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/index.html)  

## Contributes
- Fork and clone at `ProjectName\Packages`
- Add an Scripting Define Symbol `DEBUG_ASSETLENS` at ProjectSettings/Player.
- Run `Tools/Asset Lens_DEV/Add New Language` to create a new localization profile.
- Run `Tools/Asset Lens_DEV/Update Language profiles` to add field after edit `Localize` class.
- Create PR.

![image](https://user-images.githubusercontent.com/79823287/131787910-1cc009e6-d483-4a87-afb0-a6ac31d3cf0d.png)  
![image](https://user-images.githubusercontent.com/79823287/131797772-078dda37-0917-4d98-abea-f09645e33a77.png)  
![image](https://user-images.githubusercontent.com/79823287/131797825-213d2927-db5a-47d0-a02d-bb87e0400b52.png)