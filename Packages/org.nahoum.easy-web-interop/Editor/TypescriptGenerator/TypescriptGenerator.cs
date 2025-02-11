using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

namespace Nahoum.UnityJSInterop.Editor
{
    public class TypescriptGenerator
    {
        // A GUID generator
        static HashSet<Type> additionalTypesToGenerate = new HashSet<Type>(){
            typeof(String),
            typeof(Double),
            typeof(Int32),
            typeof(Byte),
            typeof(Boolean),
            typeof(Single),
            typeof(Int64),
            typeof(Action),
            typeof(string[]),
            typeof(int[]),
            typeof(bool[]),
            typeof(double[]),
            typeof(byte[]),
            typeof(float[]),
            typeof(IList),
            typeof(ICollection),
            typeof(IEnumerable),
            typeof(IEnumerator),
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
            string path = EditorUtility.SaveFilePanel("Save Typescript file", "", "UnityJSInterop.d.ts", "ts");

            if (string.IsNullOrEmpty(path))
                return;

            // Generate TS
            string ts = GenerateTypescript();

            // Write to file
            System.IO.File.WriteAllText(path, ts);
        }

        /// <summary>
        /// Generate a typescript file describing all exposed methods with the ExposeWebAttribute
        /// Save it in a .d.ts file of your choice then assign it directly to unity module in your js project
        /// </summary>
        public static string GenerateTypescript()
        {
            // Generate the typescript description
            Dictionary<TsNamespaceDescriptor, HashSet<TsTypeDescriptor>> typescriptDescription = GenerateTypescriptTypesToExportDescription();

            // Generate the typescript file
            string typescriptString = GenerateTypescriptFromTsDescriptor(typescriptDescription);

            // Append the static module signature
            typescriptString += GenerateStaticModuleSignature();

            return typescriptString;
        }

