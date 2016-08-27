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
using System.Windows.Controls;
using Frontend.App.PresentationLayer.ViewModels.Orders;
using Frontend.App.PresentationLayer.Views.Orders;
using Frontend.WPF.Mediators.Operations;

namespace Frontend.App.PresentationLayer.Mediators.Operations.Orders
{
    public abstract class EditableOrderOperationMediatorBase : ShowContentViewInSingleWindowOperationMediatorBase
    {
        private OrderDetailsView _orderDetailsView;
        private EditableOrderDetailsViewModelBase _viewModel;

        protected EditableOrderOperationMediatorBase(string name, string description, string imagePath = null)
            : base(name, description, imagePath)
        {
            Enabled = true;
        }

        protected override Control CreateContentView()
        {
            return _orderDetailsView ?? (_orderDetailsView = new OrderDetailsView(ViewModel));
        }

        private EditableOrderDetailsViewModelBase ViewModel
        {
            get { return _viewModel?? (_viewModel = CreateViewModel()); }
        }

        protected abstract EditableOrderDetailsViewModelBase CreateViewModel();

        protected override bool ValidateAndInitialize()
        {
            return ViewModel.ValidateAndInitialize();
        }
    }
}