using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("com.nahoum.EasyWebInterop.Tests")]
[assembly: InternalsVisibleTo("com.nahoum.EasyWebInterop.Editor")]
namespace Nahoum.EasyWebInterop
{
    public static class EasyWebInterop
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