using System;
using UniGLTF;
using UniVRM10;

namespace MToonGltf.Editor
{
    /// <summary>
    /// VRM10で使われるシェーダーをExport対象にできる gltfExporter
    /// </summary>
    public class MToonGltfExporter : gltfExporter
    {
        public MToonGltfExporter(
            ExportingGltfData data,
            GltfExportSettings settings,
            IProgress<ExportProgress> progress = null,
            IAnimationExporter animationExporter = null)
            : base(data, settings, progress, animationExporter)
        {
        }

        protected override IMaterialExporter CreateMaterialExporter() 
            => new BuiltInVrm10MaterialExporter();
    }
}
