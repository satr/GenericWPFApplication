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
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Documents;
using Backend.BusinessLayer.Wia;
using Backend.Common;
using Frontend.WPF.Common;
using Frontend.WPF.ViewModels.Documents;

namespace Frontend.WPF.Commands.Documents
{
    public class ScanDocumentCommand<TDocument> : CommandBase
        where TDocument : Document, new()
    {
        private readonly IWiaManager _wiaManager;
        private readonly ScanDocumentViewModel<TDocument> _viewModel;
        private IActionResult _actionResult = new ActionResult();

        public ScanDocumentCommand(ScanDocumentViewModel<TDocument> viewModel)
        {
            _wiaManager = ServiceLocator.Get<WiaManager>();
            _viewModel = viewModel;
        }

        protected override bool CommandAction(object parameter)
        {
            _actionResult = new ActionResult();
            ScanInProgress(true);
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Bitmap image = null;
            Task.Factory.StartNew(() => image = ScanDocument())
                .ContinueWith(t => ScanInProgress(false), CancellationToken.None, TaskContinuationOptions.None, scheduler)
                .ContinueWith(t => SetDocumentToViewModel(image), CancellationToken.None, TaskContinuationOptions.NotOnFaulted, scheduler);
            return true;
        }

        private void ScanInProgress(bool inProgress)
        {
            _viewModel.ScanEnabled = !inProgress;
            SetCanExecute(!inProgress);
        }

        private void SetDocumentToViewModel(Bitmap image)
        {
            if (_actionResult.Success)
            {
                _viewModel.ScannedImage = image;
                _actionResult.AddInfo("Document scanned.");
            }
            else
            {
                MessageHelper.Show(_actionResult);
            }
            ServiceLocator.Get<OperationScopeContext>().SetActionResultAsync(_actionResult);
        }

        private Bitmap ScanDocument()
        {
            var deviceCount = _wiaManager.GetDeviceCount();
            if (deviceCount == 0)
            {
                _actionResult.AddError("No devices found.");
                return null;
            }

            Scanner scanner = null;
            if (!_viewModel.ScanSettings.ShowScanDialog)
            {
                scanner = _wiaManager.GetScanner(_viewModel.ScanSettings.SelectDevice);
                if (scanner == null)
                {
                    _actionResult.AddError("Scanner not found");
                    return null;
                }
                scanner.ImageColorMode = _viewModel.ScanSettings.ImageColorMode;
                scanner.ImageResolution = _viewModel.ScanSettings.ImageResolution;
            }

            var image = _wiaManager.ScanDocument(_viewModel.ScanSettings.SelectDevice, scanner, _actionResult);
            if (image != null)
                _actionResult.AddInfo("Image scanned.");
            return image;
        }
    }
}