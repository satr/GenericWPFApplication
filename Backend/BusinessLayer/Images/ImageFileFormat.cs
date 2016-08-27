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
using System.Drawing.Imaging;
using System.Linq;

namespace Backend.BusinessLayer.Images
{
    public class ImageFileFormat
    {
        private readonly ICollection<string> _extensions = new List<string>();
        private static IList<ImageFileFormat> _formats;
        public string Name { get; private set; }
        public ImageFormat ImageFormat { get; private set; }

        public ICollection<string> Extensions
        {
            get { return _extensions; }
        }

        public string FirstExtension {
            get { return _extensions.FirstOrDefault()?? string.Empty; }
        }

        public ImageFileFormat(string name, ImageFormat imageFormat, string extension, params string[] additionExtensions)
        {
            ImageFormat = imageFormat;
            Name = name;
            _extensions.Add(extension);
            foreach (var additionExtension in additionExtensions)
            {
                _extensions.Add(additionExtension);
            }
        }

        public static ImageFileFormat GetBy(string extension)
        {
            return Formats.FirstOrDefault(format => format.Extensions.Contains(extension));
        }

        public static ImageFileFormat GetBy(ImageFormat imageFormat)
        {
            return Formats.FirstOrDefault(format => Equals(format.ImageFormat, imageFormat));
        }

        public static IList<ImageFileFormat> Formats
        {
            get { return _formats?? (_formats = CreateFormatsList()); }
        }

        private static IList<ImageFileFormat> CreateFormatsList()
        {
            return new List<ImageFileFormat>
            {
                new ImageFileFormat("Bitmap",  ImageFormat.Bmp, ".bmp"),
                new ImageFileFormat("Jpeg", ImageFormat.Jpeg, ".jpeg", ".jpg"),
                new ImageFileFormat("Png", ImageFormat.Png, ".png"),
                new ImageFileFormat("Gif", ImageFormat.Gif, ".gif"),
                new ImageFileFormat("Tiff", ImageFormat.Tiff, ".tiff", ".tif"),
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}