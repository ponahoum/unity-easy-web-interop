using System;
using UnityEngine.Scripting;

namespace Nahoum.UnityJSInterop
{
    [Preserve]
    internal static class DyncallSignature
    {
        public delegate void V();
        public delegate void VI(IntPtr A);
        public delegate void VII(IntPtr A, IntPtr B);
        public delegate void VIII(IntPtr A, IntPtr B, IntPtr C);
        public delegate void VIIII(IntPtr A, IntPtr B, IntPtr C, IntPtr D);
        public delegate void VIIIII(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E);
        public delegate IntPtr I();
        public delegate IntPtr II(IntPtr A);
        public delegate IntPtr III(IntPtr inputA, IntPtr inputB);
        public delegate IntPtr IIII(IntPtr inputA, IntPtr inputB, IntPtr inputC);
        public delegate IntPtr IIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD);
        public delegate IntPtr IIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE);
        public delegate IntPtr IIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF);
        public delegate IntPtr IIIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG);
        public delegate IntPtr IIIIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH);
        public delegate IntPtr IIIIIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI);
        public delegate IntPtr IIIIIIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI, IntPtr inputJ);
        public delegate IntPtr IIIIIIIIIIII(IntPtr inputA, IntPtr inputB, IntPtr inputC, IntPtr inputD, IntPtr inputE, IntPtr inputF, IntPtr inputG, IntPtr inputH, IntPtr inputI, IntPtr inputJ, IntPtr inputK);
    }
}