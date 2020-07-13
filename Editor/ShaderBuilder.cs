using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The ShaderBuilder takes the input blocks and builds a shader text file from it.
/// </summary>
public class ShaderBuilder
{
    public string Build(ShaderBlockReader parser)
    {
        // unlit for now.. Likely want to have some way of specifying templates too..

        var text = LoadTemplate("SurfaceShader_Template_Unlit");

        text = text.Replace("%PROPERTIES%", parser.GetContent("PROPERTIES"));
        text = text.Replace("%CODE%", parser.GetContent("CODE"));
        text = text.Replace("%DEFINES%", parser.GetContent("DEFINES"));
        text = text.Replace("%CBUFFER%", parser.GetContent("CBUFFER"));


        return text;
    }

    string LoadTemplate(string fileName)
    {
        // We need to find the surface shader 2 editor folder.
        // To do this, we lookup the "SurfaceShaderImporter.cs" path from its guid.
        var importerPath = AssetDatabase.GUIDToAssetPath("b406cae38b5f8db4b8b570888ede71b7");
        var importerFolder = System.IO.Path.GetDirectoryName(importerPath);
        var templatePath = System.IO.Path.Combine(importerFolder, "Templates/" + fileName + ".txt");

        // Use File.ReadAllText rather than Resources.Load, because when dropping the repo
        // in Unity for the first time, when Unity imports the shader examples, it didn't import
        // the templates files and thus can't load then. Therefore we use regular file io instead.
        var content = System.IO.File.ReadAllText(templatePath);
        return content;
    }

}
