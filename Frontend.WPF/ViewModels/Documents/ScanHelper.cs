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
using System.Windows.Controls;
using System.Windows.Media;

namespace Frontend.WPF.ViewModels.Documents
{
    internal static class ScanHelper
    {
        public static IList<KeyValuePair<string, int>> CreateImageColorModeSource()
        {
            return new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Color", 1),
                new KeyValuePair<string, int>("Grayscale", 2),
                new KeyValuePair<string, int>("B/W", 4)
            };
        }

        public static IList<ImageDisplayMode> CreateImageDisplayModeSource()
        {
            return new List<ImageDisplayMode>
            {
                new ImageDisplayMode("Uniform to fill horizontal", Stretch.UniformToFill, ScrollBarVisibility.Auto,
                    ScrollBarVisibility.Disabled, true),
                new ImageDisplayMode("Uniform to fill all", Stretch.Uniform),
                new ImageDisplayMode("Stretch to fill", Stretch.Fill),
                new ImageDisplayMode("Scale")
            };
        }

        public static IList<ImageQuality> CreateImageQualitySource()
        {
            return new List<ImageQuality>
            {
                new ImageQuality("Low", ImageQuality.ImageQualityValue.Low),
                new ImageQuality("Normal", ImageQuality.ImageQualityValue.Normal, true),
                new ImageQuality("High", ImageQuality.ImageQualityValue.High),
                new ImageQuality("Max", ImageQuality.ImageQualityValue.Max),
            };
        }

        public static IList<KeyValuePair<string, bool>> CreateSelectDeviceSource()
        {
            return new List<KeyValuePair<string, bool>>
            {
                new KeyValuePair<string, bool>("First", false),
                new KeyValuePair<string, bool>("Always Select", true),
            };
        }

        public static IList<KeyValuePair<string, int>> CreateImageDPISource()
        {
            return new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("Draft  (100 dpi)", 100),
                new KeyValuePair<string, int>("Normal (300 dpi)", 300),
                new KeyValuePair<string, int>("Fine   (600 dpi)", 600),
                new KeyValuePair<string, int>("Super  (1200 dpi)", 1200),
            };
        }

        public static IList<KeyValuePair<string, bool>> CreateShowScanDialogSource()
        {
            return new List<KeyValuePair<string, bool>>
            {
                new KeyValuePair<string, bool>("None", false),
                new KeyValuePair<string, bool>("Show", true),
            };
        }

    }
}
