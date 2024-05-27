using System;
using System.Runtime.InteropServices;

internal static class MarshalUtilities
{
    /// <summary>
    /// Method to marshal string array to IntPtr and provide a free function to liberate the memory after the intptr has been used
    /// </summary>
    internal static IntPtr MarshalStringArray(string[] array, out int length, out Action free)
    {
        // Allocate memory for the array of string pointers
        IntPtr arrayPtr = Marshal.AllocHGlobal(IntPtr.Size * array.Length);

        // Allocate memory and copy each string
        for (int i = 0; i < array.Length; i++)
        {
            IntPtr stringPtr = Marshal.StringToHGlobalAnsi(array[i]);
            Marshal.WriteIntPtr(arrayPtr, i * IntPtr.Size, stringPtr);
        }

        // Set the free function
        free = () => FreeStringArray(arrayPtr, array.Length);
        length = array.Length;
        return arrayPtr;
    }

    /// <summary>
    /// Private method to free the allocated memory for string array
    /// </summary>
    private static void FreeStringArray(IntPtr arrayPtr, int length)
    {
        for (int i = 0; i < length; i++)
        {
            IntPtr stringPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
            Marshal.FreeHGlobal(stringPtr);
        }
        Marshal.FreeHGlobal(arrayPtr);
    }

    /// <summary>
    /// Method to marshal a single string to IntPtr and provide a free function
    /// </summary>
    internal static IntPtr MarshalString(string str, out Action free)
    {
        IntPtr stringPtr = Marshal.StringToHGlobalAnsi(str);

        // Set the free function
        free = () => FreeString(stringPtr);
        return stringPtr;
    }

    /// <summary>
    /// Private method to free the allocated memory for a single string
    /// </summary>
    internal static void FreeString(IntPtr stringPtr)
    {
        Marshal.FreeHGlobal(stringPtr);
    }
}