using FelicaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppTest45
{
    static class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                Console.WriteLine(e.ExceptionObject);
                Console.WriteLine("Press [Enter] key to exit.");
                Console.ReadLine();
            };

            Task.Run(() => Test1());

            Thread.Sleep(15000);
            //Console.WriteLine("Press [Enter] key to exit.");
            //Console.ReadLine();
        }

        static void Test1()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("Start");

                using (var felica = new Felica(FelicaSystemCode.Edy))
                {
                    Console.WriteLine("FeliCa ポート: {0}", felica.TryConnectionToPort());
                    Console.WriteLine("IC カード: {0}", felica.TryConnectionToCard());

                    try
                    {
                        Console.WriteLine(felica.GetIDm().ToHexString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    try
                    {
                        Console.WriteLine(felica.GetPMm().ToHexString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
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
        }

        static void Test2()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                for (int i = 0; i < 1000; i++)
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
        }
    }
}
