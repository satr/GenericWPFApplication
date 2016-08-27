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
using System.Collections.Specialized;
using System.Linq;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Backend.Common;
using Frontend.WPF.Mediators.Operations.Emails;
using Frontend.WPF.ViewModels.Emails;

namespace Frontend.WPF.Commands.Email
{
    public class AddEmailAddressCommand:CommandBase
    {
        private readonly AddContactsToEntityOperationMediator _addContactsOperationMediator;
        private ObservableCollection<ContactEmailViewModel> _contactEmailViewModels;
        private readonly OperationScopeContext _operationScopeContext;

        public AddEmailAddressCommand()
        {
            _operationScopeContext = new OperationScopeContext();
            _addContactsOperationMediator = new AddContactsToEntityOperationMediator(_operationScopeContext, ServiceLocator.Get<ContactsRepository>());
            ContactEmailViewModels = new ObservableCollection<ContactEmailViewModel>();
        }

        public ObservableCollection<ContactEmailViewModel> ContactEmailViewModels
        {
            private get { return _contactEmailViewModels; }
            set
            {
                _contactEmailViewModels = value;
                InitContactsInMediator();
            }
        }

        private void InitContactsInMediator()
        {
            if (_addContactsOperationMediator.Contacts != null)
                _addContactsOperationMediator.Contacts.CollectionChanged -= ContactsOnCollectionChanged;
            var contacts = new ObservableCollection<Contact>();
            foreach (var contactEmailViewModel in ContactEmailViewModels)
            {
                contacts.Add(contactEmailViewModel.Contact);
            }
            _addContactsOperationMediator.Contacts = contacts;
            _addContactsOperationMediator.Contacts.CollectionChanged += ContactsOnCollectionChanged;
        }

        private void ContactsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            AddViewModelsForAddedContacts(notifyCollectionChangedEventArgs);
            RemoveViewModelsForRemovedContacts(notifyCollectionChangedEventArgs);
        }

        private void RemoveViewModelsForRemovedContacts(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action != NotifyCollectionChangedAction.Remove) 
                return;
            foreach (var contact in notifyCollectionChangedEventArgs.OldItems.OfType<Contact>())
            {
                foreach (var contactEmailViewModel in ContactEmailViewModels.Where(i => i.Contact == contact).ToList())
                {
                    ContactEmailViewModels.Remove(contactEmailViewModel);
                }
            }
        }

        private void AddViewModelsForAddedContacts(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action != NotifyCollectionChangedAction.Add) 
                return;
            foreach (var contact in notifyCollectionChangedEventArgs.NewItems.OfType<Contact>()
                .Where(contact => ContactEmailViewModels.All(i => i.Contact != contact)))
            {
                ContactEmailViewModels.Add(new ContactEmailViewModel(contact, ContactEmailViewModels));
            }
        }

        protected override bool CommandAction(object parameter)
        {
            _operationScopeContext.Clear<Contact>();
            _addContactsOperationMediator.PerformOperation();
            return true;
        }
    }
}