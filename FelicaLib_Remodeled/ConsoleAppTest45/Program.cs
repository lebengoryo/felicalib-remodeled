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
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("Start");
                    ReadEdyBalanceEtc();
                }
            });

            Thread.Sleep(15000);
            //Console.WriteLine("Press [Enter] key to exit.");
            //Console.ReadLine();
        }

        static void ReadEdyBalanceEtc()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
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
