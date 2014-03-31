using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FelicaLib
{
    public class FelicaWatcher : IDisposable
    {
        const int DefaultInterval = 500;

        Felica felica;
        bool isStopped;

        /// <summary>
        /// システム コードを取得します。
        /// </summary>
        /// <value>システム コード。</value>
        public int SystemCode { get { return felica.SystemCode; } }

        /// <summary>
        /// IC カードとの接続を確認する間隔を取得または設定します。
        /// </summary>
        /// <value>IC カードとの接続を確認する間隔。</value>
        public int Interval { get; set; }

        /// <summary>
        /// <see cref="FelicaWatcher"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        public FelicaWatcher(int systemCode)
        {
            felica = new Felica(systemCode);
            Interval = DefaultInterval;

            new Action(WatchCard).BeginInvoke(null, null);
        }

        public event Action<Felica> CardArrived = f => { };
        public event Action<Felica> CardDeparted = f => { };
        public event Action<Felica, Exception> Error = (f, ex) => { };

        void WatchCard()
        {
            var isCardConnected_old = false;
            var isCardConnected = false;

            while (!isStopped)
            {
                try
                {
                    isCardConnected_old = isCardConnected;
                    isCardConnected = felica.TryConnectionToCard();

                    if (!isCardConnected_old && isCardConnected)
                    {
                        CardArrived(felica);
                    }
                    else if (isCardConnected_old && !isCardConnected)
                    {
                        CardDeparted(felica);
                    }
                }
                catch (Exception ex)
                {
                    Error(felica, ex);
                }

                if (Interval > 0)
                {
                    Thread.Sleep(Interval);
                }
            }
        }

        // Felica クラスはスレッド セーフではないため、現在は使えません。
        static void Synchronize(Action action)
        {
            var context = SynchronizationContext.Current;
            if (context != null)
            {
                SynchronizationContext.Current.Post(o => action(), null);
            }
            else
            {
                action.BeginInvoke(null, null);
            }
        }

        ~FelicaWatcher()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            isStopped = true;

            if (disposing)
            {
                felica.Dispose();
            }
        }
    }
}
