using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The ShaderBuilder takes the input blocks and builds a shader text file from it.
/// </summary>
public class ShaderBuilder
{
    public string Build(ShaderBlockReader parser, string assetPath)
    {
        // unlit for now.. Likely want to have some way of specifying templates too..

        var text = SurfaceShaderUtility.LoadTemplate("SurfaceShader_Template_Unlit");

        // Try to get the shader name. If no shadername block exists, use the filename instead.
        var shaderName = parser.GetContent("SHADERNAME").Trim().Replace("\"", "");
        if (string.IsNullOrEmpty(shaderName))
            shaderName = "Surface Shader v2/" + System.IO.Path.GetFileNameWithoutExtension(assetPath);
        text = text.Replace("%SHADERNAME%", shaderName);

        text = text.Replace("%PROPERTIES%", parser.GetContent("PROPERTIES"));
        text = text.Replace("%CODE%", parser.GetContent("CODE"));
        text = text.Replace("%DEFINES%", parser.GetContent("DEFINES"));
        text = text.Replace("%CBUFFER%", parser.GetContent("CBUFFER"));

        return text;
    }
}
