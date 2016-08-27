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
using System.Data;
using System.Data.Sql;
using System.Linq;
using Microsoft.Win32;

namespace Backend.DataLayer
{
    public class SqlConnectionInfoFactory
    {
        public static List<SqlConnectionInfo> GetSqlLocalDbSettingsList()
        {
            var list = new List<SqlConnectionInfo>();
            using (var registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions"))
            {
                if (registryKey == null)
                    return list;
                list.AddRange(registryKey.GetSubKeyNames().Select(SqlConnectionInfoFactory.GetLocalDbSettings)
                                                          .Where(item => item != null));
            }
            return list.OrderByDescending(s => s.Version).ToList();
        }

        public static List<SqlConnectionInfo> GetSqlServerSettingsList()
        {
            var list = new List<SqlConnectionInfo>();
            var table = SqlDataSourceEnumerator.Instance.GetDataSources();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (DataRow row in table.Rows)
            {
                var sqlConnectionInfo = GetServerSettings(table, row);
                if(sqlConnectionInfo?.Version != null)
                    list.Add(sqlConnectionInfo);
            }
            return list.OrderByDescending(s => s.Version).ToList();
        }

        private static SqlConnectionInfo GetLocalDbSettings(string versionName)
        {
            if (Equals(versionName, "12.0"))
                return new SqlConnectionInfo("Local file, SQL Server 2014", "(localdb)", "sqldblocaldb", 12.0m, SqlConnectionInfo.DbType.LocalDb);
            if (Equals(versionName, "11.0"))
                return new SqlConnectionInfo("Local file, SQL Server 2012", "(localdb)", "v11.0", 11.0m, SqlConnectionInfo.DbType.LocalDb);
            return null;
        }


        private static SqlConnectionInfo GetServerSettings(DataTable table, DataRow dataRow)
        {
            var serverName = GetValue(table, "ServerName", dataRow);
            var instanceName = GetValue(table, "InstanceName", dataRow);
            var sqlVersion = GetValue(table, "Version", dataRow);
            if (serverName == null || instanceName == null || sqlVersion == null)
                return null;
            if (sqlVersion.StartsWith("11."))
                return new SqlConnectionInfo("SQL Server 2012", serverName, instanceName, 11.0m, SqlConnectionInfo.DbType.Server);
            if (sqlVersion.StartsWith("10.5"))
                return new SqlConnectionInfo("SQL Server 2008 R2", serverName, instanceName, 10.5m, SqlConnectionInfo.DbType.Server);
            if (sqlVersion.StartsWith("10."))
                return new SqlConnectionInfo("SQL Server 2008", serverName, instanceName, 10.0m, SqlConnectionInfo.DbType.Server);
            if (sqlVersion.StartsWith("9."))
                return new SqlConnectionInfo("SQL Server 2005", serverName, instanceName, 9.0m, SqlConnectionInfo.DbType.Server);
            if (sqlVersion.StartsWith("8."))
                return new SqlConnectionInfo("SQL Server 2000", serverName, instanceName, 8.0m, SqlConnectionInfo.DbType.Server);
            return null;
        }

        private static string GetValue(DataTable table, string columnName, DataRow dataRow)
        {
            return table.Columns.Contains(columnName) ? dataRow[columnName].ToString() : null;
        }
    }
}
