using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Nahoum.UnityJSInterop
{

    public struct NamespaceDescriptor
    {
        public NamespaceDescriptor(string name)
        {
            this.name = name;
        }

        public bool Contains(Type type)
        {
            return type.Namespace == name;
        }

        public string name;
    }
    public class TypescriptGenerator
    {
        // Menu items in unity top bar to call this method
        [MenuItem("UnityJSInterop/Generate Typescript")]
        public static void GenerateTypescript()
        {
            // Gather all types with exposed methods
            var allTypesWithExposedMethods = ExposeWebAttribute.GetAllTypesWithWebExposeMethods();
            List<Type> typesToGenerate = new List<Type>();
            foreach (var type in allTypesWithExposedMethods)
            {

                // For each type, get all the exposed methods
                var exposedMethods = ExposeWebAttribute.GetExposedMethods(type);

                // For each methods, get the types of the parameters and output
                foreach (var method in exposedMethods)
                {
                    var methodInfo = method.Key;
                    var parameters = methodInfo.GetParameters();
                    var returnType = methodInfo.ReturnType;
                    typesToGenerate.Add(returnType);
                    foreach (var parameter in parameters)
                    {
                        typesToGenerate.Add(parameter.ParameterType);
                    }
                }
            }

            // Now order types by namespace
            Dictionary<NamespaceDescriptor, HashSet<Type>> typesByNamespace = new Dictionary<NamespaceDescriptor, HashSet<Type>>();
            foreach (var type in typesToGenerate)
            {
                // Get the namespace of the type and add it to the dictionary
                NamespaceDescriptor namespaceName = new NamespaceDescriptor(type.Namespace);
                if (!typesByNamespace.ContainsKey(namespaceName))
                    typesByNamespace.Add(namespaceName, new HashSet<Type>());

                // Add the type to the namespace
                typesByNamespace[namespaceName].Add(type);
            }

            // Output ts for each namespace
            StringBuilder sb = new StringBuilder();
            HashSet<Type> alreadyGeneratedGenericTypes = new HashSet<Type>();
            foreach (var namespaceEntry in typesByNamespace)
            {
                NamespaceDescriptor namespaceName = namespaceEntry.Key;
                HashSet<Type> types = namespaceEntry.Value;

                sb.AppendLine("namespace " + namespaceName.name + " {");

                foreach (var type in types)
                {
                    // Don't regenerate types deriving from generics (only once is needed)
                    if (type.IsGenericType && alreadyGeneratedGenericTypes.Contains(type))
                        continue;
                    alreadyGeneratedGenericTypes.Add(type);

                    var typeName = GenerateTsNameFromType(type, namespaceName);
                    sb.AppendLine("export type " + typeName + " = {");

                    // Get all exposed methods within this type
                    var exposedMethods = ExposeWebAttribute.GetExposedMethods(type);
                    foreach (var method in exposedMethods)
                    {
                        var methodInfo = method.Key;
                        var signature = GenerateSignatureFromMethod(methodInfo, namespaceName);
                        sb.AppendLine(signature + ";");
                    }
                    sb.AppendLine("}");

                }
                sb.AppendLine("}");
            }

            UnityEngine.Debug.Log(sb.ToString());

        }

        /// <summary>
        /// Given a type, generate a typescript name for it
        // This is done in the context of a provided namespace, so that we know if we have to fully qualify the type name
        /// </summary>
        private static string GenerateTsNameFromType(Type type, NamespaceDescriptor fromCurrentNamespace)
        {
            string typeName = type.Name;

            // handle array case
            if (type.IsArray)
                typeName = GetTsNameForArray(type, fromCurrentNamespace);
            // Get a name that makes sense in typescript, expecially for generics
            else if (type.IsGenericType)
            {
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
                typeName += "<";
                var genericArguments = type.GetGenericArguments();

                for (int i = 0; i < genericArguments.Length; i++)
                {
                    typeName += GenerateTsNameFromType(genericArguments[i], fromCurrentNamespace);
                    if (i < genericArguments.Length - 1)
                        typeName += ", ";
                }
                typeName += ">";
            }


            // If the type is in the same namespace, we don't need to prefix it
            if (fromCurrentNamespace == type.Namespace)
                return typeName;
            else
                return type.Namespace + "." + typeName;

        }


        public static string GetTsNameForArray(Type arrayType, NamespaceDescriptor fromCurrentNamespace)
        {
            // Check type represents an array
            if (arrayType.IsArray)
                return GenerateTsNameFromType(arrayType.GetElementType(), fromCurrentNamespace) + "Arr";
            throw new Exception("Type is not an array");
        }

        /// <summary>
        /// Generate a typescript signature from a method
        /// </summary>
        public static string GenerateSignatureFromMethod(MethodInfo methodInfo, NamespaceDescriptor fromCurrentNamespace)
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
    }
}