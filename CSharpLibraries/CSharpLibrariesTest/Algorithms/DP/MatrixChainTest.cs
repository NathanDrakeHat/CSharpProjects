using CSharpLibraries.Algorithms.DP;
using CSharpLibraries.Utils;
using NUnit.Framework;
using static CSharpLibraries.Utils.Extension;


namespace CSharpLibrariesTest.Algorithms.DP{
    public static class MatrixChainTest{
        private static readonly MatrixIndex[] Test =
            {new(30, 35), new(35, 15), new(15, 5), new(5, 10), new(10, 20), new(20, 25)};

        private const int Cost = 35 * 15 * 5 + 30 * 35 * 5 + 5 * 10 * 20 + 5 * 20 * 25 + 30 * 5 * 25;
        private const string Res = "((1(23))((45)6))";

        [Test]
        public static void Case1(){
            var t = MatrixChain.OptimalMatrixChainOrder(Test);
            Assert.AreEqual(Res, t.ToString());
            Assert.AreEqual(Cost, t.MinCost);
        }
    }
}