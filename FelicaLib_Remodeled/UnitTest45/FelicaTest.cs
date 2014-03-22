using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest45
{
    [TestClass]
    public class FelicaTest
    {
        [TestMethod]
        public void TryConnectionToPort_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var actual = felica.TryConnectionToPort();
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void TryConnectionToCard_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var actual = felica.TryConnectionToCard();
                Assert.IsTrue(actual);
            }
        }

        [TestMethod]
        public void GetIDm_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var actual = felica.GetIDm();
                Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
            }
        }

        [TestMethod]
        public void GetPMm_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var actual = felica.GetPMm();
                Assert.AreEqual("0123456789ABCDEF", actual.ToHexString());
            }
        }

        [TestMethod]
        public void ReadWithoutEncryption_1()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var actual = felica.ReadWithoutEncryption(0x1317, 0);
                Assert.AreEqual(12345, actual.ToEdyBalance());
            }
        }

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
                            var data = felica.ReadWithoutEncryption(0x1317, 0);
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

                var data = felica.ReadWithoutEncryption(0x1317, 0);
                Console.WriteLine(data.ToEdyBalance());
            }
        }
    }
}
