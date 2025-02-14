using System;

namespace Nahoum.UnityJSInterop.Editor
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
}