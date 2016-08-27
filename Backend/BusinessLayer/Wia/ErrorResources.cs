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
using System.ComponentModel;

namespace Backend.BusinessLayer.Wia
{
    public class ErrorResources
    {
        public IList<Error> Items { get; private set; }


        public ErrorResources()
        {
            Items = new List<Error>
            {
                new Error("WIA_ERROR_BUSY",
                    "The device is busy. Close any apps that are using this device or wait for it to finish and then try again.",
                    "0x80210006"),
                new Error("WIA_ERROR_COVER_OPEN", "One or more of the device’s cover is open.", "0x80210016"),
                new Error("WIA_ERROR_DEVICE_COMMUNICATION",
                    "Communication with the WIA device failed. Make sure that the device is powered on and connected to the PC. If the problem persists, disconnect and reconnect the device.",
                    "0x8021000A"),
                new Error("WIA_ERROR_DEVICE_LOCKED",
                    "The device is locked. Close any apps that are using this device or wait for it to finish and then try again.",
                    "0x8021000D"),
                new Error("WIA_ERROR_EXCEPTION_IN_DRIVER", "The device driver threw an exception.", "0x8021000E"),
                new Error("WIA_ERROR_GENERAL_ERROR", "An unknown error has occurred with the WIA device.", "0x80210001"),
                new Error("WIA_ERROR_INCORRECT_HARDWARE_SETTING", "There is an incorrect setting on the WIA device.",
                    "0x8021000C"),
                new Error("WIA_ERROR_INVALID_COMMAND", "The device doesn't support this command.", "0x8021000B"),
                new Error("WIA_ERROR_INVALID_DRIVER_RESPONSE", "The response from the driver is invalid.", "0x8021000F"),
                new Error("WIA_ERROR_ITEM_DELETED", "The WIA device was deleted. It's no longer available.", "0x80210009"),
                new Error("WIA_ERROR_LAMP_OFF", "The scanner's lamp is off.", "0x80210017"),
                new Error("WIA_ERROR_MAXIMUM_PRINTER_ENDORSER_COUNTER",
                    "A scan job was interrupted because an Imprinter/Endorser item reached the maximum valid value for WIA_IPS_PRINTER_ENDORSER_COUNTER, and was reset to 0. This feature is available with Windows 8 and later versions of Windows.",
                    "0x80210021"),
                new Error("WIA_ERROR_MULTI_FEED",
                    "A scan error occurred because of a multiple page feed condition. This feature is available with Windows 8 and later versions of Windows.",
                    "0x80210020"),
                new Error("WIA_ERROR_OFFLINE",
                    "The device is offline. Make sure the device is powered on and connected to the PC.", "0x80210005"),
                new Error("WIA_ERROR_PAPER_EMPTY", "There are no documents in the document feeder.", "0x80210003"),
                new Error("WIA_ERROR_PAPER_JAM", "Paper is jammed in the scanner's document feeder.", "0x80210002"),
                new Error("WIA_ERROR_PAPER_PROBLEM",
                    "An unspecified problem occurred with the scanner's document feeder.", "0x80210004"),
                new Error("WIA_ERROR_WARMING_UP", "The device is warming up.", "0x80210007"),
                new Error("WIA_ERROR_USER_INTERVENTION",
                    "There is a problem with the WIA device. Make sure that the device is turned on, online, and any cables are properly connected.",
                    "0x80210008"),
                new Error("WIA_S_NO_DEVICE_AVAILABLE",
                    "No scanner device was found. Make sure the device is online, connected to the PC, and has the correct driver installed on the PC.",
                    "0x80210015")
            };
        }

        public class Error
        {
            public string Name { get; private set; }
            public string Message { get; private set; }
            public int ErrorCode { get; private set; }
            public string HResult { get; private set; }

            public Error(string name, string message, string hResult)
            {
                Name = name;
                Message = message;
                HResult = hResult;
                ErrorCode = (int) (new Int32Converter().ConvertFromString(hResult) ?? -1);
            }
        }
    }
}
