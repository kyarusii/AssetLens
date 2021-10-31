## 0.2.6 (2021-10-31)

### Fix

- clear invalid files
- rename and link assembly definition assets
- **reference-window**: nullReferenceException in rvw because of broken ref data

### Feat

- handle logic for unmanaged resources with fake path

## 0.2.5 (2021-10-15)

- Check whether it is initialized or not only once.
- Setting Inspector

## 0.2.4 (2021-10-07)

- bug fix : exclude directories and not exist files on first initialization.

## 0.2.3 (2021-09-28)

- missing object management wip
- bug fix : removing directory causes exception because of missing filtering in OnWillDeleteAsset callback.
- dialog localization about deleting asset.
- dirty cache update way

## 0.2.2 (2021-09-26)

- exclude scene asset

## 0.2.1 (2021-09-24)

- fixed an issue about empty path
- regular expression for addressable assets

## 0.2.0 (2021-09-23)

- Rename Package name to `Asset Lens` from `Reference`
- Change registry

## 0.1.6 (2021-09-23)

- Dialog Localization

## 0.1.5 (2021-09-17)
- support 2019.4

## 0.1.4 (2021-09-17)

- support custom packages.

## 0.1.3 (2021-09-16)

- fixed an issue that cache directory exception in library folder

## 0.1.2 (2021-09-16)

- fixed an issue that unity editor throws null reference exception drawing default asset
- refactored directory
- try-catch to catch unhandled exception in dev mode

## 0.1.1 (2021-09-16)

- fixed an issue that unity editor throws null reference exception with material sub editor drawing unity default resource.

## 0.1.0 (2021-09-16)

- change indexing convention to manage version
- localization
- setting menu

## 0.0.11 (2021-09-16)

- add an option about using UnityEditor.EditorUtility.CollectDependencies.
- fix an issue that unity editor throws null reference exception with an object has missing script.

## 0.0.10 (2021-09-14)

- add an option whether to collect dependencies in playmode or not.
- add an option whether to collect scene object or not.

## 0.0.9 (2021-09-11)

- store setting in setting object instead editor prefs

## 0.0.8 (2021-09-08)

- check empty path

## 0.0.7 (2021-09-04)

- do not create ref file if its directory

## 0.0.6 (2021-09-03)

- explicit search tool (sr4dev)
- run safer (try-catch)

## 0.0.5 (2021-09-02)

- lock
- scroll view

## 0.0.4 (2021-09-02)

- object context menu
- draw count in inspector

## 0.0.3 (2021-09-02)

- bug fix
- initialize help box

## 0.0.2 (2021-09-02)

- reference viewer
- 
## 0.0.1 (2021-09-01)

- basic features