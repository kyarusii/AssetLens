# Reference

[<p align="center"><img src="https://discordapp.com/api/guilds/889046470655893574/widget.png?style=banner2"></p>](https://discord.gg/h9WPFRNFBY)  


## About Reference
Reference is a dependency tracking plugin for UnityEditor that provides additional information such as the number of usage.

This plugin is based on pre-cached complementary guid map to trace which asset has dependencies to specific asset. 
The pain point that mainly considered with Unity is that when we delete an asset, we don't know which asset is using it. 

## Requirements
- All assets must be serialized as force-text option in ProjectSetting/Editor

### Compatibility
<p align="center">
<img src="https://img.shields.io/badge/unity-2019.4f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2020.3f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2021.1f_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2021.2b_LTS-brightgreen.svg?style=flat-square&logo=unity">
<img src="https://img.shields.io/badge/unity-2022.1a_LTS-brightgreen.svg?style=flat-square&logo=unity">
</p>

## Installation
### Unity Package Manager
```json
{
    "dependencies": {
        "calci.unity.assetlens": "0.2.0"
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
                "calci"
            ]
        }
    ]
}
```

### FileSystem
- Move to `ProjectRoot\Packages`  
- Run command in terminal.  
```bash
git clone https://github.com/seonghwan-dev/AssetLens
```  

### OpenUPM
```bash
openupm add calci.unity.assetlens
```

## QuickStart
- Execute `Tools/Reference/Index All Assets` in MenuItem and wait until complete.  
- Configure `Reference` settings in `Edit/Project Settings...` in MenuItem.
- Select an asset you want to know which asset references it and run `Find References In Project` context menu.

## Fundamentals
- Create a cache file per a asset file, see also [RefData.cs](Editor/Model/RefData.cs)
- Detect asset changes from `AssetPostprocessor`, see also [AssetLensPostprocessor.cs](Editor/Callback/AssetLensPostprocessor.cs)
- Detect an attempt to delete an asset from `AssetModificationProcessor`, see also [AssetLensModification.cs](Editor/Callback/AssetLensModification.cs)

## Features
- Display asset usage count in inspector.
- Find References In Project

## Roadmap
- Safer Asset Delete ([#8](/../../issues/8))  
- Reference replacement wizard ([#9](/../../issues/9))  
- Reference dependency map visualizer  
- Detect references indexed in [Addressable](https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/index.html)  

## Contributes
- Fork and clone at `ProjectName\Packages`
- Add an Scripting Define Symbol `DEBUG_ASSETLENS` at ProjectSettings/Player.
- Run `Tools/Reference_DEV/Add New Language` to create a new localization profile.
- Run `Tools/Reference_DEV/Update Language profiles` to add field after edit `Localize` class.
- Create PR.

![image](https://user-images.githubusercontent.com/79823287/131787910-1cc009e6-d483-4a87-afb0-a6ac31d3cf0d.png)  
![image](https://user-images.githubusercontent.com/79823287/131797772-078dda37-0917-4d98-abea-f09645e33a77.png)  
![image](https://user-images.githubusercontent.com/79823287/131797825-213d2927-db5a-47d0-a02d-bb87e0400b52.png)  


## 한국어 가이드
레퍼런스 하고 있는 에셋과, 이 에셋을 레퍼런스 하고 있는 에셋을 상호 저장함으로써 레퍼런스 링크를 생성합니다. 
파일시스템을 기반으로 바이너리로 각 guid 별로 쪼개어 저장합니다.  

> 0.0.x 버전으로 인덱싱 된 경우 0.1.x 버전에서 다시 인덱싱을 진행해야합니다.  
> 이후 버전에서는 버전을 통해 자동으로 업데이트 됩니다.

### 사용법
- `Tools/Reference/Index All Assets` 메뉴로 현재 에셋들을 모두 인덱싱합니다. (프로젝트 크기에 따라 시간 소요)
- 인스펙터에 선택한 에셋을 사용중인 다른 에셋의 수가 표기됩니다. (씬, 프리팹 포함)
- <kbd>ctrl</kbd> + <kbd>alt</kbd> + <kbd>R</kbd> 키를 눌러 콘솔창에 연결된 에셋을 프린트 할 수 있습니다. (누르면 선택됨)
- `Window/Reference View` 윈도우를 열어 선택된 오브젝트와 연결된 에셋을 확인할 수 있습니다.
- 에디터에서 파일을 삭제할 때, 연결된 에셋이 있으면 삭제여부를 확인하는 창이 나타납니다.

### 작동 원리
- `Library/ReferenceCache` 경로에 파일당 하나의 파일로서 저장됩니다.
- 바이너리 방식으로 저장합니다.
### Disclaimer
- 불안정한 작동으로 모든 레퍼런스 카운트를 확인하지 않을 수도 있습니다.
- 씬 오브젝트에 대해서는 정확히 작동하지 않습니다.
- Library/PackageCache에 저장되는 읽기전용 패키지는 ReferencedBy만 카운트됩니다.

