using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Result
    {
        [Display(Name = "Numer głosu")]
        public string VerificationKey { get; set; }

        [Display(Name = "Wiadomość")]
        public string Message { get; set; }

    }
}
