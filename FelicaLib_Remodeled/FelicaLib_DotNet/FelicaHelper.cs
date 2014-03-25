using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>WAON。共通領域を使用します。</summary>
        Waon = Common,
        /// <summary>Suica。サイバネ領域を使用します。</summary>
        Suica = Cybernetics,
        /// <summary>QUICPay。</summary>
        QuicPay = 0x04C1,
    }

    /// <summary>
    /// FeliCa のサービス コードを表します。
    /// </summary>
    public static class FelicaServiceCode
    {
        /// <summary>Edy の残高情報。</summary>
        public const int EdyBalance = 0x1317;
        /// <summary>Edy の履歴情報。</summary>
        public const int EdyHistory = 0x170F;
        /// <summary>WAON の残高情報。</summary>
        public const int WaonBalance = 0x6817;
        /// <summary>WAON の履歴情報。</summary>
        public const int WaonHistory = 0x680B;
        /// <summary>Suica の属性情報。</summary>
        public const int SuicaAttributes = 0x008B;
        /// <summary>Suica の履歴情報。</summary>
        public const int SuicaHistory = 0x090F;
    }

    public abstract class FelicaBlockItem
    {
        public byte[] RawData { get; private set; }
        protected FelicaBlockItem(byte[] data)
        {
            RawData = data;
        }
    }

    /// <summary>
    /// Edy の残高情報を表します。
    /// </summary>
    [DebuggerDisplay(@"\{Balance: {Balance}\}")]
    public class EdyBalanceItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="EdyBalanceItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public EdyBalanceItem(byte[] data) : base(data) { }

        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
        public int Balance { get { return RawData.ToInt32(0, 4, true); } }
    }

    [DebuggerDisplay(@"\{ID: {TransactionId}, {DateTime}\}")]
    public class EdyHistoryItem : FelicaBlockItem
    {
        public EdyHistoryItem(byte[] data) : base(data) { }

        static readonly DateTime BaseDateTime = new DateTime(2000, 1, 1);

        public DateTime DateTime
        {
            get
            {
                var days = RawData.ToInt32(4, 2) >> 1;
                var seconds = RawData.ToInt32(5, 3) & 0x01FFFF;
                return BaseDateTime.AddDays(days).AddSeconds(seconds);
            }
        }
        public int UsageCode { get { return RawData.ToInt32(0, 1); } }
        public int TransactionId { get { return RawData.ToInt32(2, 2); } }
        public int Amount { get { return RawData.ToInt32(8, 4); } }
        public int Balance { get { return RawData.ToInt32(12, 4); } }
    }

    /// <summary>
    /// WAON の残高情報を表します。
    /// </summary>
    [DebuggerDisplay(@"\{Balance: {Balance}\}")]
    public class WaonBalanceItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="WaonBalanceItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public WaonBalanceItem(byte[] data) : base(data) { }

        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
        public int Balance { get { return RawData.ToInt32(0, 2, true); } }
    }

    /// <summary>
    /// Suica の属性情報を表します。
    /// </summary>
    [DebuggerDisplay(@"\{Balance: {Balance}\}")]
    public class SuicaAttributesItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="SuicaAttributesItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public SuicaAttributesItem(byte[] data) : base(data) { }

        public int CardCode { get { return RawData.ToInt32(8, 1) >> 4; } }
        public int AreaCode { get { return RawData.ToInt32(8, 1) & 0x0F; } }
        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
        public int Balance { get { return RawData.ToInt32(11, 2, true); } }
        public int TransactionId { get { return RawData.ToInt32(14, 2); } }
    }

    [DebuggerDisplay(@"\{ID: {TransactionId}, {DateTime}\}")]
    public class SuicaHistoryItem : FelicaBlockItem
    {
        public SuicaHistoryItem(byte[] data) : base(data) { }

        public DateTime DateTime
        {
            get
            {
                var year = 2000 + (RawData.ToInt32(4, 1) >> 1);
                var month = RawData.ToInt32(4, 2) >> 5 & 0x000F;
                var day = RawData.ToInt32(5, 1) & 0x1F;
                return new DateTime(year, month, day);
            }
        }
        public int DeviceCode { get { return RawData.ToInt32(0, 1); } }
        public int UsageCode { get { return RawData.ToInt32(1, 1); } }
        public int PaymentCode { get { return RawData.ToInt32(2, 1); } }
        public int EntryCode { get { return RawData.ToInt32(3, 1); } }
        public int Balance { get { return RawData.ToInt32(10, 2, true); } }
        public int TransactionId { get { return RawData.ToInt32(13, 2); } }
    }

    /// <summary>
    /// FeliCa に関するヘルパー メソッドを提供します。
    /// </summary>
    public static class FelicaHelper
    {
        /// <summary>
        /// Edy の残高を取得します。
        /// </summary>
        /// <returns>Edy の残高。</returns>
        public static int GetEdyBalance()
        {
            var data = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Edy, FelicaServiceCode.EdyBalance, 0);
            var item = new EdyBalanceItem(data);
            return item.Balance;
        }

        /// <summary>
        /// Edy の利用履歴を取得します。
        /// </summary>
        /// <returns>Edy の利用履歴。</returns>
        public static IEnumerable<EdyHistoryItem> GetEdyHistory()
        {
            var data = FelicaUtility.ReadBlocksWithoutEncryption(FelicaSystemCode.Edy, FelicaServiceCode.EdyHistory, 0, 6);
            return data.Select(x => new EdyHistoryItem(x));
        }

        /// <summary>
        /// WAON の残高を取得します。
        /// </summary>
        /// <returns>WAON の残高。</returns>
        public static int GetWaonBalance()
        {
            var data = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Waon, FelicaServiceCode.WaonBalance, 0);
            var item = new WaonBalanceItem(data);
            return item.Balance;
        }

        /// <summary>
        /// Suica の残高を取得します。PASMO などの交通系 IC カードと互換性があります。
        /// </summary>
        /// <returns>Suica の残高。</returns>
        public static int GetSuicaBalance()
        {
            var data = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Suica, FelicaServiceCode.SuicaAttributes, 0);
            var item = new SuicaAttributesItem(data);
            return item.Balance;
        }

        /// <summary>
        /// Suica の利用履歴を取得します。
        /// </summary>
        /// <returns>Suica の利用履歴。</returns>
        public static IEnumerable<SuicaHistoryItem> GetSuicaHistory()
        {
            var data = FelicaUtility.ReadBlocksWithoutEncryption(FelicaSystemCode.Suica, FelicaServiceCode.SuicaHistory, 0, 20);
            return data.Select(x => new SuicaHistoryItem(x));
        }

        /// <summary>
        /// バイト配列から指定した部分の要素を数値に変換します。
        /// </summary>
        /// <param name="data">バイト配列。</param>
        /// <param name="start">開始インデックス。</param>
        /// <param name="count">要素の数。</param>
        /// <returns>数値。</returns>
        public static int ToInt32(this byte[] data, int start, int count)
        {
            return ToInt32(data, start, count, false);
        }

        /// <summary>
        /// バイト配列から指定した部分の要素を数値に変換します。
        /// </summary>
        /// <param name="data">バイト配列。</param>
        /// <param name="start">開始インデックス。</param>
        /// <param name="count">要素の数。</param>
        /// <param name="littleEndian">リトル エンディアンの場合は <see langword="true"/>。</param>
        /// <returns>数値。</returns>
        public static int ToInt32(this byte[] data, int start, int count, bool littleEndian)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (count < 0 || 4 < count) throw new ArgumentOutOfRangeException("count", "最大で 4 バイトです。");
            if (start < 0 || data.Length < start + count) throw new IndexOutOfRangeException("指定されたインデックスが範囲外です。");

            return data
                .Skip(start)
                .Take(count)
                .Select((b, i) => b * (int)Math.Pow(256, littleEndian ? i : count - 1 - i))
                .Sum();
        }

        /// <summary>
        /// バイト配列を 16 進数表記の文字列に変換します。
        /// </summary>
        /// <param name="data">バイト配列。</param>
        /// <returns>16 進数表記の文字列。</returns>
        public static string ToHexString(this byte[] data)
        {
            return ToHexString(data, false);
        }

        /// <summary>
        /// バイト配列を 16 進数表記の文字列に変換します。
        /// </summary>
        /// <param name="data">バイト配列。</param>
        /// <param name="lowercase">アルファベットを小文字で表記する場合は <see langword="true"/>。</param>
        /// <returns>16 進数表記の文字列。</returns>
        public static string ToHexString(this byte[] data, bool lowercase)
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
