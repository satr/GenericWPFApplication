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
using System.Text;
using Backend.BusinessLayer;

namespace Frontend.WPF.ViewModels
{
    public class OperationScopeStatusViewModel: NotifyingEntityBase
    {
        private string _severity;
        private string _message;
        private string _description;

        public string Severity
        {
            get { return _severity; }
            set { SetProperty(ref _severity, value); }
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public IActionResult ActionResult
        {
            set
            {
                var actionResult = value;
                if (actionResult.Errors.Count > 0)
                    Severity = "Error(s):";
                else if (actionResult.Errors.Count > 0)
                    Severity = "Warning(s):";
                else if (actionResult.Errors.Count > 0)
                    Severity = string.Empty;
                ApplyActionResult(actionResult);
            }
        }

            private void ApplyActionResult(IActionResult actionResult)
            {
                var message = new StringBuilder();
                var description = new StringBuilder();
                AppendLines(message, description, "Errors:", actionResult.Errors);
                AppendLines(message, description, "Warnings:", actionResult.Warnings);
                AppendLines(message, description, null, actionResult.Infos);
                Message = message.ToString();
                Description = description.ToString();
            }

            private static void AppendLines(StringBuilder message, StringBuilder description, string title, ICollection<string> items)
            {
                if (items.Count == 0)
                    return;
                if (!string.IsNullOrWhiteSpace(title))
                {
                    description.AppendLine(title);
                }
                foreach (var item in items)
                {
                    message.AppendFormat(" {0}", item.Replace('\r', ' ').Replace('\n', ' '));
                    description.AppendLine(item);
                }
                description.AppendLine();
            }

    }
}