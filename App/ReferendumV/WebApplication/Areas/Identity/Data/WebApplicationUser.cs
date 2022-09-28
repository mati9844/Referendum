using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApplication.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the WebApplicationUser class
    public class WebApplicationUser : IdentityUser
    {
        public string Id { get; set; }
        [PersonalData]
        [Display(Name = "Imię")]
        public string Name { get; set; }
        [PersonalData]
        [Display(Name = "Drugie imię")]
        public string SecondName { get; set; }
        [PersonalData]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }
        [PersonalData]
        [Display(Name = "Data urodzenia")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [PersonalData]
        [Display(Name = "Numer dowodu osobistego")]
        public string IDNumber { get; set; }
        [PersonalData]
        [Display(Name = "Numer PESEL")]
        [ScaffoldColumn(true)]
        [StringLength(11, ErrorMessage = "Długość numeru PESEL nie może przekraczać {1} znaków. ")]
        public string PIN { get; set; }

    }
}
