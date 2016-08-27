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
using System.Configuration;
using System.Security.Principal;
using Backend.Common;

namespace Backend.DataLayer.Settings
{
    public abstract class SettingsStorageBase<T> : ApplicationSettingsBase, ISettingsStorage<T>
        where T: class, new()
    {
        public virtual T Convert()
        {
            var settings = new T();
            Helper.ApplyChangesToPrimitivePropertiesOfDifferentTypes(this, settings);
            return settings;
        }

        public virtual void UpdateWith(T settings)
        {
            Helper.ApplyChangesToPrimitivePropertiesOfDifferentTypes(settings, this);
        }

        static class PropertyNames
        {
            public const string NeedUpgrade = "NeedUpgrade";
            public const string UpdatedBy = "UpdatedBy";
            public const string UpdatedOn = "UpdatedOn";
        }

        [UserScopedSetting]
        [DefaultSettingValue(@"true")]
        public bool NeedUpgrade
        {
            get { return ((bool)this[PropertyNames.NeedUpgrade]); }
            set { base[PropertyNames.NeedUpgrade] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string UpdatedBy
        {
            get { return (string)this[PropertyNames.UpdatedBy]; }
            set { this[PropertyNames.UpdatedBy] = value?? GetCurrentUser(); }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public DateTime UpdatedOn
        {
            get { return (DateTime)this[PropertyNames.UpdatedOn]; }
            // ReSharper disable once ValueParameterNotUsed
            set { this[PropertyNames.UpdatedOn] = DateTimeOffset.Now.LocalDateTime; }
        }

        private static string GetCurrentUser()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            return windowsIdentity == null ? "Undefined" : windowsIdentity.Name;
        }

        protected T GetInitialized(string propertyName, Func<T> initFunc)
        {
            return (T)(this[propertyName] ?? (this[propertyName] = initFunc()));
        }

        protected bool GetBoolValue(string propertyName, bool defaultValue)
        {
            bool value;
            return bool.TryParse(this[propertyName].ToString(), out value)? value: defaultValue;
        }

        protected decimal GetDecimalValue(string propertyName, decimal defualtValue)
        {
            decimal value;
            return decimal.TryParse(this[propertyName].ToString(), out value)? value: defualtValue;
        }

        protected int GetIntValue(string propertyName, int defaultValue)
        {
            int value;
            return int.TryParse(this[propertyName].ToString(), out value)? value: defaultValue;
        }
    }
}