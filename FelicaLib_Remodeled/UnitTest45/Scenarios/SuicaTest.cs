using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTest.Scenarios
{
    /// <summary>
    /// Suica に接続できる場合のテストです。
    /// </summary>
    [TestClass]
    public class SuicaTest
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
                Assert.AreEqual(false, felica.TryConnectionToCard());
            }
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Assert.AreEqual(true, felica.TryConnectionToCard());
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void Felica_GetIDm_Edy()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Debug.WriteLine(felica.GetIDm().ToHexString());
                Debug.WriteLine(felica.GetPMm().ToHexString());
            }
        }

        [TestMethod]
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
                Debug.WriteLine(felica.ReadWithoutEncryption(0x008B, 0).ToEdyBalance());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
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
        public void Felica_ReadWithoutEncryption_Suica()
        {
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                Debug.WriteLine(felica.ReadWithoutEncryption(0x008B, 0).ToSuicaBalance());
            }
        }

        [TestMethod]
        public void Felica_ReadBlocksWithoutEncryption_Any()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                foreach (var data in felica.ReadBlocksWithoutEncryption(0x090F, 0, 20))
                {
                    Debug.WriteLine(data.ToHexString());
                }
            }
        }

        [TestMethod]
        public void Felica_ReadBlocksWithoutEncryption_Suica()
        {
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                foreach (var data in felica.ReadBlocksWithoutEncryption(0x090F, 0, 20))
                {
                    Debug.WriteLine(data.ToHexString());
                }
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
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Edy));
            Assert.AreEqual(true, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Suica));
        }

        [TestMethod]
        public void FelicaUtility_GetIDm_Any()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Any).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Any).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_GetIDm_Suica()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Suica).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Suica).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_ReadWithoutEncryption_Suica()
        {
            Debug.WriteLine(FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Suica, 0x008B, 0).ToSuicaBalance());
        }

        [TestMethod]
        public void FelicaUtility_ReadBlocksWithoutEncryption_Suica()
        {
            foreach (var data in FelicaUtility.ReadBlocksWithoutEncryption(FelicaSystemCode.Suica, 0x090F, 0, 20))
            {
                Debug.WriteLine(data.ToHexString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
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
        public void FelicaHelper_GetSuicaBalance()
        {
            Debug.WriteLine(FelicaHelper.GetSuicaBalance());
        }
    }
}
