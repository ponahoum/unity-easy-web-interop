namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Test exposing getters and setters.
    /// </summary>
    public class TestGetSets
    {
        public static string TestString { [ExposeWeb] get; [ExposeWeb] set; } = SampleValues.TestString;
    }
}