using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ShaderBuilder takes the input blocks and builds a shader text file from it.
/// </summary>
public class ShaderBuilder
{
    public string Build(ShaderBlockReader parser)
    {
        var text = k_ShaderSkeleton;

        text = text.Replace("%PROPERTIES%", parser.GetContent("PROPERTIES"));
        text = text.Replace("%FRAG%", parser.GetContent("FRAG"));

        return text;
    }


    const string k_ShaderSkeleton = @"
Shader ""%SHADERNAME%""
{
    Properties
    {
        %PROPERTIES%
    }

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
                %FRAG%
                //return fixed4(0,1,1,1);
            }
            ENDCG
        }
    }
    Fallback Off
}";
}
