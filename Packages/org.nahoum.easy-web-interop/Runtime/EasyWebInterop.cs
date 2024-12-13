using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Tests")]
[assembly: InternalsVisibleTo("Nahoum.UnityJSInterop.Editor")]
namespace Nahoum.UnityJSInterop
{
    public static class UnityJSInterop
    {
        // Protection against double setup
        static bool isSetup = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Setup()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            if(isSetup)
            throw new System.Exception("UnityJSInterop is already setup. You should only call this method once.");
            InternalInteropSetup.Setup();
            AutoRegister.Setup();
            isSetup = true;
#endif
        }
    }
}