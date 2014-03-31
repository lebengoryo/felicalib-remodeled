using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace UnitTest45
{
    [TestClass]
    public class FelicaWatcherTest
    {
        [TestMethod]
        public void FelicaWatcher_1()
        {
            using (var watcher = new FelicaWatcher(FelicaSystemCode.Edy))
            {
                watcher.CardArrived += f =>
                {
                    Debug.WriteLine(FelicaHelper.GetEdyBalance());
                };
                watcher.CardDeparted += f =>
                {
                    Debug.WriteLine("Card Departed");
                };

                Thread.Sleep(10000);
            }
        }
    }
}
