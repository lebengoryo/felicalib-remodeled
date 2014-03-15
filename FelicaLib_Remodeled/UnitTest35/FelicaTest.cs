using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace UnitTest35
{
    [TestClass]
    public class FelicaTest
    {
        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            var target = FelicaUtility.GetEdyBalance();
            Assert.AreEqual(12345, target);
        }
    }
}
