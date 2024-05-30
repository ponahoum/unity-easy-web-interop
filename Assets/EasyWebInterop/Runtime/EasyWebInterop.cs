namespace Nahoum.EasyWebInterop
{
    public static class EasyWebInterop
    {
        public static void Setup()
        {
            InternalInteropSetup.Setup();
            AutoRegister.Setup();
        }
    }
}