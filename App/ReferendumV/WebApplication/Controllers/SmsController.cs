using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Configuration;
using Twilio;
using Twilio.AspNet.Core;
using Twilio.AspNet.Common;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace SendAndReceiveSms.Controllers
{
    public class SmsController : TwilioController
    {
        public string SendSms(string phoneNumber, string code)
        {
            var accountSid = "XXXXX";
            var authToken = "XXXXX";

            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber(phoneNumber);
            var from = new PhoneNumber("+12XXXXX");

            var message = MessageResource.Create(
                to: to,
                from: from,
                body: "Twój kod weryfikacyjny to: " + code);

            return message.Sid;
        }
    }
}
