using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

[ScriptedImporter(13, SurfaceShaderImporter.k_FileExtension)]
public class SurfaceShaderImporter : ScriptedImporter
{
    public const string k_FileExtension = "surfshader";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var text = BuildShader(ctx);
        Debug.Log(text);

        var shader = ShaderUtil.CreateShaderAsset(text);

        ctx.AddObjectToAsset("MainAsset", shader);
        ctx.SetMainObject(shader);
    }

    string BuildShader(AssetImportContext ctx)
    {
        try
        {
            // Read all blocks
            var blocks = new ShaderBlockReader();
            blocks.Read(ctx.assetPath);

            // Mark included files as dependencies
            foreach (var include in blocks.includes)
                ctx.DependsOnSourceAsset(include);

            // Build the actual shader text from the blocks
            var builder = new ShaderBuilder();
            var text = builder.Build(blocks, ctx.assetPath);

            return text;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        return k_ErrorShader;
    }

    [MenuItem("Assets/Create/Shader/Surface Shader v2", priority = 310)]
    static void CreateMenuItem()
    {
        // https://forum.unity.com/threads/how-to-implement-create-new-asset.759662/
        string directoryPath = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            directoryPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(directoryPath) && File.Exists(directoryPath))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
                break;
            }
        }
        directoryPath = directoryPath.Replace("\\", "/");
        if (directoryPath.Length > 0 && directoryPath[directoryPath.Length - 1] != '/')
            directoryPath += "/";
        if (string.IsNullOrEmpty(directoryPath))
            directoryPath = "Assets/";

        var fileName = string.Format("New Surface Shader v2.{0}", k_FileExtension);
        directoryPath = AssetDatabase.GenerateUniqueAssetPath(directoryPath + fileName);

        var content = SurfaceShaderUtility.LoadTemplate("SurfaceShader_New");
        ProjectWindowUtil.CreateAssetWithContent(directoryPath, content);
    }

    const string k_ErrorShader = @"
Shader ""Hidden/MYSHADER_ERROR""
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include ""UnityCG.cginc""
            struct appdata_t {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 0, 1, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}";
}
