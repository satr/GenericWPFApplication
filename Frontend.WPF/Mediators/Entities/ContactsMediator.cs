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
using System.Collections.Generic;
using System.Windows.Controls;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.Mediators.Operations.Contacts;
using Frontend.WPF.ViewModels.Contacts;
using Frontend.WPF.Views.Contacts;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Backend.Common;

namespace Frontend.WPF.Mediators.Entities
{
    public class ContactsMediator: EntitiesMediatorBase
    {
        private UserControl _view;
        private EditContactOperationMediator _editContactOperationMediator;
        private ContactsViewModel _viewModel;

        public override string Name
        {
            get { return "Contacts"; }
        }

        protected override IEnumerable<IOperationMediator> CreateOperationMediators()
        {
            yield return new AddContactOperationMediator();
            yield return GetEditContactOperationMediator();
            yield return new DeleteContactOperationMediator();
        }

        private EditContactOperationMediator GetEditContactOperationMediator()
        {
            return _editContactOperationMediator?? (_editContactOperationMediator = new EditContactOperationMediator());
        }

        public override UserControl GetView()
        {
            return _view ?? (_view = CreateView());
        }

        public override void Initialize()
        {
            ViewModel.Initialize();
        }

        private ContactsView CreateView()
        {
            return new ContactsView(ViewModel);
        }

        private ContactsViewModel ViewModel
        {
            get
            {
                return _viewModel?? (_viewModel = CreateEntitiesViewModel());
            }
        }

        protected ContactsViewModel CreateEntitiesViewModel()
        {
            return new ContactsViewModel(ServiceLocator.Get<OperationScopeContext>(),
                ServiceLocator.Get<ContactsRepository>(),
                GetEditContactOperationMediator())
            {
                DetailView = new ContactDetailsView(new ViewContactDetailsViewModel())
            };
        }
    }
}