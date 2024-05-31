
using System;

namespace Nahoum.EasyWebInterop
{
    /// <summary>
    /// A simple extension to encode some exotic IntPtr values
    /// Those typically doesn't represent a valid memory address but a special value for the JS side
    /// </summary>
    internal class IntPtrExtension
    {
        public readonly static IntPtr Null = new IntPtr(-1);
        public readonly static IntPtr Void = new IntPtr(-2);
        public readonly static IntPtr Exception = new IntPtr(-3);
    }
}