using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest45
{
    [TestClass]
    public class FelicaHelperTest
    {
        [TestMethod]
        public void GetEdyBalance_1()
        {
            var actual = FelicaHelper.GetEdyBalance();
            Assert.AreEqual(12345, actual);
        }

        [TestMethod]
        public void GetWaonBalance_1()
        {
            var actual = FelicaHelper.GetWaonBalance();
            Assert.AreEqual(12345, actual);
        }

        [TestMethod]
        public void GetSuicaBalance_1()
        {
            var actual = FelicaHelper.GetSuicaBalance();
            Assert.AreEqual(12345, actual);
        }
    }
}
