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
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.WPF.Commands;
using Frontend.WPF.Views;
using Frontend.WPF.Views.Windows;
using Backend.BusinessLayer;
using Backend.Common;

namespace Frontend.WPF.ViewModels
{
    public class OperationDialogViewModel : NotifyingEntityBase
    {
        private string _operationTitle;
        private string _cancelTitle;
        private Control _contentView;

        public OperationDialogViewModel(IView view)
        {
            OperationCommand = new FunctionWithParameterCommand(PerformOperation)
                                            .SetPostExecuteAction(view.CloseView);
            CancelCommand = new FunctionWithParameterCommand(CancelOperation)
                                            .SetPostExecuteAction(view.CloseView);
            OperationTitle = "Perform";
            CancelTitle = "Cancel";
        }

        public ICommand OperationCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string OperationTitle
        {
            get { return _operationTitle; }
            set { SetProperty(ref _operationTitle, value); }
        }

        public string CancelTitle
        {
            get { return _cancelTitle; }
            set { SetProperty(ref _cancelTitle, value); }
        }

        public Control ContentView
        {
            get { return _contentView; }
            set { SetProperty(ref _contentView, value); }
        }

        public bool PerformOperation(object parameter)
        {
            try
            {
                var operationViewModel = parameter as IOperationViewModel;
                if(operationViewModel == null)
                    throw new ArgumentException("IOperationViewModel expected.");
                if (!operationViewModel.PerformOperationEnabled)
                    return false;
                operationViewModel.PerformOperation();
                return true;
            }
            catch (Exception ex)
            {
                var viewModel = new OperationFailedConfirmDialogViewModel(ex);
                var actionResult = new ActionResult().AddError(viewModel.Message);
                var operationScopeContext = ServiceLocator.Get<OperationScopeContext>();
                operationScopeContext.SetActionResultAsync(actionResult);
                new ConfirmDialogView(viewModel).ShowDialog();
                return viewModel.Result == ConfirmDialogViewModel.DialogResult.Confirm;
            }
        }

        public bool CancelOperation(object parameter)
        {
            var operationViewModel = parameter as IOperationViewModel;
            if(operationViewModel == null)
                throw new ArgumentException("IOperationViewModel expected.");
            return operationViewModel.CancelOperationEnabled;
        }
    }
}