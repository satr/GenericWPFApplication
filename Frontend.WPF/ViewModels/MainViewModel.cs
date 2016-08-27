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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Frontend.WPF.Mediators;
using Frontend.WPF.Mediators.Entities;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.Views;
using Backend.BusinessLayer;
using Frontend.WPF.Views.Contacts;

namespace Frontend.WPF.ViewModels
{
    public class MainViewModel : NotifyingEntityBase
    {
        private ImageSource _imageSource;
        private string _title;

        public MainViewModel(IView view, ICompositeMediator applicationMediator)
        {
            Title = applicationMediator.Name;
            var moduleMediators = applicationMediator.EntitiesMediators.ToList();
            MainMenuSource = CreateMainMenuSource(moduleMediators, applicationMediator, view);
            ToolbarSource = CreateToolbarSource(moduleMediators, applicationMediator);
            StatusBarSource = CreateToolbarSourceStatusBarSource(applicationMediator);
            
        }

        private static ObservableCollection<StatusBarItem> CreateToolbarSourceStatusBarSource(ICompositeMediator applicationMediator)
        {
            var statusBarItems = new ObservableCollection<StatusBarItem>
            {
                new StatusBarItem {Content = applicationMediator.ApplicationStatusView}
            };
            return statusBarItems;
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private static ObservableCollection<OperationViewModel> CreateToolbarSource(IEnumerable<IEntitiesMediator> moduleMediators, IMediator applicationMediator)
        {
            var toolbarSource = new ObservableCollection<OperationViewModel>();
            foreach (var mediator in moduleMediators.SelectMany(moduleMediator => moduleMediator.OperationMediators))
            {
                toolbarSource.Add(new OperationViewModel(mediator));
            }
            foreach (var mediator in applicationMediator.OperationMediators)
            {
                toolbarSource.Add(new OperationViewModel(mediator));
            }
            return toolbarSource;
        }

        private static ObservableCollection<MenuItem> CreateMainMenuSource(IEnumerable<IEntitiesMediator> moduleMediators, IMediator applicationMediator, IView view)
        {
            var mainMenuSource = new ObservableCollection<MenuItem>();

            // ReSharper disable once UseObjectOrCollectionInitializer
            var fileMenuOperationMediators = new List<IOperationMediator>(applicationMediator.OperationMediators);
            fileMenuOperationMediators.Add(new CloseViewOperationMediator(view));
            mainMenuSource.Add(CreateMenuItem("File", fileMenuOperationMediators));
            
            foreach (var moduleMediator in moduleMediators)
            {
                mainMenuSource.Add(CreateMenuItem(moduleMediator.Name, moduleMediator.OperationMediators));
            }
            return mainMenuSource;
        }

        private static MenuItem CreateMenuItem(string header, IEnumerable<IOperationMediator> operationMediators)
        {
            var menuItem = new MenuItem {Header = header};
            foreach (var operationMediator in operationMediators)
            {
                menuItem.Items.Add(CreateMenuItem(operationMediator));
            }
            return menuItem;
        }

        private static MenuItem CreateMenuItem(IOperationMediator operationMediator)
        {
            var menuItem = new MenuItem
            {
                Header = operationMediator.Name, 
                ToolTip = operationMediator.Description,
                IsEnabled = operationMediator.Enabled
            };
            operationMediator.EnabledChanged += enabled => menuItem.IsEnabled = enabled;
            menuItem.Click += (sender, args) => operationMediator.PerformOperation();
            return menuItem;
        }

        public ObservableCollection<MenuItem> MainMenuSource { get; set; }
        public ObservableCollection<OperationViewModel> ToolbarSource { get; set; }
        public ObservableCollection<StatusBarItem> StatusBarSource { get; set; }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }
    }
}