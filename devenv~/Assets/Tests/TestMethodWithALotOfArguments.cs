using AnotherNamespace;
using Nahoum.UnityJSInterop;
using Nahoum.UnityJSInterop.Tests;

namespace Nahoum.UnityJSInterop.Tests
{
    /// <summary>
    /// Test the maximum number of arguments we may have in exposed methods.
    /// This is limited by the registry.
    /// </summary>
    public class TestMethodWithALotOfArguments
    {
        /// <summary>
        /// Method with 1 argument.
        /// </summary>
        [ExposeWeb]
        public static int OneArgument(int firstArg)
        {
            return firstArg;
        }

        /// <summary>
        /// Method with 2 arguments.
        /// </summary>
        [ExposeWeb]
        public static int TwoArguments(int firstArg, int secondArg)
        {
            return Sum(firstArg, secondArg);
        }

        /// <summary>
        /// Method with 3 arguments.
        /// </summary>
        [ExposeWeb]
        public static int ThreeArguments(int firstArg, int secondArg, int thirdArg)
        {
            return Sum(firstArg, secondArg, thirdArg);
        }

        /// <summary>
        /// Method with 4 arguments.
        /// </summary>
        [ExposeWeb]
        public static int FourArguments(int firstArg, int secondArg, int thirdArg, int fourthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg);
        }

        /// <summary>
        /// Method with 5 arguments.
        /// </summary>
        [ExposeWeb]
        public static int FiveArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg);
        }

        /// <summary>
        /// Method with 6 arguments.
        /// </summary>
        [ExposeWeb]
        public static int SixArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg, int sixthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg, sixthArg);
        }

        /// <summary>
        /// Method with 7 arguments.
        /// </summary>
        [ExposeWeb]
        public static int SevenArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg, int sixthArg, int seventhArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg, sixthArg, seventhArg);
        }

        /// <summary>
        /// Method with 8 arguments.
        /// </summary>
        [ExposeWeb]
        public static int EightArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg, int sixthArg, int seventhArg, int eighthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg, sixthArg, seventhArg, eighthArg);
        }

        /// <summary>
        /// Method with 9 arguments.
        /// </summary>
        [ExposeWeb]
        public static int NineArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg, int sixthArg, int seventhArg, int eighthArg, int ninthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg, sixthArg, seventhArg, eighthArg, ninthArg);
        }

        /// <summary>
        /// Method with 10 arguments.
        /// </summary>
        [ExposeWeb]
        public static int TenArguments(int firstArg, int secondArg, int thirdArg, int fourthArg, int fifthArg, int sixthArg, int seventhArg, int eighthArg, int ninthArg, int tenthArg)
        {
            return Sum(firstArg, secondArg, thirdArg, fourthArg, fifthArg, sixthArg, seventhArg, eighthArg, ninthArg, tenthArg);
        }

        /// <summary>
        /// Helper method that sums an array of integers.
        /// </summary>
        private static int Sum(params int[] args)
        {
            int sum = 0;
            for (int i = 0; i < args.Length; i++)
            {
                sum += args[i];
            }
            return sum;
        }
    }
}
