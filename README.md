# UnityMToonGltfExtension

Author: Baxter, 獏星(ばくすたー)

A package for Unity export and import glTF data with MToon shader.

Unity Editor上で、MToonシェーダーを定義した glTF のエクスポート/インポートを補助するパッケージです。

## Prerequisite

- Unity 2022.3 or later
- [UniVRM](https://github.com/vrm-c/UniVRM) v0.121.0 or later

Note that UniVRM update might lead error, since this package heavily depends on UniVRM implementation.

注意: 本パッケージはUniVRMの実装に強く依存しているため、UniVRMの更新によって本パッケージが動作しなくなる可能性があります。

## Install

Unity Package Manager > `Install Package from git URL`

```
https://github.com/malaybaku/UnityMToonGltfExtension.git?path=/Package#v0.1.0
```

## Limitation

Import process only supports `.glb` and `.gltf` is unsupported.
Importer function is limited.
I recommend to use importer feature only to check the exported `.glb` contains MToon material data.

===

Unity Editorでのインポート処理では `.glb` のみをサポートしており、 `.gltf` には未対応です。
インポート処理のサポート範囲は限定的です。
インポート機能は、本パッケージでエクスポートした `.glb` にMToonのデータが載っていることを簡易チェックする目的でのみ使うことを推奨しています。

## Usage

### Export `.glb` or `.gltf` with MToon

> Menu Bar -> `MToonGltf` -> `Export MToon glTF...` 

Export the selected object in which `MToon` materials are used.

上記の項目で開いたウィンドウに対し、`MToon` のマテリアルを使用したprefab等を選択してExportします。

### Import `.glb` file

> Menu Bar -> `MToonGltf` -> `Use MToon glTF Importer`

Check this menu to enable `.glb` importer which supports MToon material. 
After checking the menu item, you can drag & drop `.glb` file into project view to import data.
Note that this menu item internally changes script define symbols. Uncheck the item to reset the settings.

上記のメニュー項目をオンにすることで、MToon対応の `.glb` ファイルのインポート機能が有効になります。
有効化ののち、プロジェクトビュー上に `.glb` ファイルをドラッグ & ドロップすることでMToon情報つきで `.glb` ファイルがインポートされます。
なお、このメニュー項目を実行すると内部的にスクリプトシンボルの定義が変更されます。
変更内容をリセットする場合、メニュー項目をオフに戻します。


## Note: How to use in the app

In the app with UniVRM, you can read MToon data in glb by specifying `materialGenerator` in import process. Following code is an example for Built-in Render Pipeline.

UniVRMを使用しているアプリケーションで実行時にMToon情報つきのglTFファイルを読み込む場合、`materialGenerator`としてVRM10相当のGeneratorを指定しながらImportを行うとMToon情報の読み出しに簡易的に対応できます。下記はBuilt-in Render Pipeline向けの実装例です。

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
