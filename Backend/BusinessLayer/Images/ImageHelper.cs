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
using System.IO;
using System.Linq;

namespace Backend.BusinessLayer.Images
{
    public static class ImageHelper
    {
        public static byte[] ConvertImageToByteArray(Image image, ImageFileFormat imageFileFormat, long imageQuality)
        {
            if (image == null)
                return new byte[0];

            using (var stream = new MemoryStream())
            {
                var encoder = GetEncoder(imageFileFormat.ImageFormat);
                if (encoder == null)
                    throw new InvalidOperationException(string.Format("Encoder not found for {0}", imageFileFormat.ImageFormat));

                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, imageQuality);
                image.Save(stream, encoder, encoderParams);
                return stream.GetBuffer();
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public static Bitmap ConvertToGrayScale(Bitmap image)
        {
            IterateTroughImagePixels(image,
                                     (color, x, y) =>
                                     {
                                         var grayScale = GetGrayScale(color);
                                         image.SetPixel(x, y, Color.FromArgb(color.A, grayScale, grayScale, grayScale));
                                     });
            return image;
        }

        public static Dictionary<int, int> ComputeGrayScaleLevels(Bitmap image)
        {
            var grayScalesLevels = new Dictionary<int, int>();
            IterateTroughImagePixels(image, (color, x, y) =>
            {
                var grayScale = color.B;
                if (!grayScalesLevels.ContainsKey(grayScale))
                {
                    grayScalesLevels.Add(grayScale, 0);
                }
                grayScalesLevels[grayScale]++;
            });
            return grayScalesLevels;
        }

        private static void IterateTroughImagePixels(Bitmap image, Action<Color, int, int> action)
        {
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    action(image.GetPixel(x, y), x, y);
                }
            }
        }

        private static int GetGrayScale(Color color)
        {
            const double grayScaleLevelFactorByRed = 0.3;
            const double grayScaleLevelFactorByGreen = 0.59;
            const double grayScaleLevelFactorByBlue = 0.11;
            return (int)((color.R * grayScaleLevelFactorByRed)
                          + (color.G * grayScaleLevelFactorByGreen)
                          + (color.B * grayScaleLevelFactorByBlue));
        }

    }
}
