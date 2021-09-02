# Reference

레퍼런스 하고 있는 에셋과, 이 에셋을 레퍼런스 하고 있는 에셋을 상호 저장함으로써 레퍼런스 링크를 생성합니다. 파일시스템을 기반으로 바이너리로 각 guid 별로 쪼개어 저장합니다. 

![image](https://user-images.githubusercontent.com/79823287/131787910-1cc009e6-d483-4a87-afb0-a6ac31d3cf0d.png)
![image](https://user-images.githubusercontent.com/79823287/131797772-078dda37-0917-4d98-abea-f09645e33a77.png)
![image](https://user-images.githubusercontent.com/79823287/131797825-213d2927-db5a-47d0-a02d-bb87e0400b52.png)

## Install

```json
{
    "dependencies": {
        "kr.seonghwan.reference": "0.0.4"
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

## 기타
- `Library/ReferenceCache` 경로에 파일로서 저장됩니다.  
- 최초 실행시 `Tools/Referecne/Index All Assets` 메뉴를 실행해야 합니다.  
- 이후부터는 에셋을 삭제하려고 할 때, 링크가 있는 경우 재차 물어보게 됩니다.  
- `Tools/Reference/Log Selection` 메뉴로 현재 선택한 오브젝트의 레퍼런스 정보를 프린트 할 수 있습니다. 
  - 단축키 <kbd>ctrl</kbd> + <kbd>alt</kbd> + <kbd>R</kbd>  
- 프린트되는 로그를 선택하면 프로젝트 뷰에서 해당 에셋을 하이라이트해줍니다.  
- `Window/Reference View` 윈도우를 열면 현재 선택된 오브젝트의 레퍼런스 데이터를 보여줍니다.


### TODO
- Safe Delete : 삭제 전 명시적으로 레퍼런스 검색을 한번 진행합니다.  
- Replacement : 삭제 전 다른 에셋의 GUID로 대체합니다.  
- Visualizer
