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
using System.Text;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Frontend.WPF.Commands;

namespace Frontend.WPF.ViewModels.Emails
{
    public class ContactEmailViewModel: NotifyingEntityBase
    {
        private string _title;
        private string _toolTip;

        public ContactEmailViewModel(Contact contact, ICollection<ContactEmailViewModel> contactEmailViewModels)
        {
            Contact = contact;
            BuildViewModelContent(Contact);
            Contact.PropertyChanged += (sender, args) => BuildViewModelContent((Contact)sender);
            RemoveCommand = new ActionCommand(() => contactEmailViewModels.Remove(this));
        }

        public ActionCommand RemoveCommand { get; set; }

        private void BuildViewModelContent(Contact contact)
        {
            Title = string.IsNullOrWhiteSpace(contact.Name) ? contact.Email : contact.Name;
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(contact.Name))
            {
                sb.AppendFormat("Name: {0}", contact.Name);
                sb.AppendLine();
            }
            sb.AppendFormat("Email: {0}", contact.Email);
            ToolTip = sb.ToString();
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value); }
        }

        public Contact Contact { get; private set; }
    }
}