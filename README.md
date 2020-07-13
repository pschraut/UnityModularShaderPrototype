There is a discussion going on in the Unity forums about a new shader system ([link](https://forum.unity.com/threads/what-is-next-for-us-at-unity-with-scriptable-render-pipelines.924218/page-3#post-6060881)). Jason pitched a more modular approach for a Surface Shader 2.0 type system, and here is a proof of concept. 

There are currently two examples, one of a bare minimum shader, which just returns green. The other is an example of a simple "snow on top" effect being applied to a regular shader. In this example, all of the data, code, and properties for the snow shader is contained in a single .subshader file. 




