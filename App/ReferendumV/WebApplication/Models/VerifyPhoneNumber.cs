using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class VerifyPhoneNumber
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kod")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }
        public string EnvelopeID { get; set; }
        public string DateTimeSent { get; set; }
        public string DateTimeConfirmation { get; set; }


    }
}
