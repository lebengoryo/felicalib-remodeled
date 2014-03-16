using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest45
{
    [TestClass]
    public class FelicaUtilityTest
    {
        [TestMethod]
        public void GetIDm_1()
        {
            var target = FelicaUtility.GetIDm(0xFE00);
            Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
        }

        [TestMethod]
        public void GetIDm_2()
        {
            var target = FelicaUtility.GetIDm(FelicaSystemCode.Edy);
            Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
        }

        [TestMethod]
        public void GetPMm_1()
        {
            var target = FelicaUtility.GetPMm(0xFE00);
            Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
        }

        [TestMethod]
        public void GetPMm_2()
        {
            var target = FelicaUtility.GetPMm(FelicaSystemCode.Edy);
            Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
        }

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
