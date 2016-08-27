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
using System.Collections.ObjectModel;
using Backend.BusinessLayer;
using Frontend.App.BusinessLayer.Documents;
using Frontend.App.BusinessLayer.Orders;
using Frontend.App.PresentationLayer.Mediators.Operations.Orders;
using Frontend.WPF.Mediators;
using Frontend.WPF.Mediators.Entities;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.ViewModels;
using Frontend.WPF.Views.Contacts;

namespace Frontend.App.PresentationLayer.ViewModels.Orders
{
    public abstract class EditableOrderDetailsViewModelBase : OrderDetailsViewModelBase, IOperationViewModel
    {
        private readonly AddDocumentsToOrderOperationMediator _addDocumentsToOrderOperationMediator;

        protected EditableOrderDetailsViewModelBase()
        {
            var orderDocumentsScopeContext = new OperationScopeContext();
            var addDocumentsToOrderMediator = new DocumentsMediator<OrderDocument>(OrderDocumentsScopeContext, OrderDocumentsRepository);
            _addDocumentsToOrderOperationMediator = new AddDocumentsToOrderOperationMediator(orderDocumentsScopeContext);
            PopulateToolbar(addDocumentsToOrderMediator, _addDocumentsToOrderOperationMediator);
        }

        private void PopulateToolbar(IMediator orderDocumentsMediator, IOperationMediator addDocumentsToOrderOperationMediator)
        {
            ToolbarSource = new ObservableCollection<OperationViewModel>();
            foreach (var operationMediator in orderDocumentsMediator.OperationMediators)
            {
                ToolbarSource.Add(new OperationViewModel(operationMediator));
            }
            ToolbarSource.Add(new OperationViewModel(addDocumentsToOrderOperationMediator));
        }

        public override Order Order
        {
            get { return base.Order; }
            set
            {
                base.Order = value;
                _addDocumentsToOrderOperationMediator.Order = Order;
            }
        }

        public override bool ReadOnly
        {
            get { return false; }
        }

        public ObservableCollection<OperationViewModel> ToolbarSource { get; set; }
        public abstract bool ValidateAndInitialize();
        public abstract void PerformOperation();

        public bool PerformOperationEnabled
        {
            get { return true; }
        }

        public bool CancelOperationEnabled
        {
            get { return true; }
        }
    }
}