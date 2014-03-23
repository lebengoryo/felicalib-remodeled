using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest45
{
    [TestClass]
    public class FelicaLoadTest
    {
        [TestMethod]
        public void ReadWithoutEncryption_ManyTimes1()
        {
            Task.Run(() =>
            {
                using (var felica = new Felica(FelicaSystemCode.Edy))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        try
                        {
                            var data = felica.ReadWithoutEncryption(FelicaServiceCode.EdyBalance, 0);
                            Console.WriteLine(data.ToEdyBalance());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            });

            Thread.Sleep(10000);
        }

        [TestMethod]
        public void ReadWithoutEncryption_ManyTimes2()
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

                var data = felica.ReadWithoutEncryption(FelicaServiceCode.EdyBalance, 0);
                Console.WriteLine(data.ToEdyBalance());
            }
        }
    }
}
