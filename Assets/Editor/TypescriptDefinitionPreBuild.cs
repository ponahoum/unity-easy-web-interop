using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using Nahoum.UnityJSInterop.Editor;

public class TypescriptDefinitionPreBuild : IPreprocessBuildWithReport
{
    // Determines the order in which the build preprocessors are called.
    public int callbackOrder => 0;

    // This method is called before the build starts.
    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateAndSaveTypescriptDefinition();
    }

    private void GenerateAndSaveTypescriptDefinition()
    {
        // Generate the TypeScript definition using your custom generator
        string tsDefinition = TypescriptGenerator.GenerateTypescript();

        // Define the target file path (relative to the project root)
        string filePath = "Assets/WebGLTemplates/TestTemplate/UnityInstance.d.ts";

        try
        {
            // Write the generated TypeScript definition to the file
            File.WriteAllText(filePath, tsDefinition);
            Debug.Log($"TypeScript definition file generated at: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to write TypeScript definition to {filePath}: {ex.Message}");
        }

        // Refresh the AssetDatabase so that Unity picks up the changes immediately
        AssetDatabase.Refresh();
    }
}
