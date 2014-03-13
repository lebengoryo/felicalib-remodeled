using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest45
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

        [TestMethod]
        public void ReadWithoutEncryption_ManyTimes()
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        Console.WriteLine("Start");
                        Console.WriteLine(ReadData());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            Thread.Sleep(10000);
        }

        static object ReadData()
        {
            using (var felica = new Felica(SystemCode))
            {
                var data = felica.ReadWithoutEncryption(ServiceCode, Address);
                return ToSemanticData(data);
            }
        }
    }
}
