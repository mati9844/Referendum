using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Session
    {
        public string Id { get; set; }
        public string GeolocationID;
        public virtual Geolocation Geolocation { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Timestamp]
        public DateTime Timestamp = DateTime.Now;

    }
}
