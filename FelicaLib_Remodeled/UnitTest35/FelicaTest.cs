using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTest35
{
    [TestClass]
    public class FelicaTest
    {
        [TestMethod]
        public void GetIDm_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var target = felica.GetIDm();
                Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
            }
        }

        [TestMethod]
        public void GetPMm_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var target = felica.GetPMm();
                Assert.AreEqual("0123456789ABCDEF", target.ToHexString());
            }
        }

        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var target = felica.ReadWithoutEncryption(0x1317, 0);
                Assert.AreEqual(12345, target.ToEdyBalance());
            }
        }
    }
}
