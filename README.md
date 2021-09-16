# Reference

레퍼런스 하고 있는 에셋과, 이 에셋을 레퍼런스 하고 있는 에셋을 상호 저장함으로써 레퍼런스 링크를 생성합니다. 파일시스템을 기반으로 바이너리로 각 guid 별로 쪼개어 저장합니다.  
This plugin generate both dependency map of the asset and which asset is referencing the asset.  

> 0.0.x 버전으로 인덱싱 된 경우 0.1.x 버전에서 다시 인덱싱을 진행해야합니다. 이후 버전에서는 버전을 통해 자동으로 업데이트 됩니다.

## Install
```json
{
    "dependencies": {
        "kr.seonghwan.reference": "0.1.3"
    }
}
```

```json
{
    "scopedRegistries": [
        {
            "name": "npm-seonghwan",
            "url": "https://registry.npmjs.org",
            "scopes": [
                "kr.seonghwan"
            ]
        }
    ]
}
```

## 사용법
- `Tools/Reference/Index All Assets` 메뉴로 현재 에셋들을 모두 인덱싱합니다. (프로젝트 크기에 따라 시간 소요)
- 인스펙터에 선택한 에셋을 사용중인 다른 에셋의 수가 표기됩니다. (씬, 프리팹 포함)
- <kbd>ctrl</kbd> + <kbd>alt</kbd> + <kbd>R</kbd> 키를 눌러 콘솔창에 연결된 에셋을 프린트 할 수 있습니다. (누르면 선택됨)
- `Window/Reference View` 윈도우를 열어 선택된 오브젝트와 연결된 에셋을 확인할 수 있습니다.
- 에디터에서 파일을 삭제할 때, 연결된 에셋이 있으면 삭제여부를 확인하는 창이 나타납니다.

## 작동 원리
- `Library/ReferenceCache` 경로에 파일당 하나의 파일로서 저장됩니다.
- 바이너리 방식으로 저장합니다.

## Contributes
- Fork and clone at `ProjectName\Packages`
- Add an Scripting Define Symbol `DEBUG_REFERENCE` at ProjectSettings/Player.
- Execute `Tools/Reference_DEV/Add New Language` to create a new localization profile.
- Execute `Tools/Reference_DEV/Update Language profiles` to add field after edit `Localize` class.

### TODO
- Safe Delete : 삭제 전 명시적으로 레퍼런스 검색을 한번 진행합니다.
- Replacement : 삭제 전 다른 에셋의 GUID로 대체합니다.
- Visualizer

### Disclaimer
- 불안정한 작동으로 모든 레퍼런스 카운트를 확인하지 않을 수도 있습니다.
- 씬 오브젝트에 대해서는 정확히 작동하지 않습니다.
- Library/PackageCache에 저장되는 읽기전용 패키지는 ReferencedBy만 카운트됩니다.

![image](https://user-images.githubusercontent.com/79823287/131787910-1cc009e6-d483-4a87-afb0-a6ac31d3cf0d.png)
![image](https://user-images.githubusercontent.com/79823287/131797772-078dda37-0917-4d98-abea-f09645e33a77.png)
![image](https://user-images.githubusercontent.com/79823287/131797825-213d2927-db5a-47d0-a02d-bb87e0400b52.png)
