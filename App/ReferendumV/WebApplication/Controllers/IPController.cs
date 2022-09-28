using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace WebApplication.Controllers
{
    public class IPController : Controller
    {
        public string GetCurrentIP()
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
	    return ip.ToString();
            //return "78.156.178.151".ToString();
        }
        public Geolocation GetGeolocation()
        {
            string IP = GetCurrentIP();
            if (IP != null)
            {
                Geolocation geolocation = new Geolocation();

                string url = "http://api.ipstack.com/" + IP + "?access_key=XXXXXXXXX";
                var request = System.Net.WebRequest.Create(url);

                using (WebResponse wrs = request.GetResponse())
                using (Stream stream = wrs.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);
                    geolocation.ID = (string)obj["ip"];
                    geolocation.type = (string)obj["type"];
                    geolocation.continent_code = (string)obj["continent_code"];
                    geolocation.continent_name = (string)obj["continent_name"];
                    geolocation.country_code = (string)obj["country_code"];
                    geolocation.country_name = (string)obj["country_name"];
                    geolocation.region_code = (string)obj["region_code"];
                    geolocation.city = (string)obj["city"];
                    geolocation.latitude = (double)obj["latitude"];
                    geolocation.longitude = (double)obj["longitude"];
                    geolocation.geoname_id = (string)obj["location"]["geoname_id"];
                 
                    return geolocation;
                }
            }
            return null;
        }
    }
}
