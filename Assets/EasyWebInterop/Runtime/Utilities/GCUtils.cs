using System;
using System.Diagnostics;
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
                return IntPtrExtension.Null;

            GCHandle elementHandle = GCHandle.Alloc(targetObject);
            return GCHandle.ToIntPtr(elementHandle);
        }

        /// <summary>
        /// Returns the managed object from the GCHandle ptr
        /// </summary>
        internal static object GetManagedObjectFromPtr(IntPtr targetObject)
        {
            if (targetObject == IntPtrExtension.Null)
                return null;

            return GCHandle.FromIntPtr(targetObject).Target;
        }

        internal static void CollectManagedObjectFromPtr(IntPtr ptrToGcHandle)
        {
            if (ptrToGcHandle == IntPtrExtension.Null)
                return;

            var fromIntPtr = GCHandle.FromIntPtr(ptrToGcHandle);
            if (fromIntPtr.IsAllocated){
                fromIntPtr.Free();
            }
            else
            {
                UnityEngine.Debug.LogError("Tried to free a GCHandle that was not allocated");
                throw new InvalidOperationException("The GCHandle is not allocated");
            }
        }
    }
}