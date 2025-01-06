using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Tests")]
[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Editor")]
namespace Nahoum.UnityJSInterop
{
    [Preserve]
    public static class UnityJSInteropLoader

    {
        // Protection against double setup
        static bool isSetup = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
        public static void Setup()
        {
            if (isSetup)
                throw new System.Exception("UnityJSInterop is already setup. You should only call this method once.");
#if !UNITY_EDITOR && UNITY_WEBGL
            InternalInteropSetup.Setup();
            AutoRegister.Setup();
#endif
            isSetup = true;
        }
    }
}