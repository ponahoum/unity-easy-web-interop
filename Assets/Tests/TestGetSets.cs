namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Test exposing getters and setters.
    /// </summary>
    public class TestGetSets
    {
        /// <summary>
        /// Test get set on individual methods
        /// </summary>
        public static string TestString { [ExposeWeb] get; [ExposeWeb] set; } = SampleValues.TestString;


        /// <summary>
        /// Test directly on the property
        /// </summary>
        [ExposeWeb]
        public static string TestString2 { get; set; } = SampleValues.TestString2;
    }
}