﻿/*
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
using System.Diagnostics;
using System.Linq;
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
        [return: MarshalAs(UnmanagedType.Bool)]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
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

        /// <summary>
        /// DLL が持つ、指定されたエクスポート済み関数をデリゲートとして取得します。
        /// </summary>
        /// <typeparam name="TDelegate">デリゲートの型。</typeparam>
        /// <param name="module">DLL モジュールのハンドル。</param>
        /// <param name="procName">関数名。</param>
        /// <returns>デリゲート。</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static TDelegate GetDelegate<TDelegate>(IntPtr module, string procName) where TDelegate : class
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidOperationException("TDelegate はデリゲート型でなければなりません。");
            }
            var proc = GetProcAddress(module, procName);
            return Marshal.GetDelegateForFunctionPointer(proc, typeof(TDelegate)) as TDelegate;
        }
    }

    /// <summary>
    /// FeliCa を通じて IC カードからデータを読み取るためのクラスを表します。
    /// </summary>
    public class Felica : IDisposable
    {
        #region DLL およびデリゲート

        // 遅延ロード用デリゲートの定義
        delegate IntPtr Pasori_open(string dummy);
        delegate int Pasori_close(IntPtr p);
        delegate int Pasori_init(IntPtr p);
        delegate IntPtr Felica_polling(IntPtr p, ushort systemcode, byte rfu, byte time_slot);
        delegate void Felica_free(IntPtr f);
        delegate void Felica_getidm(IntPtr f, byte[] data);
        delegate void Felica_getpmm(IntPtr f, byte[] data);
        delegate int Felica_read_without_encryption02(IntPtr f, int servicecode, int mode, byte addr, byte[] data);

        // 遅延ロード用デリゲートの実体
        Pasori_open pasori_open;
        Pasori_close pasori_close;
        Pasori_init pasori_init;
        Felica_polling felica_polling;
        Felica_free felica_free;
        Felica_getidm felica_getidm;
        Felica_getpmm felica_getpmm;
        Felica_read_without_encryption02 felica_read_without_encryption02;

        void LoadDllAndDelegates()
        {
            dllModulePtr = NativeMethodsHelper.LoadLibrary(dllFileName);

            pasori_open = NativeMethodsHelper.GetDelegate<Pasori_open>(dllModulePtr, "pasori_open");
            pasori_close = NativeMethodsHelper.GetDelegate<Pasori_close>(dllModulePtr, "pasori_close");
            pasori_init = NativeMethodsHelper.GetDelegate<Pasori_init>(dllModulePtr, "pasori_init");
            felica_polling = NativeMethodsHelper.GetDelegate<Felica_polling>(dllModulePtr, "felica_polling");
            felica_free = NativeMethodsHelper.GetDelegate<Felica_free>(dllModulePtr, "felica_free");
            felica_getidm = NativeMethodsHelper.GetDelegate<Felica_getidm>(dllModulePtr, "felica_getidm");
            felica_getpmm = NativeMethodsHelper.GetDelegate<Felica_getpmm>(dllModulePtr, "felica_getpmm");
            felica_read_without_encryption02 = NativeMethodsHelper.GetDelegate<Felica_read_without_encryption02>(dllModulePtr, "felica_read_without_encryption02");
        }

        #endregion

        string dllFileName;
        IntPtr dllModulePtr;
        IntPtr pasoriPtr;
        IntPtr felicaPtr;

        /// <summary>
        /// システム コードを取得します。
        /// </summary>
        /// <value>システム コード。</value>
        public int SystemCode { get; private set; }

        /// <summary>
        /// <see cref="Felica"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="systemCode">システム コード。</param>
        public Felica(int systemCode)
        {
            SystemCode = systemCode;

            // x64対応 20100501 - DeForest
            // プラットフォーム別のロードモジュール名決定（x64/x86サポート、Iteniumはサポート外）
            dllFileName = IntPtr.Size >= 8 ? "felicalib64.dll" : "felicalib.dll";

            try
            {
                LoadDllAndDelegates();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("{0} をロードできません。", dllFileName), ex);
            }
        }

        #region IDisposable メンバー

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
            // 読み込みとは逆の順序でリソースを解放します。
            try
            {
                CloseFelicaPtr();
            }
            catch (Exception ex)
            {
                // 発生したことのある例外:
                // System.AccessViolationException
                Debug.WriteLine(ex);
            }

            try
            {
                ClosePasoriPtr();
            }
            catch (Exception ex)
            {
                // 発生したことのある例外:
                // System.AccessViolationException
                Debug.WriteLine(ex);
            }

            try
            {
                CloseDllModule();
            }
            catch (Exception ex)
            {
                // 発生したことのある例外:
                // System.IO.FileNotFoundException
                Debug.WriteLine(ex);
            }
        }

        void CloseDllModule()
        {
            if (dllModulePtr != IntPtr.Zero)
            {
                NativeMethodsHelper.FreeLibrary(dllModulePtr);
                dllModulePtr = IntPtr.Zero;
            }
        }

        void ClosePasoriPtr()
        {
            if (pasoriPtr != IntPtr.Zero)
            {
                pasori_close(pasoriPtr);
                pasoriPtr = IntPtr.Zero;
            }
        }

        void CloseFelicaPtr()
        {
            if (felicaPtr != IntPtr.Zero)
            {
                felica_free(felicaPtr);
                felicaPtr = IntPtr.Zero;
            }
        }

        #endregion

        /// <summary>
        /// FeliCa ポートに接続できるかどうかを確認します。
        /// </summary>
        /// <returns>FeliCa ポートに接続できる場合は <see langword="true"/>。</returns>
        public bool TryConnectionToPort()
        {
            try
            {
                return
                    (pasoriPtr = pasori_open(null)) != IntPtr.Zero &&
                    pasori_init(pasoriPtr) == 0;
            }
            finally
            {
                ClosePasoriPtr();
            }
        }

        /// <summary>
        /// IC カードに接続できるかどうかを確認します。
        /// </summary>
        /// <returns>IC カードに接続できる場合は <see langword="true"/>。</returns>
        public bool TryConnectionToCard()
        {
            try
            {
                return
                    (pasoriPtr = pasori_open(null)) != IntPtr.Zero &&
                    pasori_init(pasoriPtr) == 0 &&
                    (felicaPtr = felica_polling(pasoriPtr, (ushort)SystemCode, 0, 0)) != IntPtr.Zero;
            }
            finally
            {
                CloseFelicaPtr();
                ClosePasoriPtr();
            }
        }

        TResult TransferData<TResult>(Func<TResult> readData)
        {
            try
            {
                if ((pasoriPtr = pasori_open(null)) == IntPtr.Zero)
                {
                    throw new InvalidOperationException(string.Format("{0} を開けません。", dllFileName));
                }
                if (pasori_init(pasoriPtr) != 0)
                {
                    throw new InvalidOperationException("PaSoRi に接続できません。");
                }

                // felica_polling 関数によるポーリングは、IC カードが範囲内に存在する場合のみ可能です。
                if ((felicaPtr = felica_polling(pasoriPtr, (ushort)SystemCode, 0, 0)) == IntPtr.Zero)
                {
                    throw new InvalidOperationException("IC カードが見つかりません。または、システム コードが一致しません。");
                }

                return readData();
            }
            finally
            {
                CloseFelicaPtr();
                ClosePasoriPtr();
            }
        }

        /// <summary>
        /// 製造 ID (IDm) を取得します。
        /// </summary>
        /// <returns>製造 ID (IDm)。配列の長さは 8 です。</returns>
        public byte[] GetIDm()
        {
            return TransferData(() =>
            {
                var data = new byte[8];
                felica_getidm(felicaPtr, data);
                return data;
            });
        }

        /// <summary>
        /// 製造パラメータ (PMm) を取得します。
        /// </summary>
        /// <returns>製造パラメータ (PMm)。配列の長さは 8 です。</returns>
        public byte[] GetPMm()
        {
            return TransferData(() =>
            {
                var data = new byte[8];
                felica_getpmm(felicaPtr, data);
                return data;
            });
        }

        /// <summary>
        /// 非暗号化領域の 1 つのブロックのデータを読み込みます。
        /// </summary>
        /// <param name="serviceCode">サービス コード。</param>
        /// <param name="address">アドレス。</param>
        /// <returns>非暗号化領域のブロックのデータ。配列の長さは 16 です。</returns>
        public byte[] ReadWithoutEncryption(int serviceCode, int address)
        {
            return TransferData(() => ReadBlock_Internal(serviceCode, address));
        }

        /// <summary>
        /// 非暗号化領域の連続した複数のブロックのデータを読み込みます。
        /// </summary>
        /// <param name="serviceCode">サービス コード。</param>
        /// <param name="addressStart">読み込むブロックの最初のアドレス。</param>
        /// <param name="addressCount">読み込むブロックの数。</param>
        /// <returns>非暗号化領域のブロックのデータのシーケンス。</returns>
        public IEnumerable<byte[]> ReadBlocksWithoutEncryption(int serviceCode, int addressStart, int addressCount)
        {
            return TransferData(() =>
                Enumerable.Range(addressStart, addressCount)
                    .Select(i => ReadBlock_Internal(serviceCode, i))
                    .ToArray());
        }

        byte[] ReadBlock_Internal(int serviceCode, int address)
        {
            var data = new byte[16];
            if (felica_read_without_encryption02(felicaPtr, serviceCode, 0, (byte)address, data) != 0)
            {
                throw new InvalidOperationException("指定されたサービス コードおよびアドレスのデータが存在しません。");
            }
            // 関数の戻り値が 0 でも、配列の要素がすべて 0 のままであることがあります。
            return data;
        }
    }
}
