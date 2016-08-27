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
using System.Collections.ObjectModel;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Backend.Common;
using Frontend.App.BusinessLayer.Documents;

namespace Frontend.App.BusinessLayer.Orders
{
    public class Order: EntityBase
    {
        private string _contractNumber;
        private string _orderNumber;
        private string _invoiceNumber;
        private DateTimeOffset _createdOn;
        private ObservableCollection<OrderDocument> _documents;
        private ObservableCollection<Contact> _contacts;

        public string ContractNumber
        {
            get { return _contractNumber; }
            set { SetProperty(ref _contractNumber, value); }
        }

        public string OrderNumber
        {
            get { return _orderNumber; }
            set { SetProperty(ref _orderNumber, value); }
        }

        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }

        public DateTimeOffset CreatedOn
        {
            get { return _createdOn; }
            set { SetProperty(ref _createdOn, value); }
        }

        public virtual ObservableCollection<OrderDocument> Documents
        {
            get { return _documents ?? (_documents = new ObservableCollection<OrderDocument>()); }
            private set { _documents = value; }
        }

        public virtual ObservableCollection<Contact> Contacts
        {
            get { return _contacts?? (_contacts = new ObservableCollection<Contact>()); }
            set { _contacts = value; }
        }

        public Order Clone()
        {
            var clone = Helper.ShallowCopy(this);
            clone.Documents = new ObservableCollection<OrderDocument>(Documents);
            return clone;
        }

        public void ApplyChangesFrom(Order order)
        {
            Helper.ApplyChangesToPrimitiveProperties(order, this);
            Helper.ApplyChangesToCollection(order.Documents, Documents);
        }
    }
}
