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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Backend.BusinessLayer.Settings;
using Backend.DataLayer;
using Frontend.WPF.Commands.Settings;
using Frontend.WPF.Common;

namespace Frontend.WPF.ViewModels.Settings
{
    public class DatabaseSettingsViewModel : SecuritySettingsViewModelBase<DatabaseSettings, DatabaseSettingsRepository>
    {
        private KeyValuePair<string, SqlConnectionInfo.DbType> _databaseType;
        private List<SqlConnectionInfo> _databaseProvidersSource;
        private bool _databaseTypeIsServer;
        private bool _databaseTypeIsFile;
        private IList<KeyValuePair<string, SqlConnectionInfo.DbType>> _databaseTypeSource;
        private SqlConnectionInfo _databaseProvider;
        private List<SqlConnectionInfo> _localDbConnectionInfos;
        private bool _sqlServerSettingsListInitialized;
        private KeyValuePair<string, Authentication> _databaseAuthentication;
        private IList<KeyValuePair<string, Authentication>> _databaseAuthenticationSource;
        private bool _sqlServerAuthentication;

        public DatabaseSettingsViewModel()
        {
            DatabaseTypeIsServer = false;
            SetDatabaseType(null);
            DatabaseAuthentication = DatabaseAuthenticationSource.FirstOrDefault();
        }

        public KeyValuePair<string, Authentication> DatabaseAuthentication
        {
            get { return _databaseAuthentication; }
            set
            {
                SetProperty(ref _databaseAuthentication, value);
                SqlServerAuthentication = DatabaseAuthentication.Value == Authentication.SqlServer;
                if (Settings != null)
                    Settings.WindowsAuthentication = !SqlServerAuthentication;
                if (!SqlServerAuthentication)
                    ChangePassword = false;
            }
        }

        public bool SqlServerAuthentication
        {
            get { return _sqlServerAuthentication; }
            set { SetProperty(ref _sqlServerAuthentication, value); }
        }

        public IList<KeyValuePair<string, Authentication>> DatabaseAuthenticationSource
        {
            get { return _databaseAuthenticationSource?? (_databaseAuthenticationSource = CreateDatabaseAuthenticationSource()); }
        }

        private static IList<KeyValuePair<string, Authentication>> CreateDatabaseAuthenticationSource()
        {
            return new List<KeyValuePair<string, Authentication>>
            {
                new KeyValuePair<string, Authentication>("Windows Authentication", Authentication.Windows),
                new KeyValuePair<string, Authentication>("SQL Server Authentication", Authentication.SqlServer)
            };
        }

        public ICommand CreateDatabaseFileCommand { get { return new CreateDatabaseFileCommand(this); } }
        public ICommand SelectDatabaseFileCommand { get { return new SelectDatabaseFileCommand(this); } }

        public bool DatabaseTypeIsServer
        {
            get { return _databaseTypeIsServer; }
            set
            {
                SetProperty(ref _databaseTypeIsServer, value);
                DatabaseTypeIsFile = !DatabaseTypeIsServer;
            }
        }

        public bool DatabaseTypeIsFile
        {
            get { return _databaseTypeIsFile; }
            set { SetProperty(ref _databaseTypeIsFile, value); }
        }

        public List<SqlConnectionInfo> DatabaseProvidersSource
        {
            get { return _databaseProvidersSource; }
            set { SetProperty(ref _databaseProvidersSource, value); }
        }

        public SqlConnectionInfo DatabaseProvider
        {
            get { return _databaseProvider; }
            set
            {
                SetProperty(ref _databaseProvider, value);
                if(DatabaseProvider == null || Settings == null)
                    return;
                Settings.ProviderName = DatabaseProvider.Name;
                Settings.ServerName = DatabaseProvider.ServerName;
                Settings.ProviderVersion = DatabaseProvider.Version;
                Settings.ProviderInstanceName = DatabaseProvider.InstanceName;
            }
        }

