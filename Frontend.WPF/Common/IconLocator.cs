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
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Frontend.WPF.Common
{
    public class IconLocator
    {
        private static readonly string[] EntryAssemblyResourceNames;
        private static readonly string[] FrameworkAssemblyResourceNames;
        private static readonly string EntryAssemblyName;
        private static readonly string ExecutingAssemblyName;
        private static readonly BitmapImage NotFoundIcon;
        private static readonly BitmapImage UndefinedIcon;
        const string FrameworkImagesFolder = "Images";

        static IconLocator()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            EntryAssemblyName = entryAssembly != null? entryAssembly.GetName().Name: string.Empty;
            EntryAssemblyResourceNames = GetResourceNames(entryAssembly);

            var executingAssembly = Assembly.GetExecutingAssembly();
            ExecutingAssemblyName = executingAssembly.GetName().Name;
            FrameworkAssemblyResourceNames = GetResourceNames(executingAssembly);
            NotFoundIcon = new BitmapImage();
            NotFoundIcon = GetIcon(GetFrameworkImagePath(@"IconNotFound.png"));
            UndefinedIcon = GetIcon(GetFrameworkImagePath(@"UndefinedOperation.png"));
        }

        private static string GetFrameworkImagePath(string fileName)
        {
            return string.Format("{0}/{1}", FrameworkImagesFolder, fileName);
        }

        public static BitmapImage GetIcon(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return UndefinedIcon;
            
            var fileName = Path.GetFileName(imagePath);
            if (EntryAssemblyResourceNames.Contains(imagePath.ToLower()))
                return GetImageBy(imagePath, EntryAssemblyName);

            imagePath = GetFrameworkImagePath(fileName);
            return FrameworkAssemblyResourceNames.Contains(imagePath.ToLower()) 
                ? GetImageBy(imagePath, ExecutingAssemblyName) 
                : NotFoundIcon;
        }

        private static BitmapImage GetImageBy(string imagePath, string assemblyName)
        {
            return new BitmapImage(new Uri(string.Format(@"pack://application:,,,/{0};component/{1}", assemblyName, imagePath)));
        }

        private static string[] GetResourceNames(Assembly assembly)
        {
            const string gResources = ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + gResources))
            {
                if (stream == null)
                    return new string[0];

                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    return reader.OfType<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
                }
            }
        }
    }
}
