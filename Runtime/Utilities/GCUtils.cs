using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Nahoum.UnityJSInterop
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
            // Handle reference types
            if (targetObject == IntPtrExtension.Null)
                return null;

            // Get object from GCHandle
            return GCHandle.FromIntPtr(targetObject).Target;
        }

        /// <summary>
        /// Given a GCHandle ptr, free the object behind it
        /// Also handle the null case
        /// </summary>
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