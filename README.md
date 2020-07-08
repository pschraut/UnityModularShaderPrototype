There is a discussion going on in the Unity forums about a new shader system [link](https://forum.unity.com/threads/what-is-next-for-us-at-unity-with-scriptable-render-pipelines.924218/page-3#post-6060881). Jason Booth came up with an idea to provide BEGN/END blocks in a shader file that describe certain parts of a shader.

This repository contains a basic shader file importer, that processes those blocks. Perhaps we can use this to farther prototype this idea, to see how well it works and perhaps pitch it to Unity Technologies.

[![](http://img.youtube.com/vi/O1bnX3LZn8o/0.jpg)](http://www.youtube.com/watch?v=O1bnX3LZn8o "")


The repository contains the shader import pipeline (MyShaderImporter.cs), a block parser (ShaderBlockReader.cs) which reads BEGIN/END blocks, as well as a pretty useless shader builder (ShaderBuilder.cs).

The shader builder uses the input from the block reader to build a shader text file. The shader text file is then turned into an Unity shader by the shader importer (MyShaderImporter.cs).

The shader builder is pretty much the black box for me. So perhaps Jason can jump in here to get his idea further.
