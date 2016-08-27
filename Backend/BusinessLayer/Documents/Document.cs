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
using System.IO;
using System.Text.RegularExpressions;

namespace Backend.BusinessLayer.Documents
{
    public class Document: EntityBase, IFileContent
    {
        private static readonly string InvalidFileSystemChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        private static readonly Regex InvalidFileSystemCharsRegex = new Regex(string.Format("[{0}]", Regex.Escape(InvalidFileSystemChars)));
        private static readonly Regex DoubleUnderscoreRegex = new Regex(@"_{2,}");
        const string UnderscoreChar = @"_";
        private string _name;
        private DateTimeOffset _createdOn;
        private byte[] _documentData;
        private decimal _size;

        public DateTimeOffset CreatedOn
        {
            get { return _createdOn; }
            set { SetProperty(ref _createdOn, value); }
        }

        public byte[] DocumentData
        {
            get { return _documentData; }
            set
            {
                _documentData = value;
                Size = DocumentData != null ? DocumentData.Length / 1024.0m / 1024.0m : 0;
            }
        }

        public decimal Size
        {
            get { return _size; }
            set { SetProperty(ref _size, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Extension { get; set; }

        public FileInfo SaveToFile(string folderPath)
        {
            if (DocumentData == null || DocumentData.Length <= 0) 
                return null;
            var fileName = GetValidFileNameBy(Name);
            return SaveDocumentToFile(folderPath, fileName);
        }

        private static string GetValidFileNameBy(string documentName)
        {
            var fileName = InvalidFileSystemCharsRegex.Replace(documentName, UnderscoreChar);
            return DoubleUnderscoreRegex.Replace(fileName, UnderscoreChar);
        }

        private FileInfo SaveDocumentToFile(string folderPath, string fileName)
        {
            var fileNameWithExt = fileName + Extension;
            var documentFileInfo = new FileInfo(Path.Combine(folderPath, fileNameWithExt));
            File.WriteAllBytes(documentFileInfo.FullName, DocumentData);
            return documentFileInfo;
        }
    }
}