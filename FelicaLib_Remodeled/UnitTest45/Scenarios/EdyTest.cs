using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTest.Scenarios
{
    /// <summary>
    /// Edy に接続できる場合のテストです。
    /// </summary>
    [TestClass]
    public class EdyTest
    {
        [TestMethod]
        public void Felica_TryConnectionToPort()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Assert.AreEqual(true, felica.TryConnectionToPort());
            }
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Assert.AreEqual(true, felica.TryConnectionToPort());
            }
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Assert.AreEqual(true, felica.TryConnectionToPort());
            }
        }

        [TestMethod]
        public void Felica_TryConnectionToCard()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Assert.AreEqual(true, felica.TryConnectionToCard());
            }
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Assert.AreEqual(true, felica.TryConnectionToCard());
            }
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Assert.AreEqual(false, felica.TryConnectionToCard());
            }
        }

        [TestMethod]
        public void Felica_GetIDm_Any()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Debug.WriteLine(felica.GetIDm().ToHexString());
                Debug.WriteLine(felica.GetPMm().ToHexString());
            }
        }

        [TestMethod]
        public void Felica_GetIDm_Edy()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Debug.WriteLine(felica.GetIDm().ToHexString());
                Debug.WriteLine(felica.GetPMm().ToHexString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_GetIDm_Suica()
        {
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Debug.WriteLine(felica.GetIDm().ToHexString());
                Debug.WriteLine(felica.GetPMm().ToHexString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_ReadWithoutEncryption_Any()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(0x1317, 0).ToEdyBalance());
            }
        }

        [TestMethod]
        public void Felica_ReadWithoutEncryption_Edy()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(0x1317, 0).ToEdyBalance());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_ReadWithoutEncryption_Waon()
        {
            using (var felica = new Felica(FelicaSystemCode.Waon))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(0x6817, 0).ToWaonBalance());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_ReadWithoutEncryption_Suica()
        {
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(0x1317, 0).ToEdyBalance());
            }
        }

        [TestMethod]
        public void FelicaUtility_TryConnectionToPort()
        {
            Assert.AreEqual(true, FelicaUtility.TryConnectionToPort());
        }

        [TestMethod]
        public void FelicaUtility_TryConnectionToCard()
        {
            Assert.AreEqual(true, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Any));
            Assert.AreEqual(true, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Edy));
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Suica));
        }

        [TestMethod]
        public void FelicaUtility_GetIDm_Any()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Any).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Any).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_GetIDm_Edy()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Edy).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Edy).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_ReadWithoutEncryption_Edy()
        {
            Debug.WriteLine(FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Edy, 0x1317, 0).ToEdyBalance());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaUtility_ReadWithoutEncryption_Waon()
        {
            Debug.WriteLine(FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Waon, 0x6817, 0).ToWaonBalance());
        }

        [TestMethod]
        public void FelicaHelper_GetEdyBalance()
        {
            Debug.WriteLine(FelicaHelper.GetEdyBalance());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaHelper_GetWaonBalance()
        {
            Debug.WriteLine(FelicaHelper.GetWaonBalance());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaHelper_GetSuicaBalance()
        {
            Debug.WriteLine(FelicaHelper.GetSuicaBalance());
        }
    }
}
