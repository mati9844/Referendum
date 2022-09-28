using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;
using System.Security.Cryptography;
using System.Text;
using WebApplication.Areas.Identity.Data;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Security.Claims;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using SendAndReceiveSms.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    public class VotingController : Controller
    {
        private const int KeyIdStartIndex = 4;
        private const string message = "Dziękujemy za udział w głosowaniu. Zachowaj kod, aby móc skontrolować swój udział w referendum po ogłoszeniu wyników.";
        private readonly WebApplicationContext _context;
        private readonly MyKeysContext _contextKeys;

        public VotingController(WebApplicationContext context, MyKeysContext contextKeys)
        {
            _context = context;
            _contextKeys = contextKeys;
        }


        static String PythonScript(string message, string key)
        {
            string python = @"C:\Users\Mateusz\AppData\Local\Programs\Python\Python38-32\python.exe";

            string app = "D:\\informatyka\\api\\first.py";

            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;

            myProcessStartInfo.Arguments = app + " " + message + " " + key;

            Process myProcess = new Process();
            myProcess.StartInfo = myProcessStartInfo;

            Console.WriteLine("Uruchomienie skryptu szyfrowania z parametrami {0} oraz {1}", message, key);
            myProcess.Start();

            StreamReader myStreamReader = myProcess.StandardOutput;
            string encrypted = myStreamReader.ReadLine();

            myProcess.WaitForExit();
            myProcess.Close();

            Console.WriteLine("Wartosc zwrocona: " + encrypted);
            return encrypted;
        }

        private Envelope CreateEnvelope()
        {
            Envelope envelope = new Envelope();
            envelope.Id = Guid.NewGuid().ToString();
            envelope.Status = Status.F;
            return envelope;
        }


        public async Task<DataProtectionKey> SearchFreeKey()
        {
            foreach(DataProtectionKey row in _contextKeys.DataProtectionKeys)
            {
                if(GetXMLValue(row.Xml, "key/activationDate") == "NULL")
                {
                    return row;
                }
            }
            return null;
        }
        public async Task<string> GetFreeKey()
        {
            string key = null;

            var row = await SearchFreeKey();
            var dt = DateTime.UtcNow;
            row.Xml = SetXMLValue(row.Xml, "key/activationDate", dt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"));
            dt.AddDays(30);
            row.Xml = SetXMLValue(row.Xml, "key/expirationDate", dt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"));
            _contextKeys.DataProtectionKeys.Update(row);
            await _contextKeys.SaveChangesAsync();
            key = GetXMLValue(row.Xml, "key/encryptedKey");
            
            return key;

        }


        public static string SetXMLValue(string XML, string searchTerm, string setValue)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);
            XmlNodeList nodes = doc.SelectNodes(searchTerm);
            foreach (XmlNode node in nodes)
            {
                node.InnerText = setValue;
            }
            return doc.OuterXml;
        }

        public static string GetXMLValue(string XML, string searchTerm)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);
            XmlNodeList nodes = doc.SelectNodes(searchTerm);
            foreach (XmlNode node in nodes)
            {
                if (node.InnerText.Length == 0)
                    return "NULL";
                else
                    return node.InnerText;
            }
            return "NULL";
        }

        public async Task<List<Question>> GetQuestionsAsync()
        {

            return await _context.Questions.ToListAsync();
        }
        public async Task<IActionResult> Index()
        {
            Geolocation geolocation = new IPController().GetGeolocation();
            Console.WriteLine(geolocation.ID);
            Console.WriteLine(geolocation.type);
            Console.WriteLine(geolocation.longitude);
            Console.WriteLine(geolocation.latitude);
            Console.WriteLine(geolocation.geoname_id);


            return View();
        }

        public bool IsReferendum()
        {
            var currentDateTime = DateTime.UtcNow;
            var referendum = _context.Referendums.FirstOrDefaultAsync(d => d.EndDate < currentDateTime);
            if(referendum != null)
            {
                return true;
            }
            return false;
        }
        public string getUserPhoneNumber()
        {
            return "+48537902523";
        }

        [Authorize]
        public async Task<IActionResult> Select()
        {

            ViewData["Questions"] = await GetQuestionsAsync();

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select([Bind("Id,Question,Answer")] Vote vote)
        {

            Envelope envelope = CreateEnvelope();
            Vote obj = new Vote();
            string key = null;

            var row = await SearchFreeKey();
            var dt = DateTime.UtcNow;
            row.Xml = SetXMLValue(row.Xml, "key/activationDate", dt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"));
            dt.AddDays(30);
            row.Xml = SetXMLValue(row.Xml, "key/expirationDate", dt.ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"));
            _contextKeys.DataProtectionKeys.Update(row);
            await _contextKeys.SaveChangesAsync();

            key = GetXMLValue(row.Xml, "key/encryptedKey");

            obj.Question = vote.Question;
            obj.Answer = PythonScript(vote.Answer, key);
            obj.Id = row.FriendlyName.Substring(KeyIdStartIndex);
            _context.Add(obj);

            envelope.VoteId = obj.Id;
            envelope.Timestamp = DateTime.Now;

            VerifyPhoneNumber verifyPhoneNumber = new VerifyPhoneNumber();
            verifyPhoneNumber.PhoneNumber = getUserPhoneNumber();
            verifyPhoneNumber.EnvelopeID = envelope.Id.ToString();
            verifyPhoneNumber.Code = new Random().Next(1000, 100000).ToString();
            verifyPhoneNumber.DateTimeSent = DateTime.UtcNow.ToString();

            _contextKeys.Add(verifyPhoneNumber);
            await _contextKeys.SaveChangesAsync();

            new SmsController().SendSms(verifyPhoneNumber.PhoneNumber, verifyPhoneNumber.Code);
            _context.Add(envelope);
            await _context.SaveChangesAsync();

            verifyPhoneNumber = new VerifyPhoneNumber();
            verifyPhoneNumber.PhoneNumber = getUserPhoneNumber();

            return View("Verify", verifyPhoneNumber);                 
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify([Bind("Code,PhoneNumber")] VerifyPhoneNumber verifyPhoneNumber)
        {
            var envelope = new Envelope();
            //Console.WriteLine("Phone:" + verifyPhoneNumber.PhoneNumber);

            VerifyPhoneNumber verifyPhoneNumberDB = new VerifyPhoneNumber();
            verifyPhoneNumberDB = await _contextKeys.verifyPhoneNumbers.FirstOrDefaultAsync(n => n.PhoneNumber == verifyPhoneNumber.PhoneNumber);
            if (verifyPhoneNumberDB != null)
            {
                var Id = verifyPhoneNumberDB.EnvelopeID;
                envelope = await _context.Envelopes.FindAsync(Id);

                if (envelope != null)
                {
                    var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    /* if (envelope.WebApplicationUserId != currentUserId)
                     {
                         envelope.Status = Status.C;
                         _context.Update(envelope);
                         await _context.SaveChangesAsync();
                         return View("Fail");

                     }*/
                    if (verifyPhoneNumber.Code != verifyPhoneNumberDB.Code)
                    {
                        envelope.Status = Status.B;
                        _context.Update(envelope);
                        await _context.SaveChangesAsync();
                        return View("Fail");
                    }

                    envelope.Status = Status.A;
                    Result result = new Result();
                    result.VerificationKey = envelope.VoteId;
                    result.Message = message;
                    _context.Update(envelope);
                    await _context.SaveChangesAsync();
                    return View("Success", result);

                }
                else
                {
                    return View("Fail");
                }

            }
            return View("Fail");

        }
    }
}
