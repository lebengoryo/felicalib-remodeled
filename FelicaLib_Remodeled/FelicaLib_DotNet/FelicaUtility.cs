using System;
using System.Collections.Generic;
using System.Linq;

namespace FelicaLib
{
    /// <summary>
    /// FeliCa のシステム コードを表します。
    /// </summary>
    public enum FelicaSystemCode
    {
        /// <summary>すべて。</summary>
        Any = 0xFFFF,
        /// <summary>共通領域。</summary>
        Common = 0xFE00,
        /// <summary>サイバネ領域。</summary>
        Cybernetics = 0x0003,

        /// <summary>Edy。共通領域を使用します。</summary>
        Edy = Common,
        /// <summary>Suica。サイバネ領域を使用します。</summary>
        Suica = Cybernetics,
        /// <summary>QUICPay。</summary>
        QuicPay = 0x04C1,
    }

    public static class FelicaUtility
    {
        public static byte[] GetIDm(int systemCode)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.GetIDm();
            }
        }

        public static byte[] GetIDm(FelicaSystemCode systemCode)
        {
            return GetIDm((int)systemCode);
        }

        public static byte[] GetPMm(int systemCode)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.GetPMm();
            }
        }

        public static byte[] GetPMm(FelicaSystemCode systemCode)
        {
            return GetPMm((int)systemCode);
        }

        public static int GetEdyBalance()
        {
            using (var felica = new Felica(FelicaSystemCode.Edy))
            {
                var data = felica.ReadWithoutEncryption(0x1317, 0);
                return data.ToEdyBalance();
            }
        }

        public static int GetSuicaBalance()
        {
            using (var felica = new Felica(FelicaSystemCode.Suica))
            {
                // 20 件の利用履歴が保存されており、address の値の範囲は 0 ～ 19 です。
                var data = felica.ReadWithoutEncryption(0x090F, 0);
                return data.ToSuicaBalance();
            }
        }
    }

    public static class FelicaHelper
    {
        public static int ToEdyBalance(this byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return data
                .Take(4)
                .Select((b, i) => b * (int)Math.Pow(256, i))
                .Sum();
        }

        public static int ToSuicaBalance(this byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return data
                .Skip(10)
                .Take(2)
                .Select((b, i) => b * (int)Math.Pow(256, i))
                .Sum();
        }

        public static string ToHexString(this byte[] data, bool lowercase = false)
        {
            if (data == null) throw new ArgumentNullException("data");
            var format = lowercase ? "x2" : "X2";
            return data
                .Select(b => b.ToString(format))
                .ConcatStrings();
        }

        static string ConcatStrings(this IEnumerable<string> source)
        {
            return string.Concat(source.ToArray());
        }
    }
}
