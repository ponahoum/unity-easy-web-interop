using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Tests")]
[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Editor")]
namespace Nahoum.UnityJSInterop
{
    public static class UnityJSInterop
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Setup()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            InternalInteropSetup.Setup();
            AutoRegister.Setup();
            #endif
        }
    }
}