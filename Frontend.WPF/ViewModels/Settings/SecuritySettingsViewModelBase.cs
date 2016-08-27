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
using Backend.BusinessLayer;
using Backend.BusinessLayer.Settings;
using Backend.Common;

namespace Frontend.WPF.ViewModels.Settings
{
    public abstract class SecuritySettingsViewModelBase<TSettings, TRepository> : NotifyingEntityBase, ISettingsViewModel
        where TSettings: SecuritySettingsBase<TSettings>
        where TRepository: class, IEntityRepository<TSettings>, new()
    {
        private bool _changePassword;
        private bool _passwordsMatch;
        private bool _passwordsMismatch;
        private SecureString _confirmedPassword;
        private SecureString _newPassword;
        private SecureString _currentPassword;
        private readonly TRepository _repository;
        private TSettings _settings;

        protected SecuritySettingsViewModelBase()
        {
            _repository = ServiceLocator.Get<TRepository>();
        }

        public bool ValidateAndInitialize()
        {
            Settings = _repository.Get().Clone();
            UpdatePasswordMatches();
            return true;
        }

        public void PerformOperation()
        {
            if (Settings == null)//ViewModel not initialized
                return;
            _repository.Save(Settings);
        }

        public TSettings Settings
        {
            get { return _settings; }
            set
            {
                SetProperty(ref _settings, value);
                _currentPassword = Settings != null ? Settings.Password : null;
                UpdateSettings(Settings);
            }
        }

        protected virtual void UpdateSettings(TSettings settings)
        {
        }

        public bool PerformOperationEnabled
        {
            get { return true; }
        }

        public bool CancelOperationEnabled
        {
            get { return true; }
        }

        public bool ChangePassword
        {
            get { return _changePassword; }
            set
            {
                SetProperty(ref _changePassword, value);
                UpdatePasswordMatches();
            }
        }

        public SecureString NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                UpdatePasswordMatches();
            }
        }

        public SecureString ConfirmedPassword
        {
            get { return _confirmedPassword; }
            set
            {
                _confirmedPassword = value;
                UpdatePasswordMatches();
            }
        }

        public bool PasswordsMatch
        {
            get { return _passwordsMatch; }
            set { SetProperty(ref _passwordsMatch, value); }
        }

        public bool PasswordsMismatch
        {
            get { return _passwordsMismatch; }
            set { SetProperty(ref _passwordsMismatch, value); }
        }

        private void UpdatePasswordMatches()
        {
            var passwordsEqual = SecureStringHelper.Equal(NewPassword, ConfirmedPassword, false);
            PasswordsMatch = ChangePassword && passwordsEqual;
            PasswordsMismatch = ChangePassword && !passwordsEqual;
            if (Settings == null)
                return;
            Settings.Password = ChangePassword && SecureStringHelper.Equal(NewPassword, ConfirmedPassword, false)
                ? NewPassword
                : _currentPassword;
        }
    }
}