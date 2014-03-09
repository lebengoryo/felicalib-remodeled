/*
 felicalib - FeliCa access wrapper library

 Copyright (c) 2007-2010, Takuya Murakami, All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are
 met:

 1. Redistributions of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer. 

 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution. 

 3. Neither the name of the project nor the names of its contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission. 

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

//
// Porting to x64 systems by DeForest(Hirokazu Hayashi)
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FelicaLib
{
    static class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    static class UnsafeNativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string lpProcName);
    }

    /// <summary>
    /// ネイティブ関数を .NET 向けに拡張します。
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/ja-jp/library/cc429019.aspx
    /// </remarks>
    static class NativeMethodsHelper
    {
        /// <summary>
        /// 指定された実行可能モジュールを、呼び出し側プロセスのアドレス空間内にマップします。
        /// </summary>
        /// <param name="fileName">モジュールのファイル名。</param>
        /// <returns>モジュールのハンドル。</returns>
        public static IntPtr LoadLibrary(string fileName)
        {
            var ptr = NativeMethods.LoadLibrary(fileName);
            if (ptr == IntPtr.Zero)
            {
                var hResult = Marshal.GetHRForLastWin32Error();
                throw Marshal.GetExceptionForHR(hResult);
            }
            return ptr;
        }

        /// <summary>
        /// ロード済みの DLL モジュールの参照カウントを 1 つ減らします。
        /// </summary>
        /// <param name="module">DLL モジュールのハンドル。</param>
        public static void FreeLibrary(IntPtr module)
        {
            var result = NativeMethods.FreeLibrary(module);
            if (!result)
            {
                var hResult = Marshal.GetHRForLastWin32Error();
                throw Marshal.GetExceptionForHR(hResult);
            }
        }

        /// <summary>
        /// DLL が持つ、指定されたエクスポート済み関数のアドレスを取得します。
        /// </summary>
        /// <param name="module">DLL モジュールのハンドル。</param>
        /// <param name="procName">関数名。</param>
        /// <returns>DLL のエクスポート済み関数のアドレス。</returns>
        public static IntPtr GetProcAddress(IntPtr module, string procName)
        {
            var ptr = UnsafeNativeMethods.GetProcAddress(module, procName);
            if (ptr == IntPtr.Zero)
            {
                var hResult = Marshal.GetHRForLastWin32Error();
                throw Marshal.GetExceptionForHR(hResult);
            }
            return ptr;
        }
    }

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

    /// <summary>
    /// FeliCa を通じて IC カードからデータを読み取るためのクラスを表します。
    /// </summary>
    public class Felica : IDisposable
    {
        // 遅延ロード用Delegate定義
        private delegate IntPtr Pasori_open(String dummy);
        private delegate int Pasori_close(IntPtr p);
        private delegate int Pasori_init(IntPtr p);
        private delegate IntPtr Felica_polling(IntPtr p, ushort systemcode, byte rfu, byte time_slot);
        private delegate void Felica_free(IntPtr f);
        private delegate void Felica_getidm(IntPtr f, byte[] data);
        private delegate void Felica_getpmm(IntPtr f, byte[] data);
        private delegate int Felica_read_without_encryption02(IntPtr f, int servicecode, int mode, byte addr, byte[] data);

        // 遅延ロード用Delegate
        private Pasori_open pasori_open = null;
        private Pasori_close pasori_close = null;
        private Pasori_init pasori_init = null;
        private Felica_polling felica_polling = null;
        private Felica_free felica_free = null;
        private Felica_getidm felica_getidm = null;
        private Felica_getpmm felica_getpmm = null;
        private Felica_read_without_encryption02 felica_read_without_encryption02 = null;

        private string szDLLname = "";
        private IntPtr _pModule;

        private IntPtr pasorip = IntPtr.Zero;
        private IntPtr felicap = IntPtr.Zero;

        /// <summary>
        /// <see cref="Felica"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public Felica()
        {
            // x64対応 20100501 - DeForest
            try
            {
                // プラットフォーム別のロードモジュール名決定（x64/x86サポート、Iteniumはサポート外）
                if (System.IntPtr.Size >= 8)    // x64
                {
                    szDLLname = "felicalib64.dll";
                }
                else                // x86
                {
                    szDLLname = "felicalib.dll";
                }
                // DLLロード
                _pModule = NativeMethodsHelper.LoadLibrary(szDLLname);
                // エントリー取得
                pasori_open = (Pasori_open)GetDelegate("pasori_open", typeof(Pasori_open));
                pasori_close = (Pasori_close)GetDelegate("pasori_close", typeof(Pasori_close));
                pasori_init = (Pasori_init)GetDelegate("pasori_init", typeof(Pasori_init));
                felica_polling = (Felica_polling)GetDelegate("felica_polling", typeof(Felica_polling));
                felica_free = (Felica_free)GetDelegate("felica_free", typeof(Felica_free));
                felica_getidm = (Felica_getidm)GetDelegate("felica_getidm", typeof(Felica_getidm));
                felica_getpmm = (Felica_getpmm)GetDelegate("felica_getpmm", typeof(Felica_getpmm));
                felica_read_without_encryption02 = (Felica_read_without_encryption02)GetDelegate("felica_read_without_encryption02", typeof(Felica_read_without_encryption02));
            }
            catch (Exception)
            {
                throw new Exception(szDLLname + " をロードできません");
            }

            pasorip = pasori_open(null);
            if (pasorip == IntPtr.Zero)
            {
                throw new Exception(szDLLname + " を開けません");
            }
            if (pasori_init(pasorip) != 0)
            {
                throw new Exception("PaSoRi に接続できません");
            }
        }

        /// <summary>
        /// 指定名のアンマネージ関数ポインタをデリゲートに変換
        /// </summary>
        /// <param name="procName">アンマネージ関数名</param>
        /// <param name="delegateType">変換するデリゲートのType</param>
        /// <returns>変換したデリゲート</returns>
        public Delegate GetDelegate(string procName, Type delegateType)
        {
            var proc = NativeMethodsHelper.GetProcAddress(_pModule, procName);
            return Marshal.GetDelegateForFunctionPointer(proc, delegateType);
        }

        #region IDisposable メンバ

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        ~Felica()
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
            // MEMO: 読み込みとは逆の順序でリソースを解放します。
            if (felicap != IntPtr.Zero)
            {
                try
                {
                    felica_free(felicap);
                    felicap = IntPtr.Zero;
                }
                catch (AccessViolationException)
                {
                }
            }

            if (pasorip != IntPtr.Zero)
            {
                try
                {
                    pasori_close(pasorip);
                    pasorip = IntPtr.Zero;
                }
                catch (AccessViolationException)
                {
                }
            }

            if (_pModule != IntPtr.Zero)
            {
                NativeMethodsHelper.FreeLibrary(_pModule);
                _pModule = IntPtr.Zero;
            }
        }

        #endregion

        /// <summary>
        /// ポーリング
        /// </summary>
        /// <param name="systemcode">システムコード</param>
        public void Polling(FelicaSystemCode systemcode)
        {
            Polling((int)systemcode);
        }

        /// <summary>
        /// ポーリング
        /// </summary>
        /// <param name="systemcode">システムコード</param>
        public void Polling(int systemcode)
        {
            felica_free(felicap);

            felicap = felica_polling(pasorip, (ushort)systemcode, 0, 0);
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("カード読み取り失敗");
            }
        }

        /// <summary>
        /// IDm取得
        /// </summary>
        /// <returns>IDmバイナリデータ</returns>
        public byte[] IDm()
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] buf = new byte[8];
            felica_getidm(felicap, buf);
            return buf;
        }

        /// <summary>
        /// PMm取得
        /// </summary>
        /// <returns>PMmバイナリデータ</returns>
        public byte[] PMm()
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] buf = new byte[8];
            felica_getpmm(felicap, buf);
            return buf;
        }

        /// <summary>
        /// 非暗号化領域読み込み
        /// </summary>
        /// <param name="servicecode">サービスコード</param>
        /// <param name="addr">アドレス</param>
        /// <returns>データ</returns>
        public byte[] ReadWithoutEncryption(int servicecode, int addr)
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] data = new byte[16];
            int ret = felica_read_without_encryption02(felicap, servicecode, 0, (byte)addr, data);
            if (ret != 0)
            {
                return null;
            }
            return data;
        }
    }
}
