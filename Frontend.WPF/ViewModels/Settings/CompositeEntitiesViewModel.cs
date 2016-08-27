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
using System.Collections.ObjectModel;
using System.Linq;
using Frontend.WPF.Mediators;
using Frontend.WPF.Mediators.Entities;
using Frontend.WPF.Views.Contacts;

namespace Frontend.WPF.ViewModels.Settings
{
    public class CompositeEntitiesViewModel : IOperationViewModel
    {
        private readonly List<IOperationViewModel> _operationViewModels;

        public CompositeEntitiesViewModel(ICompositeMediator compositeMediator)
        {
            var entitiesMediators = compositeMediator.EntitiesMediators.ToList();
            _operationViewModels = entitiesMediators.OfType<ICompositeItemMediator>().Select(m => m.OperationViewModel).ToList();
            ToolbarSource = CreateToolbarSource(entitiesMediators, compositeMediator);
        }

        public ObservableCollection<OperationViewModel> ToolbarSource { get; set; }

        private static ObservableCollection<OperationViewModel> CreateToolbarSource(IEnumerable<IEntitiesMediator> entitiesMediators, IMediator compositeMediator)
        {
            var toolbarSource = new ObservableCollection<OperationViewModel>();
            foreach (var mediator in entitiesMediators.SelectMany(moduleMediator => moduleMediator.OperationMediators))
            {
                toolbarSource.Add(new OperationViewModel(mediator));
            }
            foreach (var mediator in compositeMediator.OperationMediators)
            {
                toolbarSource.Add(new OperationViewModel(mediator));
            }
            return toolbarSource;
        }

        public bool ValidateAndInitialize()
        {
            return _operationViewModels.All(compositeItemMediator => compositeItemMediator.ValidateAndInitialize());
        }

        public void PerformOperation()
        {
            foreach (var compositeItemMediator in _operationViewModels)
                compositeItemMediator.PerformOperation();
        }

        public bool PerformOperationEnabled
        {
            get { return _operationViewModels.All(m => m.PerformOperationEnabled); }
        }

        public bool CancelOperationEnabled
        {
            get { return _operationViewModels.All(m => m.CancelOperationEnabled); }
        }
    }
}