using System;
using System.Collections.Generic;
using System.Linq;

namespace FelicaLib
{
    /// <summary>
    /// FeliCa を通じて IC カードからデータを読み取るための静的メソッドを提供します。
    /// </summary>
    public static class FelicaUtility
    {
        /// <summary>
        /// FeliCa ポートに接続できるかどうかを確認します。
        /// </summary>
        /// <returns>FeliCa ポートに接続できる場合は <see langword="true"/>。</returns>
        public static bool TryConnectionToPort()
        {
            using (var felica = new Felica(FelicaSystemCode.Any))
            {
                return felica.TryConnectionToPort();
            }
        }

        /// <summary>
        /// IC カードに接続できるかどうかを確認します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>IC カードに接続できる場合は <see langword="true"/>。</returns>
        public static bool TryConnectionToCard(int systemCode)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.TryConnectionToCard();
            }
        }

        /// <summary>
        /// IC カードに接続できるかどうかを確認します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>IC カードに接続できる場合は <see langword="true"/>。</returns>
        public static bool TryConnectionToCard(FelicaSystemCode systemCode)
        {
            return TryConnectionToCard((int)systemCode);
        }

        /// <summary>
        /// 製造 ID (IDm) を取得します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>製造 ID (IDm)。配列の長さは 8 です。</returns>
        public static byte[] GetIDm(int systemCode)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.GetIDm();
            }
        }

        /// <summary>
        /// 製造 ID (IDm) を取得します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>製造 ID (IDm)。配列の長さは 8 です。</returns>
        public static byte[] GetIDm(FelicaSystemCode systemCode)
        {
            return GetIDm((int)systemCode);
        }

        /// <summary>
        /// 製造パラメータ (PMm) を取得します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>製造パラメータ (PMm)。配列の長さは 8 です。</returns>
        public static byte[] GetPMm(int systemCode)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.GetPMm();
            }
        }

        /// <summary>
        /// 製造パラメータ (PMm) を取得します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <returns>製造パラメータ (PMm)。配列の長さは 8 です。</returns>
        public static byte[] GetPMm(FelicaSystemCode systemCode)
        {
            return GetPMm((int)systemCode);
        }

        /// <summary>
        /// 非暗号化領域のデータを読み込みます。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <param name="serviceCode">サービス コード。</param>
        /// <param name="address">アドレス。</param>
        /// <returns>非暗号化領域のデータ。配列の長さは 16 です。</returns>
        public static byte[] ReadWithoutEncryption(int systemCode, int serviceCode, int address)
        {
            using (var felica = new Felica(systemCode))
            {
                return felica.ReadWithoutEncryption(serviceCode, address);
            }
        }

        /// <summary>
        /// 非暗号化領域のデータを読み込みます。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        /// <param name="serviceCode">サービス コード。</param>
        /// <param name="address">アドレス。</param>
        /// <returns>非暗号化領域のデータ。配列の長さは 16 です。</returns>
        public static byte[] ReadWithoutEncryption(FelicaSystemCode systemCode, int serviceCode, int address)
        {
            return ReadWithoutEncryption((int)systemCode, serviceCode, address);
        }
    }
}
