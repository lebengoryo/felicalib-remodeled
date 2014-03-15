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
        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            var target = FelicaUtility.GetEdyBalance();
            Assert.AreEqual(12345, target);
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
                        ReadEdyBalanceEtc();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            Thread.Sleep(10000);
        }

        static void ReadEdyBalanceEtc()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                Console.WriteLine(felica.GetIDm().ToHexString());
                Console.WriteLine(felica.GetPMm().ToHexString());

                var data = felica.ReadWithoutEncryption(0x1317, 0);
                Console.WriteLine(data.ToEdyBalance());
            }
        }
    }
}
