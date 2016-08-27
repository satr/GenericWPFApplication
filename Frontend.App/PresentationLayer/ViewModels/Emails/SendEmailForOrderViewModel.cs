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
using System.Text;
using Backend.BusinessLayer;
using Backend.Common;
using Frontend.App.BusinessLayer.Documents;
using Frontend.App.BusinessLayer.Orders;
using Frontend.WPF.ViewModels.Emails;

namespace Frontend.App.PresentationLayer.ViewModels.Emails
{
    public class SendEmailForOrderViewModel : EmailViewModel<OrderDocument>
    {
        private Order _order;
        
        public override bool ValidateAndInitialize()
        {
            var order = ServiceLocator.Get<OperationScopeContext>().GetLastSelectedItems<Order>().FirstOrDefault();
            if (order == null)
                return false;
            Order = order;
            return base.ValidateAndInitialize();
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                if(Order == null)
                    return;
                OrderTitle = string.Format("Order Number:{0}, Invoice Number{1}", Order.OrderNumber, Order.InvoiceNumber);
            }
        }

        private string OrderTitle { get; set; }

        protected override IEnumerable<OrderDocument> GetDocuments()
        {
            return Order.Documents;
        }

        protected override string CreateMessage()
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Hi,");
            messageBuilder.AppendFormat("Regarding {0}", OrderTitle);
            messageBuilder.AppendLine();
            messageBuilder.AppendFormat("Best regards");
            messageBuilder.AppendLine();
            var message = messageBuilder.ToString();
            return message;
        }
    }
}