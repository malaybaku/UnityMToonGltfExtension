# UnityMToonGltfExtension

Author: Baxter, 獏星(ばくすたー)

A package to support export/import glTF with MToon shader data in Unity Editor.

## Prerequisite

- Unity 2022.3 or later
- [UniVRM](https://github.com/vrm-c/UniVRM) v0.121.0 or later

Note that UniVRM update might lead error, since this package heavily depends on UniVRM implementation.

## Install

In Unity Package Manager > `Install Package from git URL`, specify followin URL.

```
https://github.com/malaybaku/UnityMToonGltfExtension.git?path=/Package#v0.1.0
```

## Limitation

Import process only supports `.glb` and `.gltf` is unsupported.

Importer function is limited. I recommend to use import feature only to check the exported `.glb` contains MToon material data.

## Usage

### Export `.glb` or `.gltf` File

> Menu Bar -> `MToonGltf` -> `Export MToon glTF...` 

Export the selected object in which `MToon` materials are used.

### Import `.glb` file

> Menu Bar -> `MToonGltf` -> `Use MToon glTF Importer`

Check the menu item to enable `.glb` importer supporting MToon material data.

After checking the menu item, you can drag & drop `.glb` file into project view to import data.

Note that this menu item internally changes script define symbols. Uncheck the item to reset the settings.

## Note: How to use MToon-included `.glb` in the app

In the app with UniVRM, you can read MToon data in `.glb` by specifying `materialGenerator` in import process. 

Following code is an example for Built-in Render Pipeline.

```csharp
private void LoadGltf(GltfData data)
{
    var context = new UniGLTF.ImporterContext(
        data,
        materialGenerator: new UniVRM10.BuiltInVrm10MaterialDescriptorGenerator()
        );
    var instance = context.Load();
    //...
}
```
