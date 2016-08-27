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
using System.Collections.ObjectModel;
using System.Linq;

namespace Backend.BusinessLayer
{
    public class EntityCollectionRepository<TEntity, TKey> : ICollectionRepository<TEntity, TKey>
        where TEntity: UniqueEntityBase<TKey>
    {
        private ObservableCollection<TEntity> _entities = new ObservableCollection<TEntity>();

        public virtual ObservableCollection<TEntity> Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public IActionResult Save(TEntity entity)
        {
            var foundEntity = Get(entity.ID);
            if (foundEntity == null)
            {
                Entities.Add(entity);
                OnAction(entity, Added);
                return new ActionResult("{0} saved.", EntityName);
            }
            var index = Entities.IndexOf(foundEntity);
            Entities.Remove(foundEntity);
            Entities.Insert(index, entity);
            return new ActionResult("{0} added.", EntityName);
        }

        public virtual string EntityName { get { return typeof(TEntity).ToString(); }}

        public IActionResult Remove(TEntity entity)
        {
            if (!Entities.Contains(entity))
            {
                return new ActionResult().AddError("{0} not removed - such entity does not exist.", EntityName);
            }
            Entities.Remove(entity);
            OnAction(entity, Removed);
            return new ActionResult("{0} removed.", EntityName);
        }

        public TEntity Get(TKey key)
        {
            return Entities.FirstOrDefault(entity => Equals(entity.ID, key));
        }

        public IEnumerable<TEntity> GetList()
        {
            return Entities;
        }

        public event Action<TEntity> Added;
        public event Action<TEntity> Removed;

        private static void OnAction(TEntity entity, Action<TEntity> action)
        {
            if (action != null)
                action(entity);
        }
    }
}