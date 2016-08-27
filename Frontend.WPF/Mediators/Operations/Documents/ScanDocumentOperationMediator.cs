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
using System.Windows.Controls;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Documents;
using Backend.BusinessLayer.Settings;
using Frontend.WPF.ViewModels.Documents;
using Frontend.WPF.Views.Documents;
using Frontend.WPF.Views.Windows;

namespace Frontend.WPF.Mediators.Operations.Documents
{
    public class ScanDocumentOperationMediator<TDocument> : ShowContentViewInSingleWindowOperationMediatorBase
        where TDocument: Document, new()
    {
        private readonly ICollectionRepository<TDocument, Guid> _documentsRepository;
        private ScanDocumentViewModel<TDocument> _viewModel;
        private readonly ScanSettings _scanSettings;

        public ScanDocumentOperationMediator(ICollectionRepository<TDocument, Guid> documentsRepository)
            : base("Scan Document", "Scan Document", @"PresentationLayer/Images/ScanDocument.png")
        {
            _documentsRepository = documentsRepository;
            _scanSettings = new ScanSettings();
        }

        protected override bool ValidateAndInitialize()
        {
            return ViewModel.ValidateAndInitialize();
        }

        protected override Control CreateContentView()
        {
            return new ScanDocumentView{DataContext = ViewModel};
        }

        protected override bool DialogModel
        {
            get { return false; }
        }

        protected override void CreateDialogViewPostProcess(ContentOperationDialogView windowView)
        {
            base.CreateDialogViewPostProcess(windowView);
            windowView.Height = 600;
            windowView.Width = 600;
        }

        public ScanDocumentViewModel<TDocument> ViewModel
        {
            get { return _viewModel ?? (_viewModel = new ScanDocumentViewModel<TDocument>(_documentsRepository, _scanSettings)); }
        }


    }
}