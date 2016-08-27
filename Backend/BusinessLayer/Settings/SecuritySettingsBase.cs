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
using System.Security;
using Backend.Common;

namespace Backend.BusinessLayer.Settings
{
    public abstract class SecuritySettingsBase<T> : NotifyingEntityBase, ISecuritySettings
        where T: SecuritySettingsBase<T>
    {
        private string _serverName;
        private string _userName;

        protected SecuritySettingsBase()
        {
            Password = new SecureString();
            Password.MakeReadOnly();
        }

        public string ServerName
        {
            get { return _serverName; }
            set { SetProperty(ref _serverName, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        //Password only entered of passed as SecureString - never exposed as plain text.
        public SecureString Password { get; set; }

        //Data encripted for current user and converted to Base64 string.
        public string EncriptedPasswordData
        {
            get { return SecureStringHelper.EncriptSecureString(Password); }
            set{ Password = SecureStringHelper.DecriptSecureString(value); }
        }

        public T Clone()
        {
            var clone = (SecuritySettingsBase<T>)this.MemberwiseClone();
            clone.Password = Password.Copy();
            return (T)clone;
        }
    }
}