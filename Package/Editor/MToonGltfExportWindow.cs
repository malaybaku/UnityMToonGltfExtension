using System.IO;
using UniGLTF;
using UnityEditor;
using UnityEngine;
using VRMShaders;

namespace MToonGltf.Editor
{
    /// <summary>
    /// MToonシェーダー情報つきでGLTFをエクスポートする機能のあるウィンドウ
    /// </summary>
    public class MToonGltfExportWindow : GltfExportWindow
    {
        [MenuItem("MToonGltf/Export Gltf with MToon...", priority = 1)]
        private static void ExportGameObjectToGltf() => ShowWindow();

        private static void ShowWindow()
        {
            var window = (MToonGltfExportWindow)GetWindow(typeof(MToonGltfExportWindow));
            window.titleContent = new GUIContent("Export Gltf with MToon...");
            window.Show();
        }

        protected override void ExportPath(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            var isGlb = false;
            switch (ext)
            {
                case ".glb": isGlb = true; break;
                case ".gltf": isGlb = false; break;
                default: throw new System.Exception();
            }

            var progress = 0;
            EditorUtility.DisplayProgressBar("export gltf", path, progress);
            try
            {
                var data = new ExportingGltfData();
                using (var exporter = new MToonGltfExporter(data, Settings,
                    progress: new EditorProgress(),
                    animationExporter: new EditorAnimationExporter()))
                {
                    exporter.Prepare(State.ExportRoot);
                    exporter.Export(new EditorTextureSerializer());
                }

                if (isGlb)
                {
                    var bytes = data.ToGlbBytes();
                    File.WriteAllBytes(path, bytes);
                }
                else
                {
                    var (json, buffer0) = data.ToGltf(path);

                    {
                        // write JSON without BOM
                        var encoding = new System.Text.UTF8Encoding(false);
                        File.WriteAllText(path, json, encoding);
                    }

                    {
                        // write to buffer0 local folder
                        var dir = Path.GetDirectoryName(path);
                        var bufferPath = Path.Combine(dir, buffer0.uri);
                        File.WriteAllBytes(bufferPath, data.BinBytes.ToArray());
                    }
                }

                if (path.StartsWithUnityAssetPath())
                {
                    AssetDatabase.ImportAsset(path.ToUnityRelativePath());
                    AssetDatabase.Refresh();
                }

            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
