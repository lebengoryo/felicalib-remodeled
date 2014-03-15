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
            var target = GetEdyBalance();
            Assert.AreEqual(12345, target);
        }

        static int GetEdyBalance()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var data = felica.ReadWithoutEncryption(0x1317, 0);
                return data.ToEdyBalance();
            }
        }
    }

    public static class FelicaHelper
    {
        public static string ToHexString(this byte[] data)
        {
            return string.Concat(data.Select(b => b.ToString("X2")));
        }

        public static int ToEdyBalance(this byte[] data)
        {
            return data
                .Take(4)
                .Select((b, i) => b * (int)Math.Pow(256, i))
                .Sum();
        }
    }
}
