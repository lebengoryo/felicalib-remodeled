using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FelicaLib
{
    /// <summary>
    /// IC カードとの接続を待機します。
    /// </summary>
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

        /// <summary>
        /// IC カードが通信の範囲に入ったときに発生します。
        /// </summary>
        public event Action<Felica> CardArrived = f => { };

        /// <summary>
        /// IC カードが通信の範囲から離れたときに発生します。
        /// </summary>
        public event Action<Felica> CardDeparted = f => { };

        /// <summary>
        /// エラーが発生したときに発生します。
        /// </summary>
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

        #region IDisposable メンバー

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        ~FelicaWatcher()
        {
            Dispose(false);
        }

        /// <summary>
        /// このオブジェクトで使用されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このオブジェクトで使用されているリソースを解放します。
        /// </summary>
        /// <param name="disposing">すべてのリソースを解放する場合は <see langword="true"/>。アンマネージ リソースのみを解放する場合は <see langword="false"/>。</param>
        protected virtual void Dispose(bool disposing)
        {
            isStopped = true;

            if (disposing)
            {
                felica.Dispose();
            }
        }

        #endregion
    }
}
