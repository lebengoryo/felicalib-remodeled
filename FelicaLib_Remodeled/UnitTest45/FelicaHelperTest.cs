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
            var target = FelicaHelper.GetEdyBalance();
            Assert.AreEqual(12345, target);
        }

        [TestMethod]
        public void GetWaonBalance_1()
        {
            var target = FelicaHelper.GetWaonBalance();
            Assert.AreEqual(12345, target);
        }

        [TestMethod]
        public void GetSuicaBalance_1()
        {
            var target = FelicaHelper.GetSuicaBalance();
            Assert.AreEqual(12345, target);
        }
    }
}
