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
                            Console.WriteLine(ToEdyBalance(data));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            });

            Thread.Sleep(15000);
            //Console.WriteLine("Press [Enter] key to exit.");
            //Console.ReadLine();
        }

        public static string ToHexString(this byte[] data)
        {
            return string.Concat(data.Select(b => b.ToString("X2")));
        }

        static readonly Func<byte[], int> ToEdyBalance = b => b
            .Take(4)
            .Select((v, i) => v * (int)Math.Pow(256, i))
            .Sum();
    }
}
