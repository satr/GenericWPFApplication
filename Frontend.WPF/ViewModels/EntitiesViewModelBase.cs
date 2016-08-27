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
using System.Linq;
using Frontend.WPF.Mediators.Operations;
using Backend.BusinessLayer;
using Frontend.WPF.Common;

namespace Frontend.WPF.ViewModels
{
    public abstract class EntitiesViewModelBase<TEntity, TKey> : EntityBase, IOperationScopeContextSubscriber, IEntitiesViewModel
    {
        private readonly IOperationScopeContext _operationScopeContext;
        private readonly ICollectionRepository<TEntity, TKey> _repository;
        private readonly IOperationMediator _editEntityOperationMediator;
        private ObservableCollection<TEntity> _entities;
        private bool _suppressNotifySelectedItems;
        public event Action<ICollection> ForceSelectItems;

        protected EntitiesViewModelBase(IOperationScopeContext operationScopeContext, 
                                        ICollectionRepository<TEntity, TKey> repository, 
                                        IOperationMediator editEntityOperationMediator)
        {
            SelectedEntities = new Collection<TEntity>();
            Entities = new ObservableCollection<TEntity>();
            _repository = repository;
            _repository.Added += OnEntitiesRepositoryOnAdded;
            _repository.Removed += OnEntitiesRepositoryOnRemoved;
            _operationScopeContext = operationScopeContext;
            _editEntityOperationMediator = editEntityOperationMediator;
            _operationScopeContext.SubscribeOnSelected<TEntity>(this);
        }

        public void Initialize()
        {
            Entities = new ObservableCollection<TEntity>(_repository.GetList());
        }

        private void OnEntitiesRepositoryOnAdded(TEntity entity)
        {
            if (Entities.Contains(entity))
                return;
            Entities.Add(entity);
        }

        private void OnEntitiesRepositoryOnRemoved(TEntity order)
        {
            if (Entities.Contains(order))
                Entities.Remove(order);
        }

        public ObservableCollection<TEntity> Entities
        {
            get { return _entities; }
            set
            {
                SetProperty(ref _entities, value);
                SelectedEntities.Clear();
            }
        }

        public void ClearSelection()
        {
            var entities = Entities;
            Entities = null;
            Entities = entities;
        }

        public virtual TEntity CurrentEntity { get; set; }

        public void NotifyItemsSelected(ICollection items)
        {
            OnForceSelectItems(items.OfType<TEntity>().ToList());
            var firstSelectedEntity = items.OfType<TEntity>().FirstOrDefault();
            if (!Equals(firstSelectedEntity, null) && Equals(CurrentEntity, firstSelectedEntity))
                return;
            CurrentEntity = firstSelectedEntity;
        }

        private void OnForceSelectItems(ICollection obj)
        {
            try
            {
                _suppressNotifySelectedItems = true;
                var handler = ForceSelectItems;
                if (handler != null) 
                    handler(obj);
            }
            finally
            {
                _suppressNotifySelectedItems = false;
            }
        }

        public void SetSelectedItems(IList addedItems, IList removedItems)
        {
            if (_suppressNotifySelectedItems)
                return;
            SelectedEntities.Update(addedItems, removedItems);
            _operationScopeContext.SelectItemsAsync<TEntity>(SelectedEntities);
        }

        public void SetSelectedItem(TEntity entity)
        {
            foreach (var item in SelectedEntities.ToList().Where(item => !Equals(item, entity)))
            {
                SelectedEntities.Remove(item);
            }
            if(!SelectedEntities.Contains(entity))
                SelectedEntities.Add(entity);
            _operationScopeContext.SelectItemsAsync<TEntity>(SelectedEntities);
            if(_editEntityOperationMediator != null)
                _editEntityOperationMediator.PerformOperation();
        }

        protected Collection<TEntity> SelectedEntities { get; private set; }
    }
}