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
using System.Linq;
using WIA;

namespace Backend.BusinessLayer.Wia
{
    public class Scanner
    {
        private readonly List<DeviceProperty> _deviceProperties;

        public Scanner(Item device)
        {
            _deviceProperties = PopulateProperties();
            Device = device;
            FillInProperties(Device, _deviceProperties);
        }

        public Item Device { get; private set; }

        public int ImageColorMode
        {
            get { return GetPropertyValue<int>(PropetyName.CurrentIntent); }
            set { SetPropertyValue(PropetyName.CurrentIntent, value); }
        }

        public int ImageResolution
        {
            get { return GetPropertyValue<int>(PropetyName.VerticalResolution); }
            set
            {
                SetPropertyValue(PropetyName.VerticalResolution, value);
                SetPropertyValue(PropetyName.HorizontalResolution, value);
            }
        }

        private T GetPropertyValue<T>(string propertyName)
        {
            var property = _deviceProperties.FirstOrDefault(p => Equals(p.Name, propertyName));
            if (property == null)
                return default(T);
            return (T) property.Value;
        }

        private void SetPropertyValue<T>(string propertyName, T value)
        {
            var property = _deviceProperties.FirstOrDefault(p => Equals(p.Name, propertyName));
            if (property != null) 
                property.Value = value;
        }

        class DeviceProperty
        {
            private object _value;
            public string Name { get; set; }

            public object Value
            {
                // ReSharper disable once UseIndexedProperty
                get { return Property == null? _value: Property.get_Value(); }
                set
                {
                    if (Property == null)
                        _value = value;
                    else
                    // ReSharper disable once UseIndexedProperty
                        Property.set_Value(value);
                }
            }

            public Property Property { private get; set; }

            public DeviceProperty(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        private static void FillInProperties(IItem device, List<DeviceProperty> deviceProperties)
        {
            if(device == null)
                return;
            for (var i = 1; i <= device.Properties.Count; i++)
            {
                var property = device.Properties[i];
                var deviceProperty = deviceProperties.FirstOrDefault(p => Equals(p.Name, property.Name));
                if(deviceProperty == null)
                    continue;
                deviceProperty.Property = property;
            }
        }

        private List<DeviceProperty> PopulateProperties()
        {
            return new List<DeviceProperty>()
            {
                new DeviceProperty(PropetyName.ItemFlags, 532483),
                new DeviceProperty(PropetyName.AccessRights, 1), 
                new DeviceProperty(PropetyName.Planar, 0), 
                new DeviceProperty(PropetyName.DataType, 3), 
                new DeviceProperty(PropetyName.BitsPerPixel, 24), 
                new DeviceProperty(PropetyName.ChannelsPerPixel, 3), 
                new DeviceProperty(PropetyName.BitsPerChannel, 8), 
                new DeviceProperty(PropetyName.CurrentIntent, 0), 
                new DeviceProperty(PropetyName.FilenameExtension, "BMP"), 
                new DeviceProperty(PropetyName.Compression, 0), 
                new DeviceProperty(PropetyName.ItemSize, 0), 
                new DeviceProperty(PropetyName.HorizontalResolution, 300), 
                new DeviceProperty(PropetyName.VerticalResolution, 300), 
                new DeviceProperty(PropetyName.HorizontalStartPosition, 0), 
                new DeviceProperty(PropetyName.VerticalStartPosition, 0), 
                new DeviceProperty(PropetyName.HorizontalExtent, 2550), 
                new DeviceProperty(PropetyName.VerticalExtent, 3507), 
                new DeviceProperty(PropetyName.Brightness, 0), 
                new DeviceProperty(PropetyName.Contrast, 0), 
                new DeviceProperty(PropetyName.Rotation, 0), 
                new DeviceProperty(PropetyName.Threshold, 128), 
                new DeviceProperty(PropetyName.PhotometricInterpretation, 0), 
                new DeviceProperty(PropetyName.PixelsPerLine, 2550), 
                new DeviceProperty(PropetyName.NumberOfLines, 3507), 
                new DeviceProperty(PropetyName.BytesPerLine, 7650), 
                new DeviceProperty(PropetyName.BufferSize, 65536), 
                new DeviceProperty(PropetyName.MediaType, 2),
                new DeviceProperty(PropetyName.PreferredFormat, "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}"),
            };
        }

        static class PropetyName
        {
            public const string ItemFlags = "Item Flags";
            public const string AccessRights = "Access Rights";
            public const string Planar = "Planar";
            public const string DataType = "Data Type";
            public const string BitsPerPixel = "Bits Per Pixel";
            public const string ChannelsPerPixel = "Channels Per Pixel";
            public const string BitsPerChannel = "Bits Per Channel";
            public const string CurrentIntent = "Current Intent";
            public const string FilenameExtension = "Filename extension";
            public const string Compression = "Compression";
            public const string ItemSize = "Item Size";
            public const string HorizontalResolution = "Horizontal Resolution";
            public const string VerticalResolution = "Vertical Resolution";
            public const string HorizontalStartPosition = "Horizontal Start Position";
            public const string VerticalStartPosition = "Vertical Start Position";
            public const string HorizontalExtent = "Horizontal Extent";
            public const string VerticalExtent = "Vertical Extent";
            public const string Brightness = "Brightness";
            public const string Contrast = "Contrast";
            public const string Rotation = "Rotation";
            public const string Threshold = "Threshold";
            public const string PhotometricInterpretation = "Photometric Interpretation";
            public const string PixelsPerLine = "Pixels Per Line";
            public const string NumberOfLines = "Number of Lines";
            public const string BytesPerLine = "Bytes Per Line";
            public const string BufferSize = "Buffer Size";
            public const string MediaType = "Media Type";
            public const string PreferredFormat = "Preferred Format";
        }
    }
}