        public IList<KeyValuePair<string, SqlConnectionInfo.DbType>> DatabaseTypeSource
        {
            get { return _databaseTypeSource?? (_databaseTypeSource = CreateDatabaseTypeSource()); }
        }

        private static IList<KeyValuePair<string, SqlConnectionInfo.DbType>> CreateDatabaseTypeSource()
        {
            return new List<KeyValuePair<string, SqlConnectionInfo.DbType>>
            {
                new KeyValuePair<string, SqlConnectionInfo.DbType>("Server", SqlConnectionInfo.DbType.Server),
                new KeyValuePair<string, SqlConnectionInfo.DbType>("File", SqlConnectionInfo.DbType.LocalDb),
            };
        }

        protected override void UpdateSettings(DatabaseSettings settings)
        {
            SetDatabaseType(settings);
        }

        private void SetDatabaseType(DatabaseSettings settings)
        {
            DatabaseType = DatabaseTypeSource.FirstOrDefault( i => i.Value == (settings == null 
                                                                                ? SqlConnectionInfo.DbType.LocalDb 
                                                                                : (SqlConnectionInfo.DbType) settings.DatabaseType));
            SetDatabaseProvider(settings);
            SetDatabaseAuthentication(settings);
        }

        private void SetDatabaseProvider(DatabaseSettings settings)
        {
            var databaseProvider = settings == null || DatabaseProvidersSource == null
                                    ? null
                                    : DatabaseProvidersSource.FirstOrDefault(i => i.Version == settings.ProviderVersion);
            if (databaseProvider != null && DatabaseProvidersSource == null)
            {
                DatabaseProvidersSource = new List<SqlConnectionInfo> {databaseProvider};
            }
            DatabaseProvider = databaseProvider;
        }

        private void SetDatabaseAuthentication(DatabaseSettings settings)
        {
            if (settings == null) 
                return;
            DatabaseAuthentication = DatabaseAuthenticationSource.FirstOrDefault(i => Equals(i.Value, settings.WindowsAuthentication
                                                                                                        ? Authentication.Windows
                                                                                                        : Authentication.SqlServer));
        }

        public KeyValuePair<string, SqlConnectionInfo.DbType> DatabaseType
        {
            get { return _databaseType; }
            set
            {
                if(!SetProperty(ref _databaseType, value))
                    return;
                DatabaseTypeIsServer = DatabaseType.Value == SqlConnectionInfo.DbType.Server;
                DatabaseProvidersSource = DatabaseTypeIsServer? SqlServerSettingsList: SqlLocalDbSettingsList;
                if (DatabaseTypeIsServer && Settings != null)
                    Settings.ServerName = string.Empty;

            }
        }

        private List<SqlConnectionInfo> SqlLocalDbSettingsList
        {
            get
            {
                return _localDbConnectionInfos?? (_localDbConnectionInfos = SqlConnectionInfoFactory.GetSqlLocalDbSettingsList());
            }
        }

        private List<SqlConnectionInfo> SqlServerSettingsList { get; set; }

        public void PopulateDatabaseProviderSource()
        {
            if (DatabaseType.Value == SqlConnectionInfo.DbType.Server && !_sqlServerSettingsListInitialized)
                InitSqlServerSettingsSource();
        }

        private void InitSqlServerSettingsSource()
        {
            UIHelper.PerformLongOperation("Collecting information",
                () => SqlServerSettingsList = SqlConnectionInfoFactory.GetSqlServerSettingsList());
            _sqlServerSettingsListInitialized = true;
            DatabaseProvidersSource = SqlServerSettingsList;
            if (Settings == null)
                return;
            DatabaseProvider = Settings.DatabaseType == (decimal) SqlConnectionInfo.DbType.Server
                ? DatabaseProvidersSource.FirstOrDefault(p => p.Version == Settings.ProviderVersion)
                : DatabaseProvidersSource.FirstOrDefault();
        }

        public enum Authentication
        {
            SqlServer,
            Windows
        }
    }

}