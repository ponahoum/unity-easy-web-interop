using System;
using UnityEditor;
using UnityEngine;

namespace Nahoum.UnityJSInterop.Editor
{

    public static class SetupWASMArgs
    {
        const string CFLAGS = "-s ALLOW_TABLE_GROWTH -s EXPORTED_RUNTIME_METHODS=['ccall,cwrap,addFunction,addRunDependency,removeRunDependency,FS_createDataFile,FS_createPath,UTF8ToString']";

        [InitializeOnLoadMethod]
        public static void SetupWASMargs()
        {
            PlayerSettings.WebGL.emscriptenArgs = CFLAGS;
        }
    }
}