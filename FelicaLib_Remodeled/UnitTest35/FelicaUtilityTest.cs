using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest35
{
    [TestClass]
    public class FelicaUtilityTest
    {
        [TestMethod]
        public void GetIDm_1()
        {
            var actual = FelicaUtility.GetIDm(0xFE00);
            Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
        }

        [TestMethod]
        public void GetIDm_2()
        {
            var actual = FelicaUtility.GetIDm(FelicaSystemCode.Edy);
            Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
        }

        [TestMethod]
        public void GetPMm_1()
        {
            var actual = FelicaUtility.GetPMm(0xFE00);
            Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
        }

        [TestMethod]
        public void GetPMm_2()
        {
            var actual = FelicaUtility.GetPMm(FelicaSystemCode.Edy);
            Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
        }

        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            var actual = FelicaUtility.ReadWithoutEncryption(0xFE00, 0x1317, 0);
            Assert.AreEqual(12345, actual.ToEdyBalance());
        }

        [TestMethod]
        public void ReadWithoutEncryption_2()
        {
            var actual = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Edy, 0x1317, 0);
            Assert.AreEqual(12345, actual.ToEdyBalance());
        }
    }
}
