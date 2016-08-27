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
using System.Data.Entity;
using System.Transactions;
using Backend.BusinessLayer;
using Backend.Common;

namespace Backend.DataLayer
{
    public abstract class EntityRepositoryBase<TDbContext, TEntity, TKey>
        : ICollectionRepository<TEntity, TKey> 
        where TDbContext: DatabaseContextBase, new()
        where TEntity: UniqueEntityBase<TKey>
    {
        protected TDbContext Context { get { return ServiceLocator.Get<TDbContext>(); } }

        public IActionResult Save(TEntity entity)
        {
            var entityState = Exists(entity) ? EntityState.Modified : EntityState.Added;
            SaveChanges(entity, entityState);
            if (Added != null)
                Added(entity);
            return new ActionResult("{0} {1}.", EntityName, entityState == EntityState.Added? "added": "saved");
        }

        public abstract string EntityName { get; }

        public IActionResult Remove(TEntity entity)
        {
            if (!Exists(entity))
                return new ActionResult().AddError("{0} not removed - such entity does not exist.", EntityName);
            SaveChanges(entity, EntityState.Deleted);
            if (Removed != null)
                Removed(entity);
            return new ActionResult("{0} removed.", EntityName);
        }

        public abstract TEntity Get(TKey key);
        public abstract IEnumerable<TEntity> GetList();

        public event Action<TEntity> Added;
        public event Action<TEntity> Removed;

        private bool Exists(TEntity entity)
        {
            return Get(entity.ID) != null;
        }

        private void SaveChanges(TEntity entity, EntityState entityState)
        {
            if (!Context.Valid)
                throw new Exception("Database is not valid or unavailable.\r\nPlease try later.");
            using (var transactionScope = new TransactionScope())
            {
                Context.Entry(entity).State = entityState;
                Context.SaveChanges();
                transactionScope.Complete();
            }
        }
    }
}