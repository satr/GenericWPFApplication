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
using System.Data.Entity;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Backend.Common;

namespace Backend.DataLayer
{
    public class DatabaseContextBase : DbContext
    {
        protected DatabaseContextBase(string connectionString) : base(connectionString)
        {
            ValidationResult = ValidateDatabase();
            Configuration.ValidateOnSaveEnabled = false;
#if DEBUG
            Database.Log = msg => System.Diagnostics.Debug.WriteLine(msg, @"Database");
#endif
        }

        public DatabaseContextBase(): this("Default")
        {
            throw new NotImplementedException();
        }

        private IActionResult ValidateDatabase()
        {
            var actionResult = new ActionResult();
            try
            {
                if(Database.CreateIfNotExists())
                    actionResult.AddInfo("Database created.");
                else if (Database.CompatibleWithModel(true))
                    actionResult.AddInfo("Database is valid.");
                else
                    actionResult.AddError("Database is not valid.");
            }
            catch (Exception e)
            {
                actionResult.AddError("Database is not available.\r\n{0}", Helper.ExpandMessages(e));
            }
            return actionResult;
        }

        public IActionResult ValidationResult { get; set; }

        public bool Valid { get { return ValidationResult.Success; } }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var contact = modelBuilder.Entity<Contact>();
            contact.HasKey(i => i.ID).Property(i => i.ID).IsRequired();
            contact.Property(i => i.Name).IsRequired();
        }
    }
}