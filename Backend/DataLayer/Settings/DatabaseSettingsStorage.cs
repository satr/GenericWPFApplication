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
using System.Configuration;
using Backend.BusinessLayer.Settings;
using Backend.DataLayer.Settings;

// ReSharper disable once CheckNamespace
namespace Settings
{
    [SettingsManageability(SettingsManageability.Roaming)]
    public class DatabaseSettingsStorage : SettingsStorageBase<DatabaseSettings>, IDatabaseSettings
    {
        public override DatabaseSettings Convert()
        {
            var settings = base.Convert();
            settings.EncriptedPasswordData = EncriptedPasswordData;
            return settings;
        }

        public override void UpdateWith(DatabaseSettings settings)
        {
            base.UpdateWith(settings);
            EncriptedPasswordData = settings.EncriptedPasswordData;
        }

        private static class PropertyNames
        {
            public const string ServerName = "ServerName";
            public const string UserName = "UserName";
            public const string EncriptedPasswordData = "EncriptedPasswordData";
            public const string DatabaseType = "DatabaseType";
            public const string FileName = "FileName";
            public const string ProviderVersion = "ProviderVersion";
            public const string ProviderName = "ProviderName";
            public const string ProviderInstanceName = "ProviderInstanceName";
            public const string WindowsAuthentication = "WindowsAuthentication";
        }


        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ServerName 
        {
            get { return (string)this[PropertyNames.ServerName]; }
            set { this[PropertyNames.ServerName] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string UserName 
        {
            get { return (string)this[PropertyNames.UserName]; }
            set { this[PropertyNames.UserName] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string EncriptedPasswordData 
        {
            get { return (string)this[PropertyNames.EncriptedPasswordData]; }
            set { this[PropertyNames.EncriptedPasswordData] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int DatabaseType 
        {
            get { return (int)this[PropertyNames.DatabaseType]; }
            set { this[PropertyNames.DatabaseType] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string FileName 
        {
            get { return (string)this[PropertyNames.FileName]; }
            set { this[PropertyNames.FileName] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public decimal ProviderVersion 
        {
            get { return GetDecimalValue(PropertyNames.ProviderVersion, 0); }
            set { this[PropertyNames.ProviderVersion] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProviderName 
        {
            get { return (string)this[PropertyNames.ProviderName]; }
            set { this[PropertyNames.ProviderName] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProviderInstanceName 
        {
            get { return (string)this[PropertyNames.ProviderInstanceName]; }
            set { this[PropertyNames.ProviderInstanceName] = value; }
        }
        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool WindowsAuthentication 
        {
            get { return GetBoolValue(PropertyNames.WindowsAuthentication, true); }
            set { this[PropertyNames.WindowsAuthentication] = value; }
        }
    }
}