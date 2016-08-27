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
using System.Collections;
using System.Collections.ObjectModel;
using Backend.BusinessLayer;
using Frontend.App.BusinessLayer.Documents;
using Frontend.App.BusinessLayer.Orders;
using Frontend.WPF.Common;
using Frontend.WPF.ViewModels.Documents;

namespace Frontend.App.PresentationLayer.ViewModels.Orders
{
    public abstract class OrderDetailsViewModelBase: NotifyingEntityBase
    {
        private Order _order;
        private readonly Collection<OrderDocument> _selectedDocuments = new Collection<OrderDocument>();
        private DocumentsViewModel<OrderDocument> _documentsDataContext;

        protected OrderDetailsViewModelBase()
        {
            OrderDocumentsScopeContext = new OperationScopeContext();
            OrderDocumentsRepository = new EntityCollectionRepository<OrderDocument, Guid>();
            DocumentsDataContext = new DocumentsViewModel<OrderDocument>(OrderDocumentsScopeContext, OrderDocumentsRepository);
        }

        protected OperationScopeContext OrderDocumentsScopeContext { get; private set; }

        protected EntityCollectionRepository<OrderDocument, Guid> OrderDocumentsRepository { get; private set; }

        public DocumentsViewModel<OrderDocument> DocumentsDataContext
        {
            get { return _documentsDataContext; }
            set { SetProperty(ref _documentsDataContext, value); }
        }

        public virtual Order Order
        {
            get { return _order; }
            set
            {
                OrderDocumentsScopeContext.Clear<OrderDocument>();
                SetProperty(ref _order, value);
                var documents = Order != null ? Order.Documents : new ObservableCollection<OrderDocument>();
                DocumentsDataContext.Entities = OrderDocumentsRepository.Entities = documents;
            }
        }

        public void SetSelectedItems(IList addedItems, IList removedItems)
        {
            _selectedDocuments.Update(addedItems, removedItems);
            OrderDocumentsScopeContext.SelectItemsAsync<OrderDocument>(_selectedDocuments);
        }

        public abstract bool ReadOnly { get; }
    }
}