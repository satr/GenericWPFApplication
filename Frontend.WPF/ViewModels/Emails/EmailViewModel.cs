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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Contacts;
using Backend.BusinessLayer.Documents;
using Backend.BusinessLayer.Email;
using Backend.Common;
using Frontend.WPF.Common;
using Frontend.WPF.Mediators;
using Frontend.WPF.Mediators.Entities;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.Mediators.Operations.Emails;
using Frontend.WPF.ViewModels.Documents;
using Frontend.WPF.Views.Contacts;

namespace Frontend.WPF.ViewModels.Emails
{
    public class EmailViewModel<TDocument> : NotifyingEntityBase, IOperationViewModel, IEmailViewModel 
        where TDocument: Document, new()
    {
        private EmailMessage _emailMessage;
        private readonly Collection<Document> _selectedDocuments = new Collection<Document>();
        private readonly AddDocumentsToEmailOperationMediator<TDocument> _addDocumentsOperationMediator;
        private readonly OperationScopeContext _emailOperationScopeContext;
        private readonly EntityCollectionRepository<TDocument, Guid> _documentsRepository;
        private DocumentsViewModel<TDocument> _documentsDataContext;
        private ObservableCollection<OperationViewModel> _toolbarSource;
        private string _title;
        private EmailAddressViewModel _toEmailAddressDataContext;
        private EmailAddressViewModel _ccEmailAddressDataContext;
        private EmailAddressViewModel _bccEmailAddressDataContext;

        public EmailViewModel()
        {
            _emailOperationScopeContext = new OperationScopeContext();
            _documentsRepository = new EntityCollectionRepository<TDocument, Guid>();
            DocumentsDataContext = new DocumentsViewModel<TDocument>(_emailOperationScopeContext, _documentsRepository);
            var documentsMediator = new DocumentsMediator<TDocument>(_emailOperationScopeContext, _documentsRepository);
            OperationScopeContext = ServiceLocator.Get<OperationScopeContext>();
            _addDocumentsOperationMediator = new AddDocumentsToEmailOperationMediator<TDocument>(OperationScopeContext);
            PopulateToolbar(documentsMediator, _addDocumentsOperationMediator);
            ToEmailAddressDataContext = new EmailAddressViewModel();
            CcEmailAddressDataContext = new EmailAddressViewModel();
            BccEmailAddressDataContext = new EmailAddressViewModel();
        }

        protected OperationScopeContext OperationScopeContext { get; set; }

        public EmailAddressViewModel BccEmailAddressDataContext
        {
            get { return _bccEmailAddressDataContext; }
            set { SetProperty(ref _bccEmailAddressDataContext, value); }
        }

        public EmailAddressViewModel CcEmailAddressDataContext
        {
            get { return _ccEmailAddressDataContext; }
            set { SetProperty(ref _ccEmailAddressDataContext, value); }
        }

        public EmailAddressViewModel ToEmailAddressDataContext
        {
            get { return _toEmailAddressDataContext; }
            set { SetProperty(ref _toEmailAddressDataContext, value); }
        }

        public DocumentsViewModel<TDocument> DocumentsDataContext
        {
            get { return _documentsDataContext; }
            set { SetProperty(ref _documentsDataContext, value); }
        }

        private void PopulateToolbar(IMediator documentsMediator, IOperationMediator addDocumentsToOrderOperationMediator)
        {
            ToolbarSource = new ObservableCollection<OperationViewModel>();
            foreach (var operationMediator in documentsMediator.OperationMediators)
            {
                ToolbarSource.Add(new OperationViewModel(operationMediator));
            }
            ToolbarSource.Add(new OperationViewModel(addDocumentsToOrderOperationMediator));
        }

        public ObservableCollection<OperationViewModel> ToolbarSource
        {
            get { return _toolbarSource; }
            set { SetProperty(ref _toolbarSource, value); }
        }

        public virtual bool ValidateAndInitialize()
        {
            EmailMessage = CreateEmailMessage();
            _emailOperationScopeContext.Clear<Document>();
            _addDocumentsOperationMediator.AttachedDocuments
                = DocumentsDataContext.Entities
                = _documentsRepository.Entities = new ObservableCollection<TDocument>(GetDocuments());
            ToEmailAddressDataContext.ContactsSource = new ObservableCollection<ContactEmailViewModel>();
            CcEmailAddressDataContext.ContactsSource = new ObservableCollection<ContactEmailViewModel>();
            BccEmailAddressDataContext.ContactsSource = new ObservableCollection<ContactEmailViewModel>();
            return true;
        }

        public void PerformOperation()
        {
            foreach (var document in DocumentsDataContext.Entities)
            {
                EmailMessage.AttachedItems.Add(new AttachedItem{Content = document});
            }
            SetAddresses(EmailMessage.ToAddresses, ToEmailAddressDataContext.ContactsSource, ToEmailAddressDataContext.CustomAddresses);
            SetAddresses(EmailMessage.CcAddresses, CcEmailAddressDataContext.ContactsSource, CcEmailAddressDataContext.CustomAddresses);
            SetAddresses(EmailMessage.BccAddresses, BccEmailAddressDataContext.ContactsSource, BccEmailAddressDataContext.CustomAddresses);
            try
            {
                ServiceLocator.Get<EmailController>().SendEmail(EmailMessage);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException(string.Format("Some email addresses have invalid format.\r\n{0}", ex.Message), ex);
            }
        }

        public bool PerformOperationEnabled
        {
            get { return true; }
        }

        public bool CancelOperationEnabled
        {
            get { return true; }
        }

        private static void SetAddresses(ICollection<Contact> addresses, IEnumerable<ContactEmailViewModel> contactEmailViewModels, string customAddresses)
        {
            foreach (var contactEmailViewModel in contactEmailViewModels)
            {
                addresses.Add(contactEmailViewModel.Contact);
            }
            if (string.IsNullOrWhiteSpace(customAddresses))
                return;
            foreach (var emailAddress in customAddresses.Split(';'))
            {
                //TODO add validation of email addresses
                addresses.Add(new Contact { Name = emailAddress, Email = emailAddress, CreatedOn = DateTimeOffset.Now });
            }
        }

        public EmailMessage EmailMessage
        {
            get { return _emailMessage; }
            set { SetProperty(ref _emailMessage, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public void SetSelectedDocuments(IList addedItems, IList removedItems)
        {
            _selectedDocuments.Update(addedItems, removedItems);
            _emailOperationScopeContext.SelectItemsAsync<Document>(_selectedDocuments);
        }

        protected virtual IEnumerable<TDocument> GetDocuments()
        {
            return new TDocument[0];
        }

        private EmailMessage CreateEmailMessage()
        {
            var emailMessage = new EmailMessage
            {
                ToAddresses = new ObservableCollection<Contact>(),
                CcAddresses = new ObservableCollection<Contact>(),
                BccAddresses = new ObservableCollection<Contact>(),
                Subject = CreateSubject() ?? string.Empty,
                Message = CreateMessage() ?? string.Empty
            };
            return emailMessage;
        }

        protected virtual string CreateSubject()
        {
            return null;
        }

        protected virtual string CreateMessage()
        {
            return null;
        }
    }
}