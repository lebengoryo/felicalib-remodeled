using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTest45.Scenarios
{
    /// <summary>
    /// WAON に接続できる場合のテストです。
    /// </summary>
    [TestClass]
    public class WaonTest
    {
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
            Assert.AreEqual(true, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Waon));
            Assert.AreEqual(false, FelicaUtility.TryConnectionToCard(FelicaSystemCode.Suica));
        }

        [TestMethod]
        public void FelicaUtility_GetIDm()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Any).ToHexString());
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Edy).ToHexString());
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Waon).ToHexString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaUtility_GetIDm_Suica()
        {
            Debug.WriteLine(FelicaUtility.GetIDm(FelicaSystemCode.Suica).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_GetPMm()
        {
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Any).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Edy).ToHexString());
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Waon).ToHexString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FelicaUtility_GetPMm_Suica()
        {
            Debug.WriteLine(FelicaUtility.GetPMm(FelicaSystemCode.Suica).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_ReadWithoutEncryption()
        {
            Debug.WriteLine(FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Waon, FelicaServiceCode.WaonBalance, 0).ToHexString());
        }

        [TestMethod]
        public void FelicaUtility_ReadBlocksWithoutEncryption()
        {
            foreach (var data in FelicaUtility.ReadBlocksWithoutEncryption(FelicaSystemCode.Waon, FelicaServiceCode.WaonHistory, 0, 9))
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
        public void FelicaHelper_GetWaonBalance()
        {
            Debug.WriteLine(FelicaHelper.GetWaonBalance());
        }
    }
}
