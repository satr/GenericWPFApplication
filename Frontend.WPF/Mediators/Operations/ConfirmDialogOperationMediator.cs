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
using Frontend.WPF.ViewModels;
using Frontend.WPF.Views.Windows;

namespace Frontend.WPF.Mediators.Operations
{
    public class ConfirmDialogOperationMediator : OperationMediatorBase
    {
        private ConfirmDialogViewModel _viewModel;
        private ConfirmDialogView _dialogView;
        private bool _disposed;

        protected ConfirmDialogOperationMediator(string name, string description, string imagePath = null) : base(name, description, imagePath)
        {
        }

        public override void PerformOperation()
        {
            try
            {
                _dialogView = new ConfirmDialogView(ViewModel);
                _dialogView.ShowDialog();
            }
            finally
            {
                _dialogView = null;
            }
        }

        public ConfirmDialogViewModel ViewModel
        {
            get
            {
                return _viewModel?? (_viewModel = CreateViewModel());
            }
        }

        protected virtual ConfirmDialogViewModel CreateViewModel()
        {
            return new ConfirmDialogViewModel(Name, Description);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                try
                {
                    if (_dialogView != null && _dialogView.IsLoaded)
                        _dialogView.Close();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception) { }
            }
            base.Dispose(disposing);
            _disposed = true;
        }
    }
}