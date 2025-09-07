using System.Linq;
using UniGLTF;
using UnityEditor.AssetImporters;
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
    public class MToonGlbScriptedImporter : GltfScriptedImporterBase
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            // 処理そのものはUniGLTFの GltfScriptedImporterBase.Import とほぼ等価だが、
            // MaterialGeneratorの取得部分だけが差し替わっている
            var extractedObjects = GetExternalObjectMap()
                .Where(x => x.Value != null)
                .ToDictionary(kv => new SubAssetKey(kv.Value.GetType(), kv.Key.name), kv => kv.Value);

            using var data = new AutoGltfFileParser(assetPath).Parse();
            using var loader = new ImporterContext(
                data,
                extractedObjects,
                materialGenerator: GetMaterialGenerator(m_renderPipeline));

            foreach (var textureInfo in loader.TextureDescriptorGenerator.Get().GetEnumerable())
            {
                TextureImporterConfigurator.Configure(textureInfo, loader.TextureFactory.ExternalTextures);
            }

            loader.InvertAxis = m_reverseAxis.ToAxes();
            var loaded = loader.Load();
            loaded.ShowMeshes();

            loaded.TransferOwnership((k, o) =>
            {
                context.AddObjectToAsset(k.Name, o);
            });
            var root = loaded.Root;
            DestroyImmediate(loaded);

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
