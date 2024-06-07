using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Nahoum.UnityJSInterop
{

    public struct NamespaceDescriptor
    {
        public NamespaceDescriptor(Type targetType)
        {
            name = targetType.Namespace;
        }

        public bool Contains(Type type)
        {
            return type.Namespace == name;
        }

        public bool HasNamespace => !string.IsNullOrEmpty(name);

        public string name { get; private set; }
    }

    public class TypescriptGenerator
    {
        // Menu items in unity top bar to call this method
        [MenuItem("UnityJSInterop/Generate Typescript")]
        public static void GenerateTypescript()
        {
            // Gather all types with exposed methods
            IReadOnlyCollection<Type> allTypesExposingMethods = ExposeWebAttribute.GetAllTypesWithWebExposeMethods();

            // Now order types by namespace
            Dictionary<NamespaceDescriptor, HashSet<Type>> typesByNamespace = new Dictionary<NamespaceDescriptor, HashSet<Type>>();

            bool TryAddTypeToNamespace(Type type)
            {
                NamespaceDescriptor namespaceName = new NamespaceDescriptor(type);
                if (!typesByNamespace.ContainsKey(namespaceName))
                    typesByNamespace.Add(namespaceName, new HashSet<Type>());
                return typesByNamespace[namespaceName].Add(type);
            }

            // Add all types we'll need to generate in a sorted dictionary
            foreach (Type exposedType in allTypesExposingMethods)
            {
                // Get the namespace of the type and add it to the dictionary
                TryAddTypeToNamespace(exposedType);

                // Get exposed methods
                Dictionary<MethodInfo, ExposeWebAttribute> exposedMethods = ExposeWebAttribute.GetExposedMethods(exposedType);
                foreach (var method in exposedMethods)
                {
                    MethodInfo methodInfo = method.Key;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    foreach (ParameterInfo parameter in parameters)
                        TryAddTypeToNamespace(parameter.ParameterType);
                    TryAddTypeToNamespace(methodInfo.ReturnType);

                }
            }

            // For each type, additions the parameters and return types of each exposed methods

            // Output ts for each namespace
            StringBuilder sb = new StringBuilder();
            foreach (var namespaceEntry in typesByNamespace)
            {
                NamespaceDescriptor targetNamespace = namespaceEntry.Key;
                HashSet<Type> types = namespaceEntry.Value;
                if (targetNamespace.HasNamespace)
                    sb.AppendLine("export namespace " + targetNamespace.name + " {");

                foreach (Type type in types)
                {
                    string typeName = GenerateTsNameFromType(type, targetNamespace);
                    Dictionary<MethodInfo, ExposeWebAttribute> exposedMethods = ExposeWebAttribute.GetExposedMethods(type);
                    Dictionary<MethodInfo, ExposeWebAttribute> staticMethods = new Dictionary<MethodInfo, ExposeWebAttribute>();
                    Dictionary<MethodInfo, ExposeWebAttribute> instanceMethods = new Dictionary<MethodInfo, ExposeWebAttribute>();
                    foreach (var method in exposedMethods)
                    {
                        if (method.Key.IsStatic)
                            staticMethods.Add(method.Key, method.Value);
                        else
                            instanceMethods.Add(method.Key, method.Value);
                    }

                    bool hasStaticMethods = staticMethods.Count > 0;
                    bool hasInstanceMethods = instanceMethods.Count > 0;

                    if (hasStaticMethods)
                    {
                        sb.AppendLine("export type " + typeName + "_static = {");

                        // Add key to fully differentiate between types in typescript (name is not enough)
                        sb.AppendLine($"key: '{type.FullName}';");

                        foreach (var method in staticMethods)
                        {
                            var methodInfo = method.Key;
                            var signature = GenerateSignatureFromMethod(methodInfo, targetNamespace);
                            sb.AppendLine(signature + ";");
                        }
                        sb.AppendLine("}");
                    }

                    sb.AppendLine("export type " + typeName + " = {");

                    // Add key to fully differentiate between types in typescript (name is not enough)
                    sb.AppendLine($"key: '{type.FullName}';");

                    // Get all exposed methods within this type
                    foreach (var method in instanceMethods)
                    {
                        var methodInfo = method.Key;
                        var signature = GenerateSignatureFromMethod(methodInfo, targetNamespace);
                        sb.AppendLine(signature + ";");
                    }

                    sb.AppendLine("}");

                    if (hasStaticMethods)
                    {
                        sb.Append(" & " + typeName + "_static;");
                    }


                }
                if (targetNamespace.HasNamespace)
                    sb.AppendLine("}");
            }

            UnityEngine.Debug.Log(sb.ToString());

        }

        /// <summary>
        /// Given a type, generate a typescript name for it
        // This is done in the context of a provided namespace, so that the generated type is relative to the provided namespace
        /// </summary>
        private static string GenerateTsNameFromType(Type type, NamespaceDescriptor fromCurrentNamespace, bool useUnderScoreForNamespace = false)
        {
            // Get the default name
            string typeName = type.Name;

            // Handle array case
            if (type.IsArray)
            {
                typeName = GetTsNameForArray(type, fromCurrentNamespace);

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


        public static string GetTsNameForArray(Type arrayType, NamespaceDescriptor fromCurrentNamespace)
        {
            // Check type represents an array
            if (arrayType.IsArray)
                return GenerateTsNameFromType(arrayType.GetElementType(), fromCurrentNamespace) + "Arr";
            throw new Exception("Type is not an array");
        }

        /// <summary>
        /// Generate a typescript signature from a method info
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