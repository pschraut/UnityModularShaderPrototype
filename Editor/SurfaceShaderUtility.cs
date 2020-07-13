using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SurfaceShaderUtility
{
    /// <summary>
    /// Gets the asset path to the directory where template files are stored.
    /// </summary>
    public static string templateDirectory
    {
        get
        {
            var path = AssetDatabase.GUIDToAssetPath("a9cd9497c0a634703a63a54915921422"); // GUID of "Templates" directory
            if (path.Length > 0 && (path[path.Length - 1] == '/' || path[path.Length - 1] == '\\'))
                return path.Substring(0, path.Length - 1);

            return path;
        }
    }

    /// <summary>
    /// Loads the content of the specified template file.
    /// </summary>
    public static string LoadTemplate(string fileNameWithoutExtension)
    {
        var filePath = templateDirectory + "/" + fileNameWithoutExtension + ".txt";

        // Use File.ReadAllText rather than Resources.Load, because when dropping the repo
        // in Unity for the first time, when Unity imports the shader examples, it didn't import
        // the templates files yet and thus can't load then. Therefore we use regular file io instead.
        var content = System.IO.File.ReadAllText(filePath);
        return content;
    }
}