        /// <summary>
        /// Generate a typescript file describing all exposed types
        /// Classified by namespace
        /// </summary>
        internal static Dictionary<TsNamespaceDescriptor, HashSet<TsTypeDescriptor>> GenerateTypescriptTypesToExportDescription()
        {
            // For each type, additions the parameters and return types of each exposed methodsmethods
            ISet<Type> allTypesExported = TypescriptGenerationUtilities.GetTypesToGenerateTypesFileFrom(excludeTestsAssemblies: true);

            // Add additional types to generate
            foreach (Type additionnalType in additionalTypesToGenerate)
                allTypesExported.Add(additionnalType);

            // Create a list of all types to generate
            Dictionary<TsNamespaceDescriptor, HashSet<TsTypeDescriptor>> typesToGenerate = new Dictionary<TsNamespaceDescriptor, HashSet<TsTypeDescriptor>>();

            // Generate the types to generate
            foreach (Type type in allTypesExported)
            {
                TsNamespaceDescriptor namespaceDescriptor = TsNamespaceDescriptor.CreateFrom(type);

                if (!typesToGenerate.ContainsKey(namespaceDescriptor))
                    typesToGenerate[namespaceDescriptor] = new HashSet<TsTypeDescriptor>();

                // Get the exposed methods for this type
                TypescriptGenerationUtilities.GetExposedMethodsSorted(type, out ISet<MethodInfo> staticMethods, out ISet<MethodInfo> instanceMethods);

                bool isStaticType = type.IsAbstract && type.IsSealed;
                bool hasStaticMethods = staticMethods.Count > 0;
                string typeName = GenerateTsNameFromType(type, namespaceDescriptor);

                // Handle static methods
                if (hasStaticMethods)
                {
                    TsTypeDescriptor staticTypeDescriptor = new TsTypeDescriptor()
                    {
                        TypeName = typeName + "_static",
                    };

                    HashSet<TsProperty> properties = new HashSet<TsProperty>
                    {
                        new TsProperty($"fullTypeName_{GetGuid()}", $"'{type.FullName}'"),
                        new TsProperty($"assembly_{GetGuid()}", $"'{type.Assembly.FullName}'")
                    };

                    foreach (MethodInfo method in staticMethods)
                        properties.Add(GenerateSignatureFromMethod(method, namespaceDescriptor));

                    staticTypeDescriptor.Properties = properties;
                    typesToGenerate[namespaceDescriptor].Add(staticTypeDescriptor);
                }

                // Handle instance methods
                if (!isStaticType)
                {
                    TsTypeDescriptor instanceTypeDescriptor = new TsTypeDescriptor()
                    {
                        TypeName = typeName,
                    };
                    HashSet<TsProperty> properties = new HashSet<TsProperty>
                    {
                        new TsProperty($"fullTypeName_{GetGuid()}", $"'{type.FullName}'"),
                        new TsProperty($"assembly_{GetGuid()}", $"'{type.Assembly.FullName}'")
                    };

                    // For all objects, we may gather the value of the serializer instance object
                    if (ObjectSerializer.TryGetSerializer(type, out IJsJsonSerializer serializer) && serializer.CanSerialize(type, out ITsTypeDescriptor tsDescriptor))
                        properties.Add(new TsProperty("value", tsDescriptor.GetTsTypeDefinition(type)));
                    else
                        properties.Add(new TsProperty("value", "unknown"));

                    // Get all exposed methods within this type
                    foreach (MethodInfo method in instanceMethods)
                        properties.Add(GenerateSignatureFromMethod(method, namespaceDescriptor));
                    instanceTypeDescriptor.Properties = properties;

                    // Add inherited types
                    HashSet<Type> inheritingTypes = TypescriptGenerationUtilities.GetAllInheritingTypes(type);
                    ISet<string> inheritingTypesString = new HashSet<string>();
                    foreach (Type inheritingType in inheritingTypes)
                    {
                        // If the inheriting type is not exposed, skip because in any case it will be totally useless
                        if (!allTypesExported.Contains(inheritingType))
                            continue;

                        string inheritingTypeName = GenerateTsNameFromType(inheritingType, namespaceDescriptor);
                        inheritingTypesString.Add(inheritingTypeName);
                    }

                    // Also add the static marker if applicable
                    if (hasStaticMethods)
                        inheritingTypesString.Add(typeName + "_static");

                    instanceTypeDescriptor.InheritedTypes = inheritingTypesString;
                    typesToGenerate[namespaceDescriptor].Add(instanceTypeDescriptor);
                }
            }

            return typesToGenerate;
        }

        /// <summary>
        /// Given a dictionary of types to export organized be namespace, generate a string representing the typescript file with all types exported, classified by namespace
        /// </summary>
        private static string GenerateTypescriptFromTsDescriptor(Dictionary<TsNamespaceDescriptor, HashSet<TsTypeDescriptor>> keyValuePairs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var namespaceEntry in keyValuePairs)
            {
                TsNamespaceDescriptor targetNamespace = namespaceEntry.Key;
                HashSet<TsTypeDescriptor> types = namespaceEntry.Value;
                targetNamespace.WriteStartExportNamespace(sb);

                foreach (var type in types)
                {
                    sb.AppendLine(type.GetExportTypeTsString());
                }

                targetNamespace.WriteEndExportNamespace(sb);
            }

            var result = sb.ToString();

            return result;
        }

