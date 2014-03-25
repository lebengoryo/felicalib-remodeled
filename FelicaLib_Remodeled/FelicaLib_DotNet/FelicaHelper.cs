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

    /// <summary>
    /// FeliCa のブロックを表すための基本クラスです。
    /// </summary>
    public abstract class FelicaBlockItem
    {
        /// <summary>元のバイナリ データを取得します。</summary>
        /// <value>元のバイナリ データ。</value>
        public byte[] RawData { get; private set; }

        /// <summary>
        /// <see cref="FelicaBlockItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
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

    /// <summary>
    /// Edy の履歴情報を表します。
    /// </summary>
    [DebuggerDisplay(@"\{ID: {TransactionId}, {DateTime}\}")]
    public class EdyHistoryItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="EdyHistoryItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public EdyHistoryItem(byte[] data) : base(data) { }

        static readonly DateTime BaseDateTime = new DateTime(2000, 1, 1);

        /// <summary>利用日時を取得します。</summary>
        /// <value>利用日時。</value>
        public DateTime DateTime
        {
            get
            {
                var days = RawData.ToInt32(4, 2) >> 1;
                var seconds = RawData.ToInt32(5, 3) & 0x01FFFF;
                return BaseDateTime.AddDays(days).AddSeconds(seconds);
            }
        }

        /// <summary>利用種別を取得します。</summary>
        /// <value>利用種別。</value>
        public int UsageCode { get { return RawData.ToInt32(0, 1); } }
        /// <summary>取引通番を取得します。</summary>
        /// <value>取引通番。</value>
        public int TransactionId { get { return RawData.ToInt32(2, 2); } }
        /// <summary>利用額を取得します。</summary>
        /// <value>利用額。</value>
        public int Amount { get { return RawData.ToInt32(8, 4); } }
        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
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
    /// PASMO などの交通系 IC カードと互換性があります。
    /// </summary>
    [DebuggerDisplay(@"\{Balance: {Balance}\}")]
    public class SuicaAttributesItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="SuicaAttributesItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public SuicaAttributesItem(byte[] data) : base(data) { }

        /// <summary>カード種別を取得します。</summary>
        /// <value>カード種別。</value>
        public int CardCode { get { return RawData.ToInt32(8, 1) >> 4; } }
        /// <summary>地域を取得します。</summary>
        /// <value>地域。</value>
        public int AreaCode { get { return RawData.ToInt32(8, 1) & 0x0F; } }
        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
        public int Balance { get { return RawData.ToInt32(11, 2, true); } }
        /// <summary>取引通番を取得します。</summary>
        /// <value>取引通番。</value>
        public int TransactionId { get { return RawData.ToInt32(14, 2); } }
    }

    /// <summary>
    /// Suica の履歴情報を表します。
    /// PASMO などの交通系 IC カードと互換性があります。
    /// </summary>
    [DebuggerDisplay(@"\{ID: {TransactionId}, {DateTime}\}")]
    public class SuicaHistoryItem : FelicaBlockItem
    {
        /// <summary>
        /// <see cref="SuicaHistoryItem"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">バイナリ データ。</param>
        public SuicaHistoryItem(byte[] data) : base(data) { }

        /// <summary>利用日付を取得します。</summary>
        /// <value>利用日付。</value>
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

        /// <summary>機器種別を取得します。</summary>
        /// <value>機器種別。</value>
        public int DeviceCode { get { return RawData.ToInt32(0, 1); } }
        /// <summary>利用種別を取得します。</summary>
        /// <value>利用種別。</value>
        public int UsageCode { get { return RawData.ToInt32(1, 1); } }
        /// <summary>支払種別を取得します。</summary>
        /// <value>支払種別。</value>
        public int PaymentCode { get { return RawData.ToInt32(2, 1); } }
        /// <summary>入出場種別を取得します。</summary>
        /// <value>入出場種別。</value>
        public int EntryCode { get { return RawData.ToInt32(3, 1); } }
        /// <summary>残高を取得します。</summary>
        /// <value>残高。</value>
        public int Balance { get { return RawData.ToInt32(10, 2, true); } }
        /// <summary>取引通番を取得します。</summary>
        /// <value>取引通番。</value>
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
        /// Suica の残高を取得します。
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
