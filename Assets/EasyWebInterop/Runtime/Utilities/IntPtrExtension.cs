
using System;

namespace Nahoum.EasyWebInterop
{
    internal class IntPtrExtension
    {
        public readonly static IntPtr True = new IntPtr(1);
        public readonly static IntPtr False = IntPtr.Zero;
        public readonly static IntPtr Null = new IntPtr(-1);
        public readonly static IntPtr Undefined = new IntPtr(-2);
        public readonly static IntPtr Exception = new IntPtr(-3);
    }
}