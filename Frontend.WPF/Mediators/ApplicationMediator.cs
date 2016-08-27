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
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Backend.DataLayer;
using Frontend.WPF.Common;
using Frontend.WPF.Mediators.Entities;
using Frontend.WPF.Mediators.Operations;
using Frontend.WPF.Mediators.Operations.Settings;
using Frontend.WPF.ViewModels;
using Frontend.WPF.Views.Controls;
using Backend.BusinessLayer;
using Backend.Common;
using Frontend.WPF.Views.Windows;

namespace Frontend.WPF.Mediators
{
    public abstract class ApplicationMediator<T> : MediatorBase, IApplicationMediator, IOperationScopeActionResultSubscriber
        where T : DatabaseContextBase, new()
    {
        private IEnumerable<IEntitiesMediator> _mediators;
        private string _name;
        private UserControl _statusView;
        private OperationScopeStatusViewModel _operationScopeStatusViewModel;

        protected ApplicationMediator(Application app)
        {
            ServiceLocator.Get<OperationScopeContext>().SubscribeOnActionResult(this);
            InitNameAndVersion();
            InitDatabase();
            InitApplication(app);
        }

        private void InitApplication(Application app)
        {
            app.DispatcherUnhandledException += OnDispatcherUnhandledException;
            app.MainWindow = new MainView(this);
            app.MainWindow.Closing += (sender, args) => this.Dispose();
            app.MainWindow.Show();
        }

        private static void InitDatabase()
        {
            var databaseContext = new T();
            ServiceLocator.Get<OperationScopeContext>().SetActionResultAsync(databaseContext.ValidationResult);
            ServiceLocator.Set<DatabaseContextBase>(databaseContext);
            ServiceLocator.Set(databaseContext);
        }

        private void InitNameAndVersion()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                _name = string.Empty;
                Version = new Version();
                return;
            }
            var assemblyName = entryAssembly.GetName();
            var productAttribute = entryAssembly.GetCustomAttribute<AssemblyProductAttribute>();
            _name = productAttribute.Product;
            Version = assemblyName.Version;
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            MessageHelper.ShowError(dispatcherUnhandledExceptionEventArgs.Exception.Message);
        }

        public Version Version { get; private set; }

        public UserControl ApplicationStatusView
        {
            get { return _statusView ?? (_statusView = CreateApplicationStatusView()); }
        }

        private UserControl CreateApplicationStatusView()
        {
            return new ApplicationStatusView(OperationScopeStatusViewModel);
        }

        private OperationScopeStatusViewModel OperationScopeStatusViewModel
        {
            get
            {
                return _operationScopeStatusViewModel??(_operationScopeStatusViewModel = new OperationScopeStatusViewModel());
            }
        }

        public IEnumerable<IEntitiesMediator> EntitiesMediators {
            get { return _mediators ?? (_mediators = CreateModuleMediator().ToList()); }
        }

        protected virtual IEnumerable<IEntitiesMediator> CreateModuleMediator()
        {
            yield break;
        }

        public override string Name
        {
            get { return _name; }
        }

        protected override IEnumerable<IOperationMediator> CreateOperationMediators()
        {
            yield return new SettingsOperationMediator();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_mediators != null) 
            {
                foreach (var mediator in _mediators)
                    mediator.Dispose();
            }
            foreach (var operationMediator in OperationMediators)
                operationMediator.Dispose();
        }

        public void NotifyWithActionResult(IActionResult actionResult)
        {
            OperationScopeStatusViewModel.ActionResult = actionResult;
        }
    }
}
