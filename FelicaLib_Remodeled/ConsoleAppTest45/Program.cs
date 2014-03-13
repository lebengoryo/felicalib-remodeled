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
                    try
                    {
                        Console.WriteLine("Start");

                        using (var felica = new Felica(FelicaSystemCode.Edy))
                        {
                            Console.WriteLine(felica.IDm().ToHexString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            Thread.Sleep(10000);
            //Console.WriteLine("Press [Enter] key to exit.");
            //Console.ReadLine();
        }

        public static string ToHexString(this byte[] data)
        {
            return string.Concat(data.Select(b => b.ToString("X2")));
        }
    }
}
