#region Copyright notice and license
// Copyright 2016 github.com/satr.  All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of github.com/satr nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Common
{
    public static class SecureStringHelper
    {
        const int PtrOffset = -4;
        private static readonly byte[] Entropy = new Guid(@"C3B2B71D-6D28-4EAE-9CF9-E57795EBDE24").ToByteArray();

        public static SecureString GetSecureString(this string value)
        {
            var secureString = new SecureString();
            foreach (var c in value)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        public static bool Equal(SecureString value1, SecureString value2, bool emptyValuesEqual)
        {
            if (value1 == null || value2 == null)
                return false;
            var bStr1 = IntPtr.Zero;
            var bStr2 = IntPtr.Zero;
            try
            {
                bStr1 = Marshal.SecureStringToBSTR(value1);
                bStr2 = Marshal.SecureStringToBSTR(value2);

                var lengthStr1 = Marshal.ReadInt32(bStr1, PtrOffset);
                var lengthStr2 = Marshal.ReadInt32(bStr2, PtrOffset);
                
                if (!emptyValuesEqual && (lengthStr1 == 0 || lengthStr2 == 0))
                    return false;
                if (lengthStr1 != lengthStr2)
                    return false;
                
                for (var i = 0; i < lengthStr1; ++i)
                {
                    if (Marshal.ReadByte(bStr1, i) != Marshal.ReadByte(bStr2, i))
                        return false;
                }
                return true;
            }
            finally
            {
                if (bStr1 != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bStr1);
                if (bStr2 != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bStr2);
            }
        }

        public static string EncriptSecureString(SecureString value)
        {
            var bStr = IntPtr.Zero;
            try
            {
                var insecureStringAsByteArray = InsecureStringAsByteArray(value);
                var encryptedData = ProtectedData.Protect(insecureStringAsByteArray, Entropy, DataProtectionScope.CurrentUser);
                var encriptSecureString = Convert.ToBase64String(encryptedData);
                for (var i = 0; i < encryptedData.Length; i++)
                {
                    encryptedData[i] = 0;
                }
                return encriptSecureString;
            }
            finally
            {
                if (bStr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bStr);
            }
        }

        public static SecureString DecriptSecureString(string encriptedData)
        {
            var secureString = new SecureString();
            if (string.IsNullOrWhiteSpace(encriptedData))
            {
                secureString.MakeReadOnly();
                return secureString;
            }
            try
            {
                var fromBase64String = Convert.FromBase64String(encriptedData);
                foreach (var ch in Encoding.Unicode.GetString(ProtectedData.Unprotect(fromBase64String, Entropy, DataProtectionScope.CurrentUser)))
                {
                    secureString.AppendChar(ch);
                }
                secureString.MakeReadOnly();
            }
            catch
            {
                secureString = new SecureString();
                secureString.MakeReadOnly();
                throw;
            }
            InsecureStringAsByteArray(secureString);
            return secureString;
        }

        private static byte[] InsecureStringAsByteArray(SecureString value)
        {
            if(value == null || value.Length == 0)
                return new byte[0];

            var bStr = IntPtr.Zero;
            try
            {
                bStr = Marshal.SecureStringToBSTR(value);
                return Encoding.Unicode.GetBytes(Marshal.PtrToStringBSTR(bStr));
            }
            finally
            {
                if (bStr != IntPtr.Zero)
                    Marshal.ZeroFreeBSTR(bStr);
            }
        }
    }
}
