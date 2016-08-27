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
using System.IO;
using System.Net;
using System.Net.Mail;
using Backend.BusinessLayer.Contacts;
using Backend.BusinessLayer.Settings;

namespace Backend.BusinessLayer.Email
{
    public class EmailController
    {
        private readonly EmailSettingsRepository _emailSettingsRepository;

        public EmailController()
        {
            _emailSettingsRepository = new EmailSettingsRepository();
        }

        public void SendEmail(EmailMessage emailMessage)
        {
            var emailSettings = _emailSettingsRepository.Get();
            using (var smtpClient = CreateSmtpClient(emailSettings))
            {
                var mailMessage = CreateMailMessage(emailMessage, emailSettings);
                smtpClient.Send(mailMessage);
            }
        }

        private static MailMessage CreateMailMessage(EmailMessage emailMessage, IEmailSettings emailSettings)
        {
            var mailMessage = new MailMessage
            {
                Subject = emailMessage.Subject,
                Body = emailMessage.Message,
                ReplyToList = {emailSettings.FromEmailAddress},
                From = new MailAddress(emailSettings.FromEmailAddress)
            };
            SetAddresses(emailMessage.ToAddresses, mailMessage.To);
            SetAddresses(emailMessage.CcAddresses, mailMessage.CC);
            SetAddresses(emailMessage.BccAddresses, mailMessage.Bcc);
            var tempPath = Path.GetTempPath();
            foreach (var attachedItem in emailMessage.AttachedItems)
            {
                var fileContent = attachedItem.Content as IFileContent;
                if(fileContent == null)
                    continue;//TODO - add error
                var attachedFileInfo = fileContent.SaveToFile(tempPath);
                if(attachedFileInfo != null)
                    mailMessage.Attachments.Add(new Attachment(attachedFileInfo.FullName));
            }
            return mailMessage;
        }

        private static void SetAddresses(IEnumerable<Contact> addresses, ICollection<MailAddress> mailAddressCollection)
        {
            foreach (var contact in addresses)
            {
                mailAddressCollection.Add(new MailAddress(contact.Email, contact.Name ?? contact.Email));
            }
        }

        private static SmtpClient CreateSmtpClient(EmailSettings emailSettings)
        {
            return new SmtpClient(emailSettings.ServerName, emailSettings.Port)
            {
                Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password),
                EnableSsl = emailSettings.UseSSL,
            };
        }
    }
}
