using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace MToonGltf.Editor
{
    /// <summary>
    /// MToonつきGLB用のEditor向けのScriptedImporterのon/offを切り替える処理を実装しているクラス
    /// </summary>
    public static class MToonGltfImporterMenu
    {
        private const string MenuPath = "MToonGltf/Use MToon GLTF Importer";

        private const string DisableDefaultGlbImporterSymbol = "UNIGLTF_DISABLE_DEFAULT_GLB_IMPORTER";
        private const string EnableMToonGltfImporterSymbol = "MTOON_GLTF_ENABLE_GLB_IMPORTER";

        private const bool EnableMToonGltfImporterSymbolDefined =
#if MTOON_GLTF_ENABLE_GLB_IMPORTER
            true;
#else
            false;
#endif

        [MenuItem(MenuPath, priority = 2)]
        private static void ToggleMToonGltfImporterEnabled()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);
            var defineList = defines.ToList();

            if (EnableMToonGltfImporterSymbolDefined)
            {
                // Symbolを両方とも削除: 既定のImporterを復活させる
                defineList.RemoveAll(v => v == EnableMToonGltfImporterSymbol);
                defineList.RemoveAll(v => v == DisableDefaultGlbImporterSymbol);
            }
            else
            {
                // Symbolを両方とも追加: MToonGltfImporterが動作するようにする
                defineList.Add(EnableMToonGltfImporterSymbol);
                if (!defineList.Contains(DisableDefaultGlbImporterSymbol))
                {
                    defineList.Add(DisableDefaultGlbImporterSymbol);
                }
            }
            
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineList.ToArray());
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleDefineValidate()
        {
            Menu.SetChecked(MenuPath, EnableMToonGltfImporterSymbolDefined);
            return true;
        }
    }
}
