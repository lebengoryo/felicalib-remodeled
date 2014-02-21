using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace FelicaLib
{
    // TODO: Error handling when stopped.
    public static class FeliCaPolling
    {
        public static event Action<object> DataRead = c => { };
        public static event Action<Exception> ReadError = ex => { };

        const double DefaultInterval = 1000;
        static Timer timer;

        public static double ReadInterval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        // TODO: Generics.
        public static Func<byte[], object> ToSemanticData { get; set; }
        public static int SystemCode { get; set; }
        public static int ServiceCode { get; set; }
        public static int Address { get; set; }

        static bool isReading;
        static object dataCache;

        static FeliCaPolling()
        {
            SystemCode = (int)FelicaLib.SystemCode.Any;

            timer = new Timer(DefaultInterval);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isReading) return;
            isReading = true;

            try
            {
                var dataCache_old = dataCache;
                dataCache = ReadData();
                if (dataCache != null && !object.Equals(dataCache_old, dataCache))
                {
                    DataRead(dataCache);
                }
            }
            catch (Exception ex)
            {
                dataCache = null;
                ReadError(ex);
            }
            finally
            {
                isReading = false;
            }
        }

        static object ReadData()
        {
            using (var felica = new Felica())
            {
                felica.Polling(SystemCode);

                //var id = string.Concat(felica.IDm().Select(b => b.ToString("X2")).ToArray());
                //var pm = string.Concat(felica.PMm().Select(b => b.ToString("X2")).ToArray());

                var data = felica.ReadWithoutEncryption(ServiceCode, Address);
                return ToSemanticData == null ? null : ToSemanticData(data);
            }
        }
    }
}
