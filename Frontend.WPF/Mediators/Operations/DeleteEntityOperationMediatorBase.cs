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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Frontend.WPF.ViewModels;
using Backend.BusinessLayer;

namespace Frontend.WPF.Mediators.Operations
{
    public abstract class DeleteEntityOperationMediatorBase<TEntity, TKey, TViewModel> : ConfirmDialogOperationMediator, IOperationScopeContextSubscriber 
        where TViewModel : DeleteEntityConfirmDialogViewModelBase<TEntity>, new()
    {
        private readonly TViewModel _viewModel;
        private readonly ICollectionRepository<TEntity, TKey> _repository;
        private readonly IOperationScopeContext _operationScopeContext;

        protected DeleteEntityOperationMediatorBase(string name, string description, string imagePath, IOperationScopeContext operationScopeContext, ICollectionRepository<TEntity, TKey> repository)
            : base(name, description, imagePath)
        {
            _repository = repository;
            _operationScopeContext = operationScopeContext;
            _operationScopeContext.SubscribeOnSelected<TEntity>(this);
            _viewModel = new TViewModel();
            Enabled = false;
        }

        protected override ConfirmDialogViewModel CreateViewModel()
        {
            return _viewModel;
        }

        public override void PerformOperation()
        {
            base.PerformOperation();
            if(ViewModel.Result != ConfirmDialogViewModel.DialogResult.Confirm)
                return;
            var entitiesToDelete = GetEntitiesToDelete().ToList();
            foreach (var entity in entitiesToDelete)
            {
                 _operationScopeContext.SetActionResultAsync(_repository.Remove(entity));
            }
        }

        protected IEnumerable<TEntity> GetEntitiesToDelete()
        {
            return _viewModel.Entities.ToList();
        }

        public void NotifyItemsSelected(ICollection items)
        {
            _viewModel.Entities = items.OfType<TEntity>().ToList();
            Enabled = _viewModel.Entities.Count > 0;
        }
    }
}