using System;
using System.Runtime.InteropServices;

namespace Nahoum.EasyWebInterop
{
    internal static class GCUtils
    {

        /// <summary>
        /// Given an object, return a GCHandle ptr to the object
        /// </summary>
        internal static IntPtr NewManagedObject(object targetObject)
        {
            // Handle null case
            if (targetObject == null)
                return IntPtr.Zero;

            GCHandle elementHandle = GCHandle.Alloc(targetObject);
            return GCHandle.ToIntPtr(elementHandle);
        }

        /// <summary>
        /// Returns the managed object from the GCHandle ptr
        /// </summary>
        internal static object GetManagedObjectFromPtr(IntPtr targetObject)
        {
            if (targetObject == IntPtr.Zero)
                return null;

            return GCHandle.FromIntPtr(targetObject).Target;
        }
    }
}