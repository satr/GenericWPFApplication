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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using WIA;

namespace Backend.BusinessLayer.Wia
{
    public class WiaManager : IWiaManager
    {
        private readonly ErrorResources _errorResources;
        private static DeviceManager _deviceManager;
        private static CommonDialog _commonDialog;

        public WiaManager()
        {
            _errorResources = new ErrorResources();
        }

        private string GetErrorMessageBy(Exception exception)
        {
            var errorItem = _errorResources.Items.FirstOrDefault(item => item.ErrorCode == exception.HResult);
            return errorItem != null? errorItem.Message: exception.Message;
        }

        public int GetDeviceCount()
        {
            var count = 0;
            WrapCOMError(() => count = DeviceManager.DeviceInfos.Count);
            return count;
        }

        private void WrapCOMError(Action action)
        {
            try
            {
                action();
            }
            catch (COMException e)
            {
                throw new InvalidOperationException(GetErrorMessageBy(e));
            }
        }

        private DeviceManager DeviceManager
        {
            get
            {
                if (_deviceManager != null) 
                    return _deviceManager;
                WrapCOMError(() => _deviceManager = new DeviceManager());
                return _deviceManager;
            }
        }

        public Scanner GetScanner(bool showDevicesDialog)
        {
            Scanner scanner = null;
            WrapCOMError(() =>
            {
                // ReSharper disable once RedundantArgumentDefaultValue
                var scannerDevice = CommonDialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, showDevicesDialog, false);
                scanner = scannerDevice == null || scannerDevice.Items.Count < 1
                            ? null
                            : new Scanner(scannerDevice.Items[1]);
/*
                Debug.WriteLine("-------Output all properties----------------");
                if(scanner != null)
                {
                  for (int i = 1; i < scannerDevice.Items[1].Properties.Count; i++)
                  {
                        var property = scannerDevice.Items[1].Properties[i];
                        Debug.WriteLine("{0}:{1}", property.Name,property.get_Value());
                  }
                }
*/
            });
            return scanner;
        }

        private CommonDialog CommonDialog
        {
            get
            {
                if (_commonDialog != null) 
                    return _commonDialog;
                WrapCOMError(() => _commonDialog = new CommonDialog());
                return _commonDialog;
            }
        }

        public Bitmap ScanDocument(bool showDevicesDialog, Scanner scanner, IActionResult actionResult)
        {
            Bitmap bitmap = null;
            WrapCOMError(() =>
            {
                ImageFile scannedImageFile;
                try
                {
                    const string bmpFormatID = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
                    if (scanner != null && scanner.Device != null)
                    {
                        scannedImageFile = CommonDialog.ShowTransfer(scanner.Device, bmpFormatID, true) as ImageFile;
                    }
                    else
                    {
                        scannedImageFile = CommonDialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, WiaImageIntent.TextIntent,
                                                WiaImageBias.MaximizeQuality, bmpFormatID, showDevicesDialog, false, true) as ImageFile;
                    }
                    if (scannedImageFile == null)
                        actionResult.AddError("Scanning failed.");
                }
                catch (COMException e)
                {
                    const int operationCancelledErrorResult = -2145320860;
                    if (e.ErrorCode != operationCancelledErrorResult)
                        throw;
                    actionResult.AddInfo("Scanning canceled.");
                    return;
                }
                bitmap = SaveFile(scannedImageFile);
            });
            return bitmap;
        }

        private static Bitmap SaveFile(ImageFile imageFile)
        {
            var process = new ImageProcess();
            object filterProperty = "Convert";
            // ReSharper disable once UseIndexedProperty
            var convertFilterID = process.FilterInfos.get_Item(ref filterProperty).FilterID;
            // ReSharper disable once RedundantArgumentDefaultValue
            process.Filters.Add(convertFilterID, 0);
            object formatPropValue = FormatID.wiaFormatTIFF;
            object formatIdProperty = "FormatID";
            object qualityPropValue = 100L;
            object qualityIdProperty = "Quality";
            var properties = process.Filters[process.Filters.Count].Properties;
            // ReSharper disable once UseIndexedProperty
            var property = properties.get_Item(ref formatIdProperty);
            // ReSharper disable once UseIndexedProperty
            property.set_Value(ref formatPropValue);
            // ReSharper disable once UseIndexedProperty
            var qualityProperty = properties.get_Item(ref qualityIdProperty);
            // ReSharper disable once UseIndexedProperty
            qualityProperty.set_Value(ref qualityPropValue);
            imageFile = process.Apply(imageFile);
            Bitmap bitmap;
            // ReSharper disable once UseIndexedProperty
            using(var stream = new MemoryStream((byte[])imageFile.FileData.get_BinaryData()))
            {
                bitmap = new Bitmap(Image.FromStream(stream));
            }
            return bitmap;
        }
    }
}
