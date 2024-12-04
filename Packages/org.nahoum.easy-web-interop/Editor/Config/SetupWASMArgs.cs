using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Nahoum.UnityJSInterop.Editor
{

    public static class SetupWASMArgs
    {
        const string CFLAGS = "-s ALLOW_TABLE_GROWTH -s EXPORTED_RUNTIME_METHODS=['addFunction,%exportedMethods%']";

        [InitializeOnLoadMethod]
        public static void SetupWASMargs()
        {
            string methodsArray = string.Join(",", GetExportedMethods());
            string emscriptenArgs = CFLAGS.Replace("%exportedMethods%", methodsArray);
            PlayerSettings.WebGL.emscriptenArgs = emscriptenArgs;
            Debug.Log("Emscripten Args: " + emscriptenArgs);
        }

        /// <summary>
        /// Gets all methods that are exported to webgl
        /// Take unity's webgl template files and extract all Module.X() methods, because there is no way to get this information from the unity API
        /// </summary>
        private static HashSet<string> GetExportedMethods()
        {

            // List of methods to export
            HashSet<string> methods = new HashSet<string>();

            // Path to WebGL template files
            string webGLPath = Path.Combine(EditorApplication.applicationContentsPath,
                "PlaybackEngines/WebGLSupport/BuildTools/");

            // Analyze framework JS files
            string[] jsFiles = Directory.GetFiles(webGLPath, "*.js", SearchOption.AllDirectories);
            foreach (string file in jsFiles)
            {
                string content = File.ReadAllText(file);
                // Look for Module.X() patterns
                var matches = Regex.Matches(content, @"Module\.(\w+)\s*\(");
                foreach (Match match in matches)
                {
                    methods.Add(match.Groups[1].Value);
                }
            }

            return methods;
        }
    }
}