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
using System.Collections.Generic;
using System.Windows.Controls;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Documents;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.Mediators.Operations.Documents;
using Frontend.WPF.ViewModels.Documents;
using Frontend.WPF.Views.Documents;

namespace Frontend.WPF.Mediators.Entities
{
    public class DocumentsMediator<TDocument>: EntitiesMediatorBase
        where TDocument: Document, new()
    {
        private UserControl _moduleView;
        private readonly ICollectionRepository<TDocument, Guid> _documentsRepository;
        private readonly IOperationScopeContext _operationScopeContext;
        private DocumentsViewModel<TDocument> _viewModel;

        public DocumentsMediator(IOperationScopeContext operationScopeContext, ICollectionRepository<TDocument, Guid> documentsRepository)
        {
            _operationScopeContext = operationScopeContext;
            _documentsRepository = documentsRepository;
        }

        public override string Name
        {
            get { return "Documents"; }
        }

        protected override IEnumerable<IOperationMediator> CreateOperationMediators()
        {
            yield return new ScanDocumentOperationMediator<TDocument>(_documentsRepository);
            yield return new DeleteDocumentOperationMediator<TDocument>(_operationScopeContext, _documentsRepository);
        }

        public override UserControl GetView()
        {
            return _moduleView ?? (_moduleView = new DocumentsView().SetViewModel(ViewModel));
        }

        private DocumentsViewModel<TDocument> ViewModel
        {
            get
            {
                return _viewModel?? (_viewModel = new DocumentsViewModel<TDocument>(_operationScopeContext, _documentsRepository));
            }
        }

        public override void Initialize()
        {
            ViewModel.Initialize();
        }
    }
}
