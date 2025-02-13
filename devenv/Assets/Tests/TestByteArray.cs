using System.Threading.Tasks;
using UnityEngine;

namespace Nahoum.UnityJSInterop.Tests
{
    public static class TestByteArray
    {
        [ExposeWeb]
        public static Texture2D LoadTexture(byte[] imageBytes)
        {
            // Load the PNG
            var tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            return tex;
        }

        [ExposeWeb]
        public static string GetTextureResolution(Texture2D tex)
        {
            return tex.width + "x" + tex.height;
        }

        [ExposeWeb]
        public static void DestroyTexture(Texture2D tex)
        {
            // Release the texture
            UnityEngine.Object.Destroy(tex);
        }

        [ExposeWeb]
        public static int GetByteArrayLength(byte[] byteArray)
        {
            return byteArray.Length;
        }
    }
}