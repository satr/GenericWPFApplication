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
using Frontend.WPF.Commands;
using Frontend.WPF.Views;
using Backend.BusinessLayer;

namespace Frontend.WPF.ViewModels
{
    public class ConfirmDialogViewModel: NotifyingEntityBase
    {
        private static bool _viewClosedByCommand;
        private IView _view;
        private readonly string _title;
        private readonly string _message;

        public ConfirmDialogViewModel(string title, string message, bool cancelAvailable = false, bool rejectAvailable = false)
            :this(cancelAvailable, rejectAvailable)
        {
            _title = title;
            _message = message;
        }

        public ConfirmDialogViewModel(bool cancelAvailable = false, bool rejectAvailable = false)
        {
            CancelAvailable = cancelAvailable;
            RejectAvailable = rejectAvailable;
            Result = DialogResult.Cancel;
            ConfirmCommand = new ActionWithParameterCommand(ConfirmOperation);
            RejectCommand = new ActionWithParameterCommand(RejectOperation);
            CancelCommand = new ActionWithParameterCommand(CancelOperation);
        }

        public bool RejectAvailable { get; private set; }

        public bool CancelAvailable { get; private set; }

        protected virtual void ConfirmOperation(object obj){}

        protected virtual void RejectOperation(object obj){}
        
        protected virtual void CancelOperation(object obj) { }

        public IView View {
            set
            {
                _view = value;
                ConfirmCommand.SetPostExecuteAction(() => CloseViewByCommand(DialogResult.Confirm));
                RejectCommand.SetPostExecuteAction(() => CloseViewByCommand(DialogResult.Reject));
                CancelCommand.SetPostExecuteAction(() => CloseViewByCommand(DialogResult.Cancel));
            } 
        }

        private void CloseViewByCommand(DialogResult dialogResult)
        {
            _viewClosedByCommand = true;
            Result = dialogResult;
            if (_view != null)
                _view.CloseView();
        }

        public void CloseView()
        {
            if (_viewClosedByCommand)
                return;
            Result = CancelAvailable
                        ? DialogResult.Cancel
                        : RejectAvailable 
                            ? DialogResult.Reject 
                            : DialogResult.Confirm;
        }

        public DialogResult Result { get; private set; }

        public virtual string ConfirmTitle { get { return "Yes"; } }

        public virtual string RejectTitle { get { return "No"; } }

        public virtual string CancelTitle { get { return "Cancel"; } }

        public virtual string Title
        {
            get { return _title; }
        }

        public virtual string Message { get { return _message; }}

        public ActionWithParameterCommand ConfirmCommand { get; set; }

        public ActionWithParameterCommand RejectCommand { get; set; }

        public ActionWithParameterCommand CancelCommand { get; set; }

        public enum DialogResult
        {
            Confirm,
            Reject,
            Cancel
        };
    }
}
