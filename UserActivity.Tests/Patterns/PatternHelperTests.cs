using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserActivity.Viewer.Patterns;

namespace UserActivity.Tests.Patterns
{
    [TestClass]
    public class PatternHelperTests
    {
        [TestMethod]
        public void DistinctMany_Sample1()
        {
            var sessions = new int[][]
            {
                new int[] {3, 2, 3 },
                new int[] {1, 1, 1, 2, 3 }
            };
            var expected = new int[] { 1, 2, 3 };
            var actual = sessions.DistinctMany();
            CollectionAssert.AreEqual(expected, actual);
        }

        private void AssertVariations(int[][] expected, int[] classes, int minLength, int maxLength)
        {
            var actual = classes.Variations(minLength, maxLength);
            Assert.AreEqual(expected.Length, actual.Length, "Count of variations must be equal.");
            for (int i = 0; i < expected.Length; i++)
            {
                CollectionAssert.AreEqual(expected[i], actual[i], "Lengths of variations must be equal.");
            }
        }

        [TestMethod]
        public void Variations_Sample1()
        {
            var classes = new int[] { 2, 5 };
            var expected = new int[][]
            {
                new int[] {2, 2},
                new int[] {2, 5},
                new int[] {5, 2},
                new int[] {5, 5}
            };
            AssertVariations(expected, classes, 2, 2);
        }

        [TestMethod]
        public void Variations_Sample2()
        {
            var classes = new int[] { 1, 2, 3 };
            var expected = new int[][]
            {
                new int[] {1, 1},
                new int[] {1, 2},
                new int[] {1, 3},
                new int[] {2, 1},
                new int[] {2, 2},
                new int[] {2, 3},
                new int[] {3, 1},
                new int[] {3, 2},
                new int[] {3, 3},
            };
            AssertVariations(expected, classes, 2, 2);
        }

        [TestMethod]
        public void Variations_Sample3()
        {
            var classes = new int[] { 1, 2 };
            var expected = new int[][]
            {
                new int[] {1, 1},
                new int[] {1, 2},
                new int[] {2, 1},
                new int[] {2, 2},
                new int[] {1, 1, 1},
                new int[] {1, 1, 2},
                new int[] {1, 2, 1},
                new int[] {1, 2, 2},
                new int[] {2, 1, 1},
                new int[] {2, 1, 2},
                new int[] {2, 2, 1},
                new int[] {2, 2, 2},
            };
            AssertVariations(expected, classes, 2, 3);
        }

        [TestMethod]
        public void Count_Sample1()
        {
            var array = new int[] { 1, 2, 1, 2, 1, 3, 1, 2, 1 };
            var subarray = new int[] { 1, 2, 1 };
            int expected = 2;
            int actual = array.Count(subarray);
            Assert.AreEqual<int>(expected, actual);
        }

        [TestMethod]
        public void Count_Sample2()
        {
            var array = new int[] { 2, 1, 2, 1, 3, 2, 1, 2, 1, 3 };
            var subarray = new int[] { 2, 1, 2, 1, 3 };
            int expected = 2;
            int actual = array.Count(subarray);
            Assert.AreEqual<int>(expected, actual);
        }
    }
}