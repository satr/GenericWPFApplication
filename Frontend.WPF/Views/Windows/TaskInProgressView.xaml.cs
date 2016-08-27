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
using System.ComponentModel;
using System.Windows;
using Frontend.WPF.ViewModels;

namespace Frontend.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for TaskInProgressView.xaml
    /// </summary>
    public partial class TaskInProgressView: IView
    {
        private readonly Action _cancellationAction;

        public TaskInProgressView(string title, Action cancellationAction = null)
        {
            _cancellationAction = cancellationAction;
            InitializeComponent();
            var viewModel = new TaskInProgressViewModel(this, title, cancellationAction);
            DataContext = viewModel;
            Closing += OnClosing;
            CancelOperationButton.Visibility = cancellationAction == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (_cancellationAction != null)
                _cancellationAction();
        }

        public void CloseView()
        {
            Close();
        }
    }
}
