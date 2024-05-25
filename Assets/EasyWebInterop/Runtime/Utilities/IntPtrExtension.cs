
using System;

namespace Nahoum.EasyWebInterop
{
    internal class IntPtrExtension
    {
        public readonly static IntPtr Null = new IntPtr(-1);
        public readonly static IntPtr Void = new IntPtr(-2);
        public readonly static IntPtr Exception = new IntPtr(-3);
    }
}