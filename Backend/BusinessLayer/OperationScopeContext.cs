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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.BusinessLayer
{
    public class OperationScopeContext : IOperationScopeContext
    {
        private readonly IDictionary<Type, ICollection<OperationScopeSubscriberContainer<IOperationScopeContextSubscriber>>> _selectedItemsSubscriberContainers 
                                                = new Dictionary<Type, ICollection<OperationScopeSubscriberContainer<IOperationScopeContextSubscriber>>>();
        private readonly ICollection<OperationScopeSubscriberContainer<IOperationScopeActionResultSubscriber>> _actionResultsSubscriberContexts 
                                                = new List<OperationScopeSubscriberContainer<IOperationScopeActionResultSubscriber>>();
        private readonly IDictionary<Type, ICollection> _lastSelectedItems = new Dictionary<Type, ICollection>();
        private readonly object _selectedItemsSubscribersSync = new object();
        private readonly object _actionResultsSubscribersSync = new object();
        private readonly object _selectedItemsSync = new object();

        public async void SelectItemsAsync<T>(ICollection items)
        {
            var type = typeof(T);
            ICollection list = items == null ? Enumerable.Empty<T>().ToList() : items.OfType<T>().ToList();
            if (!_lastSelectedItems.ContainsKey(type))
                _lastSelectedItems.Add(type, list);
            else
                _lastSelectedItems[type] = list;

            List<OperationScopeSubscriberContainer<IOperationScopeContextSubscriber>> subscriberContainers;
            lock (_selectedItemsSubscribersSync)
            {
                if (!_selectedItemsSubscriberContainers.ContainsKey(type))
                    return;
                subscriberContainers = _selectedItemsSubscriberContainers[type].ToList();
            }
            foreach (var subscriberContainer in subscriberContainers)
            {
                var container = subscriberContainer;
                await Task.Factory.StartNew(() => container.Subscriber.NotifyItemsSelected(list),
                                                  CancellationToken.None, TaskCreationOptions.None, container.TaskScheduler);
            }
        }

        public void SelectItemAsync<T>(T item)
        {
            SelectItemsAsync<T>(new []{item});
        }

        public IEnumerable<T> GetLastSelectedItems<T>()
        {
            var type = typeof(T);
            lock (_selectedItemsSync)
            {
                if (!_lastSelectedItems.ContainsKey(type))
                    _lastSelectedItems.Add(type, new T[0]);
            }
            return _lastSelectedItems[type].OfType<T>();
        }

        public void SubscribeOnSelected<T>(IOperationScopeContextSubscriber subscriber)
        {
            var type = typeof(T);
            lock (_selectedItemsSubscribersSync)
            {
                if(!_selectedItemsSubscriberContainers.ContainsKey(type))
                    _selectedItemsSubscriberContainers.Add(type, new List<OperationScopeSubscriberContainer<IOperationScopeContextSubscriber>>());

                if (_selectedItemsSubscriberContainers[type].All(i => i.Subscriber != subscriber))
                    _selectedItemsSubscriberContainers[type].Add(new OperationScopeSubscriberContainer<IOperationScopeContextSubscriber>(subscriber, 
                                                                                                      TaskScheduler.FromCurrentSynchronizationContext()));
            }
        }

        public void Clear<T>()
        {
            SelectItemsAsync<T>(new T[0]);
        }

        public void SubscribeOnActionResult(IOperationScopeActionResultSubscriber subscriber)
        {
            lock (_actionResultsSubscribersSync)
            {
                if(_actionResultsSubscriberContexts.All(i => i.Subscriber != subscriber))
                    _actionResultsSubscriberContexts.Add(new OperationScopeSubscriberContainer<IOperationScopeActionResultSubscriber>(subscriber, TaskScheduler.FromCurrentSynchronizationContext()));
            }
        }

        public void SetActionResultAsync(IActionResult actionResult)
        {
            List<OperationScopeSubscriberContainer<IOperationScopeActionResultSubscriber>> subscriberContainers;
            lock (_actionResultsSubscribersSync)
            {
                subscriberContainers = _actionResultsSubscriberContexts.ToList();
            }
            foreach (var subscriberContainer in subscriberContainers)
            {
                var container = subscriberContainer;
                Task.Factory.StartNew(() => container.Subscriber.NotifyWithActionResult(actionResult),
                                                  CancellationToken.None, TaskCreationOptions.None, container.TaskScheduler);
            }
        }
    }
}
