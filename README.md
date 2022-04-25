# Unity Assets Advanced Editor
Unity .assets and AssetBundle editor

UAAE is an advanced editor for Unity .assets and AssetBundle files. It is based on DerPopo's UABE tool, but improves its functions.
UAAE was created to improve UABE features and support newer versions of Unity.
Feel free to contribute.

#### Supported unity versions: 5 - 2021.2

# [UABE has been updated! Go use that instead!](https://github.com/SeriousCache/UABE)
UABE hadn't been updated in a while, and at the time, hadn't been open source.
And now UABE is open source, updated and that's great news for those who have been waiting for a long time.
At the moment, UABE has more features than UAAE itself, so UAAE will adjust to it.

## Download
[Latest Nightly Build](https://nightly.link/Igor55x/UAAE/workflows/dotnet-desktop/master/UAAE-Windows.zip) | [Latest Release](https://github.com/Igor55x/UAAE/releases)

## Features
**[Here](https://github.com/Igor55x/UAAE/blob/master/FEATURES.md) you can see the UAAE implemented features.**

## Todos
**[Here](https://github.com/Igor55x/UAAE/blob/master/TODOS.md) you can see the UAAE todo list.**

## Documentation
**[Here](https://github.com/Igor55x/UAAE/blob/master/DOCUMENTATION.md) you can find out how you can use the UnityTools library.**

## Exporting assets
UAAE can export textures and asset dumps, but that's about it. If you're trying to dump anything else, try [AssetStudio](https://github.com/Perfare/AssetStudio) or [AssetRipper](https://github.com/ds5678/AssetRipper) (uTinyRipper), but these tools cannot import again.

## Scripting
If you're doing something that requires scripting such as dumping all of the fields from a MonoBehaviour, importing multiple text files or textures, etc. without interacting with the gui, try using UnityTools instead. UAAE can be a good way to figure out how the file is laid out, but the script can be written with UnityTools. If UnityTools is too complicated, you can also try [UnityPy](https://github.com/K0lb3/UnityPy) which has a simpler api with the cost of supporting less assets.

## MonoBehaviours
Many newer Unity games (especially non-pc games) are compiled with il2cpp which means that out of the box, UAAE cannot correctly deserialize any MonoBehaviour scripts. This is especially obvious when you export dump and import dump and find the file size much smaller. To fix this, dump il2cpp dummy dlls using a tool like [il2cppdumper](https://github.com/Perfare/Il2CppDumper) or [cpp2il](https://github.com/SamboyCoding/Cpp2IL). Then, create a folder called Managed in the same directory as the assets file/bundle file you want to open and copy all the dummy dlls generated with the tool you used into that folder.

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