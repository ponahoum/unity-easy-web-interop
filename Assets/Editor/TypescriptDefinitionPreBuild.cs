using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;
using Nahoum.UnityJSInterop.Editor;
using Debug = UnityEngine.Debug;

public class TypescriptDefinitionPreBuild : IPreprocessBuildWithReport
{
    // Determines the order in which the build preprocessors are called.
    public int callbackOrder => 0;

    // Read-only variable that holds the base folder for TypeScript files.
    // This is relative to the project root.
    private static readonly string TS_BASE_FOLDER = "RuntimeTypescriptTests";

    // Get the absolute path of the TypeScript base folder.
    public static string GetTsBasePath() => Path.Combine(Application.dataPath,"..", TS_BASE_FOLDER);

    // This method is called before the build starts.
    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateAndSaveTypescriptDefinition();
        BuildTypescriptTests();
    }


    // Build tests by running 'npm install' and 'npm run build' in the TypeScript base folder.
    private void BuildTypescriptTests()
    {
        // Get the absolute path of the directory by combining the project root with the base folder.
        string npmDirectory = GetTsBasePath();

        try
        {
            Debug.Log($"Running 'npm install' in {npmDirectory}...");
            RunCommand("npm install", npmDirectory);

            Debug.Log($"Running 'npm run build' in {npmDirectory}...");
            RunCommand("npm run build", npmDirectory);

            // Refresh the asset database after running npm commands
            AssetDatabase.Refresh();
            Debug.Log("TypeScript tests built successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error building TypeScript tests: " + ex.Message);
        }
    }


    /// <summary>
    /// Executes a command using cmd.
    /// </summary>
    /// <param name="commandToRun">The command to execute (for example, "npm run init")</param>
    /// <param name="workingDirectory">The working directory in which to run the command.
    /// If null or empty, defaults to the root of the current drive.</param>
    /// <returns>The output produced by the command.</returns>
    private static void RunCommand(string commandToRun, string workingDirectory)
    {
        if (string.IsNullOrEmpty(workingDirectory))
            throw new ArgumentException("Working directory cannot be null or empty.");

        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "cmd.exe",
            Arguments = $"/C{commandToRun}",
           WorkingDirectory = workingDirectory
        };
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }


    private void GenerateAndSaveTypescriptDefinition()
    {
        // Generate the TypeScript definition using your custom generator.
        string tsDefinition = TypescriptGenerator.GenerateTypescript();

        // Define the target file path using the read-only base folder variable.
        string filePath = Path.Combine(GetTsBasePath(), "UnityInstance.d.ts");

        try
        {
            File.WriteAllText(filePath, tsDefinition);
            Debug.Log($"TypeScript definition file generated at: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write TypeScript definition to {filePath}: {ex.Message}");
        }
    }
}
