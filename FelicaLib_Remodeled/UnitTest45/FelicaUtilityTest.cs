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
        public void ReadWithoutEncryption_1()
        {
            var target = FelicaUtility.ReadWithoutEncryption(0xFE00, 0x1317, 0);
            Assert.AreEqual(12345, target.ToEdyBalance());
        }

        [TestMethod]
        public void ReadWithoutEncryption_2()
        {
            var target = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Edy, 0x1317, 0);
            Assert.AreEqual(12345, target.ToEdyBalance());
        }
    }
}
