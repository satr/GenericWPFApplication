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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Media;
using Backend.BusinessLayer;
using Backend.BusinessLayer.Documents;
using Backend.BusinessLayer.Images;
using Backend.BusinessLayer.Settings;
using Backend.Common;
using Frontend.WPF.Commands.Documents;
using Frontend.WPF.Common;

namespace Frontend.WPF.ViewModels.Documents
{
    public class ScanDocumentViewModel<TDocument> : NotifyingEntityBase, IOperationViewModel
        where TDocument: Document, new()
    {
        readonly ImageFormat _defaultImageFormat = ImageFormat.Jpeg;
        private TDocument _document;
        private readonly ICollectionRepository<TDocument, Guid> _documentsRepository;
        private bool _scanEnabled;
        private ImageDisplayMode _imageDisplayMode;
        private IList<ImageFileFormat> _imageFormatsSource;
        private IList<ImageQuality> _imageQualitySource;
        private Bitmap _scannedImage;
        private bool _applyChangesEnabled;
        private ImageQuality _imageQuality;
        private bool _qualityEnabled;
        private bool _scaleEnabled;
        private bool _autoScale;
        private readonly ImageDisplayMode _imageStretchUniformToFill;
        private readonly ImageDisplayMode _imageStretchNoneButScale;
        private decimal _imageScaleRatio;
        private decimal _scaleRatioPercents;
        private ScanSettings _scanSettings;
        private ImageSource _imageSource;
        private ImageFileFormat _imageFormatItem;

        public ScanDocumentViewModel(ICollectionRepository<TDocument, Guid> documentsRepository, ScanSettings scanSettings)
        {
            _documentsRepository = documentsRepository;
            ScanSettings = scanSettings;
            var imageDisplayModes = ScanHelper.CreateImageDisplayModeSource();
            _imageStretchUniformToFill = imageDisplayModes.FirstOrDefault(i => i.Stretch == Stretch.UniformToFill);
            _imageStretchNoneButScale = imageDisplayModes.FirstOrDefault(i => i.Stretch == Stretch.None);
            ImageColorModeSource = ScanHelper.CreateImageColorModeSource();
            ImageQualitySource = ScanHelper.CreateImageQualitySource();
            ImageQuality = ImageQualitySource.FirstOrDefault(i => i.Default);
            ImageFormatsSource = ImageFileFormat.Formats;
            ScanEnabled = true;
            QualityEnabled = false;
            AutoScale = true;
            ImageScaleRatio = 1;
            ImageFormatItem = ImageFormatsSource.FirstOrDefault(i => Equals(i.ImageFormat, _defaultImageFormat));
        }

        public IList<KeyValuePair<string, int>> ImageColorModeSource { get; set; }

        public decimal ImageScaleRatio
        {
            get { return _imageScaleRatio; }
            set
            {
                SetProperty(ref _imageScaleRatio, value);
                ScaleRatioPercents = ImageScaleRatio*100;
            }
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        public decimal ScaleRatioPercents
        {
            get { return _scaleRatioPercents; }
            set { SetProperty(ref _scaleRatioPercents, value); }
        }

        public bool AutoScale
        {
            get { return _autoScale; }
            set
            {
                SetProperty(ref _autoScale, value);
                ScaleEnabled = !AutoScale;
                ImageDisplayMode = AutoScale ? _imageStretchUniformToFill : _imageStretchNoneButScale;
            }
        }

        public bool ScaleEnabled
        {
            get { return _scaleEnabled; }
            set { SetProperty(ref _scaleEnabled, value); }
        }

        public ApplyChangesCommand<TDocument> ApplyChangesCommand { get { return new ApplyChangesCommand<TDocument>(this); } }
        public ScanDocumentCommand<TDocument> ScanDocumentCommand { get { return new ScanDocumentCommand<TDocument>(this); } }
        public ScanSettingsCommand<TDocument> ScanSettingsCommand { get { return new ScanSettingsCommand<TDocument>(this); } }

        public Bitmap ScannedImage
        {
            get { return _scannedImage; }
            set
            {
                if(Equals(_scannedImage == value))
                    return;
                if(_scannedImage != null)
                    _scannedImage.Dispose();
                _scannedImage = value;
                ApplyChangesEnabled = _scannedImage != null;
                SetDocumentDataAndImageSource(ScannedImage);
            }
        }

        public TDocument Document
        {
            get { return _document; }
            set { SetProperty(ref _document, value); }
        }

        public ImageFileFormat ImageFormatItem
        {
            get { return _imageFormatItem; }
            set
            {
                if (_imageFormatItem == value) 
                    return;
                _imageFormatItem = value;
                SetProperty(ref _imageFormatItem, value);
                if(Document != null)
                    Document.Extension = ImageFormatItem != null ? ImageFormatItem.FirstExtension : string.Empty;
                if (ImageFormatItem != null)
                    QualityEnabled = Equals(ImageFormatItem.ImageFormat, ImageFormat.Jpeg);
            }
        }

        public void SetDocumentDataAndImageSource(Image image)
        {
                if (Document == null) 
                    return;
            Document.DocumentData = ImageHelper.ConvertImageToByteArray(image, ImageFormatItem, ImageQuality.Value);
            Document.Size = Document.DocumentData != null ? Document.DocumentData.Length / 1024.0m / 1024.0m : 0;
            ImageSource = UIImageHelper.ConvertByteArrayImage(Document.DocumentData);
        }

        public bool QualityEnabled
        {
            get { return _qualityEnabled && ScanEnabled; }
            set { SetProperty(ref _qualityEnabled, value); }
        }

        public ImageDisplayMode ImageDisplayMode
        {
            get { return _imageDisplayMode; }
            set { SetProperty(ref _imageDisplayMode, value); }
        }

        public ImageQuality ImageQuality
        {
            get { return _imageQuality; }
            set { SetProperty(ref _imageQuality, value); }
        }

        public bool ScanEnabled
        {
            get { return _scanEnabled; }
            set { SetProperty(ref _scanEnabled, value); }
        }

        public bool ApplyChangesEnabled
        {
            get { return _applyChangesEnabled && ScanEnabled; }
            set { SetProperty(ref _applyChangesEnabled, value); }
        }

        public IList<ImageFileFormat> ImageFormatsSource
        {
            get { return _imageFormatsSource; }
            set { SetProperty(ref _imageFormatsSource, value); }
        }

        public IList<ImageQuality> ImageQualitySource
        {
            get { return _imageQualitySource; }
            set { SetProperty(ref _imageQualitySource, value); }
        }

        public bool ValidateAndInitialize()
        {
            Document = new TDocument
            {
                CreatedOn = DateTimeOffset.Now,
                Name = "Doc." + DateTime.Now.ToLongTimeString().Replace(":", "-")
            };
            return true;
        }

        public void PerformOperation()
        {
            var operationScopeContext = ServiceLocator.Get<OperationScopeContext>();
            
            if (Document == null || Document.Size == 0)
            {
                operationScopeContext.SetActionResultAsync(new ActionResult().AddWarning("Document not scanned."));
                return;
            }
            
            var actionResult = _documentsRepository.Save(Document);
            operationScopeContext.SetActionResultAsync(actionResult);
        }

        public bool PerformOperationEnabled
        {
            get { return ScanEnabled && ScannedImage != null; }
        }

        public bool CancelOperationEnabled
        {
            get { return ScanEnabled; }
        }

        public ScanSettings ScanSettings
        {
            get { return _scanSettings; }
            set { SetProperty(ref _scanSettings, value); }
        }
    }
}