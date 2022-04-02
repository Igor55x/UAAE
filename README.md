# Unity Assets Advanced Editor
Unity .assets and AssetBundle editor

UAAE is an advanced editor for Unity .assets and AssetBundle files. It is based on DerPopo's UABE tool, but improves its functions.
UAAE isn't so `advanced` because it is still missing a lot of features and may have some bugs, so unless your game is unity 2019.3+, please use the original [UABE](https://github.com/DerPopo/UABE) for now. But it's better than nothing at all. Feel free to contribute.

#### Supported unity versions: 5 - 2021.2

## Features
* Open assets/bundle files
* Add/Remove assets
* Bundle file options Export/Import/Remove/Info for assets files
* Bundle file compression/decompression
* Data view
  * Monobehaviour deserialization
  * Tabs
    * Dump view tab
* Edit button
  * Asset editor (plugins)
* Asset list searching
  * By name
* Go to asset option
* Dependency loading
* Raw export/import
* Dump export/import in
  * TXT format
  * XML format
  * JSON format
* Save modified assets files and bundle file
* Command line support (beta)
* Plugin support
* Plugins
  * Texture plugin
  * TextAsset plugin
* Mod Maker

## Todo
This list may be incomplete!
* Advanced error checking (with logging)
* Dependency data view
  * Add dependency option
  * Edit dependency option
  * Remove dependency option
  * Save map to file
* [view asset] option in data view
* Tabs in data view
  * ~~Dump view tab~~
  * Raw view tab
  * Preview tab
* Container data view
  * Add container option
  * Edit container option
  * Remove container option
* ~~Monobehaviour deserialization in data view~~ (including il2cpp technology)
* Asset list searching
  * ~~By name~~
  * Binary content
  * Monobehaviour
  * Transform
* ~~Go to asset option~~
* Asset preview option
* Plugins
  * ~~Texture plugin~~ (including sprite editor)
  * ~~TextAsset plugin~~
  * Audio plugin (including Wwise)
  * MovieTexture plugin
  * Mesh plugin (FBX)
  * TerrainData plugin
  * Font plugin **(custom)** *
  * Prefab/Scene plugin **(custom)** *
* Mod Installer ~and Mod Maker~
* Cldb/Tpk editor
* Undo/Redo option
* Edit button
  * Dump editor tab
  * Raw editor tab
  * ~~Asset editor (plugins)~~
* Blank assets file/bundle creation
* Blank ResS/Resource file creation
* Blank MonoScript creation
* Progress bar form when opening multiple assets files
* Settings
* MacOS, Linux and Mobile support
* Implement the features that are going to be in new [UABE](https://community.7daystodie.com/topic/1871-unity-assets-bundle-extractor/?do=findComment&comment=357397)

## Download
* You can download [latest build](https://nightly.link/Igor55x/UAAE/workflows/dotnet-desktop/master/UAAE-Windows.zip) (newer but unstable) or [latest release](https://github.com/Igor55x/UAAE/releases) (older and missing bug fixes or new features)

## Build
* Install Visual Studio 2019 or newer
* Build the solution. Here's a [guide](https://docs.microsoft.com/en-us/visualstudio/ide/walkthrough-building-an-application?view=vs-2019) on how to do it

## Documentation
**[Here](https://github.com/Igor55x/UAAE/blob/master/DOCUMENTATION.md) you can find out how you can use the UnityTools library.**

## Libraries
* UnityTools has borrowed code from nesrak1's [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) project licensed under the [MIT license](https://github.com/nesrak1/AssetsTools.NET/blob/master/LICENSE)
* UnityTools for assets reading/writing which uses [detex](https://github.com/hglm/detex) for DXT decoding
* [ISPC](https://github.com/GameTechDev/ISPCTextureCompressor) for DXT encoding
* [crnlib](https://github.com/Unity-Technologies/crunch/tree/unity) (crunch) for crunch decompressing and compressing
* [PVRTexLib](https://developer.imaginationtech.com/downloads/) (PVRTexTool) for all other texture decoding and encoding

## Disclaimer
**None of the repo, the tool, nor the repo owner is affiliated with, or sponsored or authorized by, Unity Technologies or its affiliates.**

## License
This software is distributed under the MIT License