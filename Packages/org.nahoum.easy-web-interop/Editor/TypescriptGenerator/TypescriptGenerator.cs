using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nahoum.UnityJSInterop.Editor
{
    public class TypescriptGenerator
    {
        // A GUID generator
        static HashSet<Type> additionalTypesToGenerate = new HashSet<Type>(){
          typeof(System.String),
            typeof(System.Double),
            typeof(System.Int32),
            typeof(System.Byte),
            typeof(System.Boolean),
            typeof(System.Single),
            typeof(System.Int64),
            typeof(Action),
            typeof(string[]),
            typeof(int[]),
            typeof(bool[]),
            typeof(double[]),
            typeof(byte[]),
            typeof(float[]),
        };

        static readonly string mainTemplatePath = "Packages/org.nahoum.easy-web-interop/Editor/TypescriptGenerator/Templates/MainTemplate.ts";

        /// <summary>
        /// A menu item to generate a typescript file describing all exposed methods
        /// One may also call GenerateTypescript() directly to get the typescript type string
        /// </summary>
        [MenuItem("UnityJSInterop/Generate Typescript")]
        public static void RequestGenerateTs()
        {

            // Prompt for path on disk for a .ts file
            string path = EditorUtility.SaveFilePanel("Save Typescript file", "", "UnityJSInterop.ts", "ts");

            if (string.IsNullOrEmpty(path))
                return;

            // Generate TS
            string ts = GenerateTypescript();

            // Write to file
            System.IO.File.WriteAllText(path, ts);
        }

        /// <summary>
        /// Generate a typescript file describing all exposed methods
        /// Save it in a .ts file of your choice then assign it directly to unity module in your js project
        /// </summary>
        public static string GenerateTypescript()
        {
            // For each type, additions the parameters and return types of each exposed methodsmethods
            Dictionary<NamespaceDescriptor, HashSet<Type>> typesByNamespace = TypescriptGenerationUtilities.GetExposedTypesByNamespace(excludeTestsAssemblies: true, additionalTypesToGenerate.ToArray());
            HashSet<Type> allTypesExported = TypescriptGenerationUtilities.GetExposedTypesFlatenned(excludeTestsAssemblies: true);

            // Keep track of generated types to avoid duplicates
            // If we have duplicates, it could be due to a type with the ame name but in different assemblies
            // As assemblies are not part of the typescript type, we need to throw
            HashSet<string> GeneratedTypename = new HashSet<string>();

            // Output ts for each namespace
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<NamespaceDescriptor, HashSet<Type>> namespaceEntry in typesByNamespace)
            {
                NamespaceDescriptor targetNamespace = namespaceEntry.Key;
                HashSet<Type> types = namespaceEntry.Value;
                if (targetNamespace.HasNamespace)
                    sb.AppendLine("export namespace " + targetNamespace.name + " {");

                foreach (Type type in types)
                {
                    // Get the exposed methods for this type
                    TypescriptGenerationUtilities.GetExposedMethodsSorted(type, out Dictionary<MethodInfo, ExposeWebAttribute> staticMethods, out Dictionary<MethodInfo, ExposeWebAttribute> instanceMethods);

                    string typeName = GenerateTsNameFromType(type, targetNamespace);

                    // Sanity check (for issues between same named classes in different assemblies)
                    if (GeneratedTypename.Contains(typeName))
                        throw new Exception($"Duplicate type name {typeName} found in typescript generation. This is likely due to a type with the same name but in different assemblies. Please rename the type in C# to avoid conflicts.");
                    GeneratedTypename.Add(typeName);

                    bool isStaticType = type.IsAbstract && type.IsSealed;
                    bool hasStaticMethods = staticMethods.Count > 0;

                    // If the type has static methods, we separate the static methods from the instance methods
                    if (hasStaticMethods)
                    {
                        sb.AppendLine("export type " + typeName + "_static = {");

                        // Add key to fully differentiate between types in typescript (name is not enough)
                        sb.AppendLine($"fullTypeName_{GetGuid()}: '{type.FullName}';");
                        sb.AppendLine($"assembly_{GetGuid()}: '{type.Assembly.FullName}';");

                        foreach (var method in staticMethods)
                        {
                            var methodInfo = method.Key;
                            var signature = GenerateSignatureFromMethod(methodInfo, targetNamespace);
                            sb.AppendLine(signature + ";");
                        }
                        sb.AppendLine("}");
                    }

                    // If the type is not static, we need to generate a type for it, otherwise the static type generated above is enough
                    if (!isStaticType)
                    {
                        sb.AppendLine($"export type {typeName}" + " = {");

                        // Add key to fully differentiate between types in typescript (name is not enough)
                        sb.AppendLine($"fullTypeName_{GetGuid()}: '{type.FullName}';");
                        sb.AppendLine($"assembly_{GetGuid()}: '{type.Assembly.FullName}';");

                        // For all objects, we may gather the value of the serializer instance object
                        if (ObjectSerializer.TryGetSerializer(type, out IJsJsonSerializer serializer) && serializer.CanSerialize(type, out ITsTypeDescriptor tsDescriptor))
                            sb.AppendLine($"value: {tsDescriptor.GetTsTypeDefinition(type)};");
                        else
                            sb.AppendLine($"value: any;");

                        // Get all exposed methods within this type
                        foreach (var method in instanceMethods)
                        {
                            var methodInfo = method.Key;
                            var signature = GenerateSignatureFromMethod(methodInfo, targetNamespace);
                            sb.AppendLine(signature + ";");
                        }

                        sb.AppendLine("}");

                        // If it has static methods, add & typeName_static at the end to extend the static method signature
                        if (hasStaticMethods)
                        {
                            sb.Append($"& {typeName}_static");
                        }

                        // Also add the types from which this type inherits
                        HashSet<Type> inheritingTypes = TypescriptGenerationUtilities.GetAllInheritingTypes(type);
                        foreach (var inheritingType in inheritingTypes)
                        {
                            // If the inheriting type is not exposed, skip because in any case it will be totally useless
                            if (!allTypesExported.Contains(inheritingType))
                                continue;

                            string inheritingTypeName = GenerateTsNameFromType(inheritingType, targetNamespace);
                            sb.Append($"& {inheritingTypeName}");
                        }

                        sb.Append(";");
                    }

                    // Get all classes and interface from which this type inherits
                }

                if (targetNamespace.HasNamespace)
                    sb.AppendLine("}");
            }

            var result = sb.ToString();
            result += GenerateStaticModuleSignature();

            return result;

        }

        /// <summary>
        /// Random GUID
        /// </summary>
        private static string GetGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// Get the hardcoded typescript file to be appended to the generated typescript file
        /// </summary>
        private static string ReadDefaultTemplate()
        {
            // Get absolute path
            string absolutePath = System.IO.Path.GetFullPath(mainTemplatePath);

            // Read file
            string hardcodedTs = System.IO.File.ReadAllText(absolutePath);

            return hardcodedTs;
        }

        /// <summary>
        /// Given a type, generate a typescript name for it
        // This is done in the context of a provided namespace, so that the generated type is relative to the provided namespace
        /// </summary>
        private static string GenerateTsNameFromType(Type type, NamespaceDescriptor fromCurrentNamespace, bool useUnderScoreForNamespace = false)
        {
            // Get the default name
            string typeName = type.Name;

            // Handle void case
            if (type == typeof(void))
                return "void";
            // Handle array case
            else if (type.IsArray)
            {
                return GenerateTsNameFromType(type.GetElementType(), fromCurrentNamespace) + "_CSharpArray";
            }
            // If type is async Task<T> or Task (async doesn't matter)
            else if (ReflectionUtilities.IsTypeTask(type, out bool hasReturnValue, out Type returnType))
            {
                if (!hasReturnValue)
                    return "Promise<void>";
                else
                    return "Promise<" + GenerateTsNameFromType(returnType, fromCurrentNamespace) + ">";
            }
            // Get a name that makes sense in typescript, expecially for generics
            // For generics, we add the number of parameters to the name, plus the name of each parameter with a $
            // For example, for Action<string, int> we would get Action2$System_String$System_Int32
            else if (type.IsGenericType)
            {
                typeName = type.BaseType.Name;
                typeName += type.GetGenericArguments().Length;
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    typeName += "$" + GenerateTsNameFromType(genericArgument, new NamespaceDescriptor(type), true);
                }
            }

            // If the type is in the same namespace, we don't need to prefix it
            if (fromCurrentNamespace.name == type.Namespace)
                return typeName;
            else
                return (type.Namespace + ".").Replace(".", useUnderScoreForNamespace ? "_" : ".") + typeName;

        }

        /// <summary>
        /// Generate a typescript signature from a method info
        /// </summary>
        internal static string GenerateSignatureFromMethod(MethodInfo methodInfo, NamespaceDescriptor fromCurrentNamespace)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(methodInfo.Name);
            sb.Append("(");
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                sb.Append(parameter.Name);
                sb.Append(": ");
                sb.Append(GenerateTsNameFromType(parameter.ParameterType, fromCurrentNamespace));
                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }
            sb.Append("): ");
            sb.Append(GenerateTsNameFromType(methodInfo.ReturnType, fromCurrentNamespace));
            return sb.ToString();

        }

        /// <summary>
        /// Generate the static module signature to expose all static 
        /// </summary>
        private static string GenerateStaticModuleSignature()
        {
            StringBuilder sb = new StringBuilder();

            // For each static method under each namespace > type > method, we want to create a nested object in the static module
            // For example, for static class ACoolObject under the namespace Nahoum.UnityJSInterop, we would have: Nahoum.UnityJSInterop.ACoolObject_static
            var sortedMethods = TypescriptGenerationUtilities.GetExposedTypesByNamespace();
            foreach (var namespaceEntry in sortedMethods)
            {
                NamespaceDescriptor targetNamespace = namespaceEntry.Key;
                HashSet<Type> types = namespaceEntry.Value;

                // Open the namespace
                bool hasNamespace = targetNamespace.HasNamespace;
                if (hasNamespace)
                    sb.AppendLine($"\"{targetNamespace.name}\":{{");

                // Expose static signatures here
                foreach (Type type in types)
                {
                    // Get the exposed methods for this type
                    TypescriptGenerationUtilities.GetExposedMethodsSorted(type, out Dictionary<MethodInfo, ExposeWebAttribute> staticMethods, out _);

                    // If no static methods, skip
                    if (staticMethods.Count == 0)
                        continue;

                    // Otherwise, add the static type
                    string simpleTypeName = type.Name;
                    string fullyQualifiedTypeName = GenerateTsNameFromType(type, new NamespaceDescriptor());
                    sb.AppendLine($"{simpleTypeName}: {fullyQualifiedTypeName}_static;");
                }

                // Exit the namespace
                if (hasNamespace)
                    sb.AppendLine("};");
            }

            // Get main template
            string hardcodedTs = ReadDefaultTemplate();

            // Replace the placeholder with the generated static module
            hardcodedTs = hardcodedTs.Replace("/*STATIC_MODULE_PLACEHOLDER*/", sb.ToString());
            return hardcodedTs;
        }
    }
}