using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Registers user created shader, which is required in order for those
/// shader to appear in the material shader dropdown list.
/// </summary>
class SurfaceShaderAssetPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        RegisterShaders(importedAssets);
    }

    static void RegisterShaders(string[] paths)
    {
        foreach (var assetPath in paths)
        {
            if (!assetPath.EndsWith(SurfaceShaderImporter.k_FileExtension, StringComparison.InvariantCultureIgnoreCase))
                continue;

            var mainObj = AssetDatabase.LoadMainAssetAtPath(assetPath) as Shader;
            if (mainObj != null)
                ShaderUtil.RegisterShader(mainObj);

            foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath))
            {
                if (obj is Shader)
                    ShaderUtil.RegisterShader((Shader)obj);
            }
        }
    }
}
