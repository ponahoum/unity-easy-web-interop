using System;

namespace Nahoum.EasyWebInterop
{
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
    }
}