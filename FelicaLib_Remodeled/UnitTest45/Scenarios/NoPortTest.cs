using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTest.Scenarios
{
    /// <summary>
    /// FeliCa ポートが接続されていない場合のテストです。
    /// </summary>
    [TestClass]
    public class NoPortTest
    {
        [TestMethod]
        public void Felica_TryConnectionToPort()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Assert.AreEqual(false, felica.TryConnectionToPort());
            }
        }

        [TestMethod]
        public void Felica_TryConnectionToCard()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Assert.AreEqual(false, felica.TryConnectionToCard());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_GetIDm()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Debug.WriteLine(felica.GetIDm().ToHexString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_ReadWithoutEncryption()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(FelicaServiceCode.EdyBalance, 0).ToEdyBalance());
            }
        }

        [TestMethod]
        public void FelicaUtility_TryConnectionToPort()
        {
            Assert.AreEqual(false, FelicaUtility.TryConnectionToPort());
        }

        [TestMethod]
        public void FelicaUtility_TryConnectionToCard()
        {
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Any));
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Edy));
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Suica));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaUtility_GetIDm()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Any).ToHexString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaHelper_GetEdyBalance()
        {
            Debug.WriteLine(FelicaHelper.GetEdyBalance());
        }
    }
}