        /// <summary>
        /// Generates a random GUID
        /// </summary>
        private static string GetGuid() => Guid.NewGuid().ToString().Replace("-", "");

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
        private static string GenerateTsNameFromType(Type type, TsNamespaceDescriptor fromCurrentNamespace, bool useUnderScoreForNamespace = false)
        {
            // Get the default name
            string typeName = type.Name;

            // Handle void case
            if (type == typeof(void))
                return "void";

            // Handle array case, for which we take the element type and add _CSharpArray
            else if (type.IsArray)
            {
                return GenerateTsNameFromType(type.GetElementType(), fromCurrentNamespace) + "_CSharpArray";
            }
            // If type is async Task<T> or Task (async doesn't matter), we return Promise<T> or Promise<void>
            else if (ReflectionUtilities.IsTypeTask(type, out bool hasReturnValue, out Type returnType))
            {
                return $"Promise<{(!hasReturnValue ? "void" : GenerateTsNameFromType(returnType, fromCurrentNamespace))}>";
            }
            // Get a name that makes sense in typescript, expecially for generics
            // For generics, we add the number of parameters to the name, plus the name of each parameter with a $
            // For example, for Action<string, int> we would get Action2$System_String$System_Int32
            else if (type.IsGenericType)
            {
                typeName = type.Name.Substring(0, type.Name.IndexOf('`'));
                typeName += type.GetGenericArguments().Length;
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    typeName += "$" + GenerateTsNameFromType(genericArgument, TsNamespaceDescriptor.CreateFrom(type), true);
                }
            }

