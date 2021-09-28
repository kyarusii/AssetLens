# CHANGE LOG

### 0.0.1

- basic features

### 0.0.2

- reference viewer

### 0.0.3

- bug fix
- initialize help box

### 0.0.4

- object context menu
- draw count in inspector

### 0.0.5

- lock
- scroll view

### 0.0.6

- explicit search tool (sr4dev)
- run safer (try-catch)

### 0.0.7

- do not create ref file if its directory

### 0.0.8

- check empty path

### 0.0.9

- store setting in setting object instead editor prefs

### 0.0.10

- add an option whether to collect dependencies in playmode or not.
- add an option whether to collect scene object or not.

### 0.0.11

- add an option about using UnityEditor.EditorUtility.CollectDependencies.
- fix an issue that unity editor throws null reference exception with an object has missing script.

### 0.1.0

- change indexing convention to manage version
- localization
- setting menu

### 0.1.1

- fixed an issue that unity editor throws null reference exception with material sub editor drawing unity default resource.

### 0.1.2

- fixed an issue that unity editor throws null reference exception drawing default asset
- refactored directory
- try-catch to catch unhandled exception in dev mode 

### 0.1.3

- fixed an issue that cache directory exception in library folder

### 0.1.4

- support custom packages.

### 0.1.5
- support 2019.4

### 0.1.6

- Dialog Localization

### 0.2.0

- Rename Package name to `Asset Lens` from `Reference`
- Change registry

### 0.2.1

- fixed an issue about empty path
- regular expression for addressable assets

### 0.2.2

- exclude scene asset

### 0.2.3

- missing object management wip
- bug fix : removing directory causes exception because of missing filtering in OnWillDeleteAsset callback.
- dialog localization about deleting asset.
- dirty cache update way