# Asset Lens

## About **Asset Lens**
**Asset Lens** is a dependency tracking plugin for UnityEditor that provides additional information such as the number of usage.

[<p align="center"><img src="https://discordapp.com/api/guilds/889046470655893574/widget.png?style=banner2"></p>](https://discord.gg/h9WPFRNFBY)

This plugin is based on pre-cached complementary guid map to trace which asset has dependencies to specific asset. 
The pain point that mainly considered with Unity is that when we delete an asset, we don't know which asset is using it. 

## Requirements
- All assets must be serialized as force-text option in ProjectSetting/Editor

### Compatibility
  
  
<a href="unityhub://2019.4.32f1/"><img src="https://img.shields.io/badge/unity-2019.4f_LTS-blue.svg?logo=unity"/></a>
<a href="unityhub://2020.3.21f1/"><img src="https://img.shields.io/badge/unity-2020.3f_LTS-blue.svg?logo=unity"/></a>
<a href="unityhub://2021.1.27f1/"><img src="https://img.shields.io/badge/unity-2021.1f_LTS-blue.svg?logo=unity"/></a>
<a href="unityhub://2021.2.0f1/"><img src="https://img.shields.io/badge/unity-2021.2f-brightgreen.svg?logo=unity"/></a>
<a href="unityhub://2022.1.0a13/"><img src="https://img.shields.io/badge/unity-2022.1 alpha-red.svg?logo=unity"/></a>

<a href="https://codecov.io/gh/seonghwan-dev/AssetLens">
<img src="https://codecov.io/gh/seonghwan-dev/AssetLens/branch/main/graph/badge.svg?token=7ODSTUTX1G"/>
</a>
<a href="https://openupm.com/packages/com.calci.assetlens/">
<img src="https://img.shields.io/npm/v/com.calci.assetlens?label=openupm&registry_uri=https://package.openupm.com"/>
</a>
<a href="https://badge.fury.io/js/com.calci.assetlens">
<img src="https://badge.fury.io/js/com.calci.assetlens.svg" alt="npm version"/>
</a>

## Installation
### Unity Package Manager (NPM)
[![NPM](https://nodei.co/npm/com.calci.assetlens.png?compact=true)](https://npmjs.org/package/com.calci.assetlens)

Replace stable version at version definition in json `x.x.x`  
example) `"com.calci.assetlens": "0.3.0"`  
```json
{
    "dependencies": {
        "com.calci.assetlens": "x.x.x"
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

## QuickStart
- Open Asset Lens Setting in `Edit/Project Settings...`  
- Hit the button `Initialize` to index all assets in project.  
- Configure the settings for convenience.  
- Select an asset you want to know which asset references it and run `Find References In Project` context menu.  

## Fundamentals
- Create a cache file per a asset file, see also [RefData.cs](Packages/com.calci.assetlens/Editor/Reference/Model/RefData.cs)
- Detect asset changes from `AssetPostprocessor`, see also [AssetLensPostprocessor.cs](Packages/com.calci.assetlens/Editor/Reference/Callback/ReferencePostprocessor.cs)
- Detect an attempt to delete an asset from `AssetModificationProcessor`, see also [AssetLensModification.cs](Packages/com.calci.assetlens/Editor/Reference/Callback/ReferenceModification.cs)

## Features
- Display asset usage count in inspector.
- Find References In Project

### Reference Viewer Window

|before initialize|after initialize|
|:---:|:---:|
|<img src="https://user-images.githubusercontent.com/79823287/134523257-28173dc7-4fd5-406e-8ac9-56b148debedb.png" width="460">|<img src="https://user-images.githubusercontent.com/79823287/134523437-166bf30b-ccdd-42ea-90ae-3084e0f013f6.png" width="460">|
|not available|available to trace dependencies|

### Inspector Indicator
Displays the number of other resources using the selected asset.

<p align="center">
<img src="https://user-images.githubusercontent.com/79823287/139777116-25ed937e-2f69-421a-91a8-4ae426a311e4.png" width="460">
</p>

- Details : Open Reference Viewer as EditorWindow instantly.  
- Refresh : Reserialized cached reference data asset.  
- GUID : Displays the guid of selected asset. onClick events will copy guid to your clipboard.  


## Roadmap
Not stable yet, but under development.

### Reference Viewer
- Multi Column Viewer (Sortable)
- Scene Object Reference Graph

### Safe Delete
- Alert before the asset that is used by other asset will be deleted.
- Replace reference during delete asset. (Reference Replacer)

### Inspector Lens
- Display how many assets are related to selected asset at the top of the inspector.

### Build Lens
- Find the assets will be included in build
- Asset bundle, Addressable, Linked assets with scenes in build setting, resources.

## Contributes
- Current Editor Version : `2021.2.0f1`    
- Fork and clone repository.  
- Edit sources and commit with conventional commits (prefer Commitizen)  
- Add unit test codes for new feature (Optional)
- Create PR.  

### Developer mode
Select menu in `Help/Asset Lens/Enter Debug Mode` or Add an scripting define symbol `DEBUG_ASSETLENS` at ProjectSettings/Player.

### Edit Languages
- Run `Tools/Asset Lens_DEV/Add New Language` to create a new localization profile.
- Run `Tools/Asset Lens_DEV/Update Language profiles` to add field after edit `Localize` class.

### Requirements
- commitizen - conventional commit log to generate changelog
