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
        // Edy 残高
        const int SystemCode = 0xFE00;
        const int ServiceCode = 0x1317;
        const int Address = 0;
        static readonly Func<byte[], object> ToSemanticData = b => Enumerable.Range(0, 4).Select(i => b[i] * (int)Math.Pow(256, i)).Sum();
        const int Expected = 12345;

        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            var target = ReadData();
            Assert.AreEqual(Expected, target);
        }

        static object ReadData()
        {
            using (var felica = new Felica())
            {
                felica.Polling(SystemCode);
                var data = felica.ReadWithoutEncryption(ServiceCode, Address);
                return ToSemanticData(data);
            }
        }
    }
}
