using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

/// <summary>
/// The ShaderBlockReader class reads the blocks defined in a .myshader file.
/// It follows included files and reads them too, adding the parsed blocks from the included files to itself.
/// </summary>
public class ShaderBlockReader
{
    public class Block
    {
        public string name; // the block name
        public List<string> lines = new List<string>(); // the block as lines
        public string text = ""; // same as lines, but as a single string
    }

    public List<Block> blocks = new List<Block>(); // list of all blocks found in this and in included files
    public List<string> includes = new List<string>(); // list of included file paths

    /// <summary>
    /// Gets the content of the specified block. The name of block 'BEGIN_FOO' would be 'FOO'.
    /// </summary>
    /// <param name="blockName"></param>
    /// <returns>The content on success, an empty string otherwise.</returns>
    public string GetContent(string blockName)
    {
        for (var n = 0; n < blocks.Count; ++n)
        {
            if (string.Equals(blocks[n].name, blockName, System.StringComparison.OrdinalIgnoreCase))
                return blocks[n].text;
        }

        return "";
    }

    /// <summary>
    /// Collects the blocks from a "myshader" file and follows its included subshaders.
    /// </summary>
    public void Read(string assetPath)
    {
        ReadInternal(assetPath, 0);
    }

    void ReadInternal(string assetPath, int recursiveDepth)
    {
        var lines = File.ReadAllLines(assetPath);

        for (var n = 0; n < lines.Length; ++n)
        {
            var line = lines[n].Trim();

            if (string.IsNullOrEmpty(line)) continue; // Skip empty lines
            if (line.StartsWith("//")) continue; // Skip line comment

            // If it's an include block, make sure to follow and parse the included files.
            if (line.StartsWith("BEGIN_INCLUDES", System.StringComparison.OrdinalIgnoreCase))
            {
                ++n; // skip BEGIN_ line

                for (; n < lines.Length; ++n)
                {
                    line = lines[n];
                    if (string.IsNullOrEmpty(line)) continue; // Skip empty lines
                    if (line.StartsWith("//")) continue; // Skip line comment

                    if (line.StartsWith("END_INCLUDES", System.StringComparison.OrdinalIgnoreCase))
                        break;

                    // Check if a file exists at the specified path
                    var includePath = line.Replace("\"", "").Trim(); // Replace quotes
                    var ipath = includePath;
                    if (!File.Exists(ipath))
                    {
                        ipath = Path.GetDirectoryName(assetPath) + "/" + ipath;
                        if (File.Exists(ipath))
                            includePath = ipath;
                    }

                    if (!File.Exists(includePath))
                    {
                        Debug.LogErrorFormat("Included file '{0}' could not be found.", includePath);
                        continue;
                    }

                    // Catch if an include includes itself
                    if (recursiveDepth > 10)
                    {
                        Debug.LogErrorFormat("Included file '{0}' skipped due to recursion depth.", includePath);
                        continue;
                    }

                    // Track which files were included
                    includes.Add(includePath);

                    // Parse the included file
                    var parser = new ShaderBlockReader();
                    parser.ReadInternal(includePath, recursiveDepth++);

                    // Embed the parsed result into this parser
                    foreach (var b in parser.blocks)
                        AddOrMerge(b);

                    includes.AddRange(parser.includes);
                }

                continue;
            }

            // We are here if it's a generic block that does not need special handling
            if (line.StartsWith("BEGIN_", System.StringComparison.OrdinalIgnoreCase))
            {
                ++n; // skip BEGIN_ line

                var block = new Block();
                block.name = line.Substring("BEGIN_".Length).Trim();

                var endTag = "END_" + block.name;
                for (; n < lines.Length; ++n)
                {
                    line = lines[n];
                    if (line.StartsWith(endTag, System.StringComparison.OrdinalIgnoreCase))
                        break;

                    block.lines.Add(line);
                    block.text += line;
                    block.text += "\n";
                }

                AddOrMerge(block);

                continue;
            }
        }
    }

    // If a block with the same name exists, add the content of the newBlock to the existing block.
    // If no block exists with the name yet, just add it.
    void AddOrMerge(Block newBlock)
    {
        var existing = FindBlock(newBlock.name);
        if (existing == null)
        {
            blocks.Add(newBlock);
            return;
        }

        foreach (var line in newBlock.lines)
            existing.lines.Add(line);

        existing.text += newBlock.text;
    }

    Block FindBlock(string name)
    {
        for (var n = 0; n < blocks.Count; ++n)
        {
            if (string.Equals(blocks[n].name, name, System.StringComparison.OrdinalIgnoreCase))
                return blocks[n];
        }
        return null;
    }
}