            // If the type is in the same namespace, we don't need to prefix it
            if (fromCurrentNamespace.NamespaceName == type.Namespace)
                return typeName;
            else
                return (type.Namespace + ".").Replace(".", useUnderScoreForNamespace ? "_" : ".") + typeName;
        }

        /// <summary>
        /// Generate a typescript signature from a method info
        /// </summary>
        internal static TsProperty GenerateSignatureFromMethod(MethodInfo methodInfo, TsNamespaceDescriptor fromCurrentNamespace)
        {
            string methodSignature = methodInfo.Name + "(";
            string methodReturnType = GenerateTsNameFromType(methodInfo.ReturnType, fromCurrentNamespace);

            // Handle method name
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                methodSignature += parameter.Name;
                methodSignature += ": ";
                methodSignature += GenerateTsNameFromType(parameter.ParameterType, fromCurrentNamespace);
                if (i < parameters.Length - 1)
                    methodSignature += ", ";
            }
            methodSignature += ")";

            // Handle return type
            return new TsProperty(methodSignature, methodReturnType);

        }

        /// <summary>
        /// Describes a type in typescript, with its properties and inherited types
        /// Used to export a type in typescript like "export type ..."
        /// </summary>
        internal struct TsTypeDescriptor
        {
            public string TypeName;

            // Typically the entries of the type, aka method / type
            public ISet<TsProperty> Properties;

            // The types to inherit from
            public ISet<string> InheritedTypes;

            internal string GetExportTypeTsString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("export type ");
                stringBuilder.Append(TypeName);
                stringBuilder.AppendLine(" = {");

                // Add properties
                foreach (var property in Properties)
                    property.WriteProperty(stringBuilder);

                // Close the type definition
                stringBuilder.Append("}");

                // Add inherited types
                if (InheritedTypes != null)
                {
                    foreach (var inheritedType in InheritedTypes)
                    {
                        stringBuilder.Append("& ");
                        stringBuilder.Append(inheritedType);
                    }
                }

                // Close the type
                stringBuilder.AppendLine(";");

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Represents a property in typescript
        /// Like key: value;
        /// </summary>
        internal struct TsProperty
        {
            public string Key;
            public string Value;

            public TsProperty(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public void WriteProperty(StringBuilder stringBuilder)
            {
                stringBuilder.Append(Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(Value);
                stringBuilder.Append(";");
                stringBuilder.AppendLine();
            }
        }

        /// <summary>
        /// Represents a namespace in typescript
        /// Allows to export the namespace like "export namespace ..."
        /// </summary>
        internal struct TsNamespaceDescriptor
        {
            public readonly string NamespaceName;

            public void WriteStartExportNamespace(StringBuilder stringBuilder)
            {
                if (string.IsNullOrEmpty(NamespaceName))
                    return;

                stringBuilder.Append("export namespace ");
                stringBuilder.Append(NamespaceName);
                stringBuilder.AppendLine(" {");

            }

            public void WriteEndExportNamespace(StringBuilder stringBuilder)
            {
                if (string.IsNullOrEmpty(NamespaceName))
                    return;
                stringBuilder.AppendLine("}");
            }

            public void WriteStartNamespaceNameAsKey(StringBuilder stringBuilder)
            {
                if (string.IsNullOrEmpty(NamespaceName))
                    return;

                stringBuilder.Append($"\"{NamespaceName}\"");
                stringBuilder.Append(": {");
            }

            public void WriteEndNamespaceNameAsKey(StringBuilder stringBuilder)
            {
                if (string.IsNullOrEmpty(NamespaceName))
                    return;

                stringBuilder.Append("},");
            }

            public bool Contains(Type type)
            {
                return type.Namespace == NamespaceName;
            }

            public static TsNamespaceDescriptor CreateFrom(Type type)
            {
                return new TsNamespaceDescriptor(type.Namespace);
            }

            public static TsNamespaceDescriptor Empty() => new TsNamespaceDescriptor(string.Empty);

            private TsNamespaceDescriptor(string namespaceName)
            {
                NamespaceName = namespaceName;
            }
        }

        /// <summary>
        /// Generates the typescript signature for the delegate constructors on the JS side
        /// This will allow to expose the constructors of delegates in JS
        /// For example Action<string> will be able to be created via Module.extras['System']['Action<String>'].createDelegate((a) => console.log(a.value));
        /// </summary>
        private static string GenerateDelegateConstructorExtraSignature()
        {
            ISet<Type> allTypes = TypescriptGenerationUtilities.GetTypesToGenerateTypesFileFrom(excludeTestsAssemblies: true);
            Dictionary<TsNamespaceDescriptor, HashSet<TsProperty>> sortedTypes = new Dictionary<TsNamespaceDescriptor, HashSet<TsProperty>>();

            // Generate delegate constructors
            foreach (Type type in allTypes)
            {
                TypescriptGenerationUtilities.GetExposedMethodsSorted(type, out ISet<MethodInfo> staticMethods, out ISet<MethodInfo> instanceMethods);
                if (staticMethods.Count == 0 && instanceMethods.Count == 0)
                    continue;
                HashSet<MethodInfo> methods = new HashSet<MethodInfo>();
                methods.UnionWith(staticMethods);
                methods.UnionWith(instanceMethods);

                HashSet<Type> delegateTypesToGenerateConstructor = new();
                foreach (var item in methods)
                {
                    if (ReflectionUtilities.MethodHasReturnlessDelegateParameter(item, out IReadOnlyList<ParameterInfo> delegateParameters))
                    {
                        foreach (var parameter in delegateParameters)
                        {
                            delegateTypesToGenerateConstructor.Add(parameter.ParameterType);
                        }
                    }
                }

                // Now we can generate the typescript
                foreach (var delegateType in delegateTypesToGenerateConstructor)
                {
                    TsNamespaceDescriptor namespaceDescriptor = TsNamespaceDescriptor.CreateFrom(delegateType);
                    if (!sortedTypes.ContainsKey(namespaceDescriptor))
                        sortedTypes[namespaceDescriptor] = new HashSet<TsProperty>();

                    // Get delegate type parameters
                    var parameters = delegateType.GetMethod("Invoke").GetParameters();
                    //string parametersString = string.Join(", ", parameters.Select(p => GenerateTsNameFromType(p.ParameterType, TsNamespaceDescriptor.Empty())));
                    // join parameters so that args are named 0: string, 1: number, etc
                    string[] alphabet = "abcdefghijklmnopqrstuvwxyz".Select(c => c.ToString()).ToArray();
                    string parametersString = string.Join(", ", parameters.Select((p, i) => $"{alphabet[i]}: {GenerateTsNameFromType(p.ParameterType, TsNamespaceDescriptor.Empty())}"));

                    // Create the type descriptor
                    TsProperty staticTypeDescriptor = new TsProperty()
                    {
                        Key = $"\"{NamingUtility.GenerateWellFormattedJSNameForType(delegateType)}\"",
                        Value = $"{{ createDelegate: (callback: ({parametersString}) => void) => {GenerateTsNameFromType(delegateType, TsNamespaceDescriptor.Empty())} }}",
                    };
                    sortedTypes[namespaceDescriptor].Add(staticTypeDescriptor);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<TsNamespaceDescriptor, HashSet<TsProperty>> item in sortedTypes)
            {
                // Write the namespace
                TsNamespaceDescriptor namespaceDescriptor = item.Key;
                namespaceDescriptor.WriteStartNamespaceNameAsKey(sb);

                // Write each static type in the namespace
                foreach (TsProperty typeDesc in item.Value)
                    typeDesc.WriteProperty(sb);

                // Close namespace
                namespaceDescriptor.WriteEndNamespaceNameAsKey(sb);
            }

            return sb.ToString();

        }
        /// <summary>
        /// Generate the static module signature to expose all static classes (which is basically the entrypoint)
        /// </summary>
        private static string GenerateStaticModuleSignature()
        {

            // For each static method under each namespace > type > method, we want to create a nested object in the static module
            // For example, for static class ACoolObject under the namespace Nahoum.UnityJSInterop, we would have: Nahoum.UnityJSInterop.ACoolObject_static
            ISet<Type> allTypes = TypescriptGenerationUtilities.GetTypesToGenerateTypesFileFrom(excludeTestsAssemblies: true);
            Dictionary<TsNamespaceDescriptor, HashSet<TsProperty>> sortedTypes = new Dictionary<TsNamespaceDescriptor, HashSet<TsProperty>>();

            foreach (Type type in allTypes)
            {
                TypescriptGenerationUtilities.GetExposedMethodsSorted(type, out ISet<MethodInfo> staticMethods, out _);
                if (staticMethods.Count == 0)
                    continue;

                // Otheriwse we can expose the type
                TsNamespaceDescriptor namespaceDescriptor = TsNamespaceDescriptor.CreateFrom(type);
                if (!sortedTypes.ContainsKey(namespaceDescriptor))
                    sortedTypes[namespaceDescriptor] = new HashSet<TsProperty>();

                // Create the type descriptor
                TsProperty staticTypeDescriptor = new TsProperty()
                {
                    Key = GenerateTsNameFromType(type, namespaceDescriptor),
                    Value = GenerateTsNameFromType(type, TsNamespaceDescriptor.Empty()) + "_static",
                };
                sortedTypes[namespaceDescriptor].Add(staticTypeDescriptor);
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<TsNamespaceDescriptor, HashSet<TsProperty>> item in sortedTypes)
            {
                // Write the namespace
                TsNamespaceDescriptor namespaceDescriptor = item.Key;
                namespaceDescriptor.WriteStartNamespaceNameAsKey(sb);

                // Write each static type in the namespace
                foreach (TsProperty typeDesc in item.Value)
                    typeDesc.WriteProperty(sb);

                // Close namespace
                namespaceDescriptor.WriteEndNamespaceNameAsKey(sb);
            }

            // Get main template
            string hardcodedTs = ReadDefaultTemplate();

            // Replace the placeholder with the generated static module
            hardcodedTs = hardcodedTs.Replace("/*STATIC_MODULE_PLACEHOLDER*/", sb.ToString());

            // Also generate the delegate constructor part and replace /*EXTRAS_PLACEHOLDER*/ with it
            string delegateConstructorUtilities = GenerateDelegateConstructorExtraSignature();
            hardcodedTs = hardcodedTs.Replace("/*EXTRAS_PLACEHOLDER*/", delegateConstructorUtilities);

            return hardcodedTs;
        }
    }
}