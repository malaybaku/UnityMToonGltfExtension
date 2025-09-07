using System.Linq;
using UniGLTF;
using UnityEditor.AssetImporters;
using UnityEngine;
using UniVRM10;
using VRMShaders;

namespace MToonGltf.Editor
{
    /// <summary>
    /// VRM10のMToonをサポートするGLTFのimporter
    /// UniGLTFのImporterと差し替える前提で作ってあり、Script Symbolベースで有効化される
    /// </summary>
#if MTOON_GLTF_ENABLE_GLB_IMPORTER
    [ScriptedImporter(1, new[] { "glb" })]
#else
    [ScriptedImporter(1, null, overrideExts: new[] { "glb" })]
#endif
    public class MToonGltfImporter : GltfScriptedImporterBase
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            ImportWithMToonSupport(this, ctx, m_reverseAxis.ToAxes(), m_renderPipeline);
        }

        // NOTE: 本質的にはGetMaterialGeneratorを差し替えてること以外UniGLTFの GltfScriptedImporterBase.Import と同じ
        private void ImportWithMToonSupport(
            ScriptedImporter scriptedImporter,
            AssetImportContext context,
            Axes reverseAxis,
            RenderPipelineTypes renderPipeline)
        {
            var extractedObjects = scriptedImporter.GetExternalObjectMap()
                .Where(x => x.Value != null)
                .ToDictionary(kv => new SubAssetKey(kv.Value.GetType(), kv.Key.name), kv => kv.Value);

            var materialGenerator = GetMaterialGenerator(renderPipeline);
            using var data = new AutoGltfFileParser(scriptedImporter.assetPath).Parse();
            using var loader = new ImporterContext(data, extractedObjects, materialGenerator: materialGenerator);

            foreach (var textureInfo in loader.TextureDescriptorGenerator.Get().GetEnumerable())
            {
                TextureImporterConfigurator.Configure(textureInfo, loader.TextureFactory.ExternalTextures);
            }

            loader.InvertAxis = reverseAxis;
            var loaded = loader.Load();
            loaded.ShowMeshes();

            loaded.TransferOwnership((k, o) =>
            {
                context.AddObjectToAsset(k.Name, o);
            });
            var root = loaded.Root;
            GameObject.DestroyImmediate(loaded);

            context.AddObjectToAsset(root.name, root);
            context.SetMainObject(root);
        }

        private static IMaterialDescriptorGenerator GetMaterialGenerator(RenderPipelineTypes renderPipeline)
        {
            return renderPipeline switch
            {
                RenderPipelineTypes.BuiltinRenderPipeline => new BuiltInVrm10MaterialDescriptorGenerator(),
                RenderPipelineTypes.UniversalRenderPipeline => new UrpVrm10MaterialDescriptorGenerator(),
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
