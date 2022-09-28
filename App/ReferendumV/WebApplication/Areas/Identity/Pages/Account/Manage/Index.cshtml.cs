using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Areas.Identity.Data;

namespace WebApplication.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<WebApplicationUser> _userManager;
        private readonly SignInManager<WebApplicationUser> _signInManager;

        public IndexModel(
            UserManager<WebApplicationUser> userManager,
            SignInManager<WebApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Imię")]
            public string Name { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Drugie imię")]
            public string SecondName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Nazwisko")]
            public string Surname { get; set; }
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime BirthDate { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Numer dowodu osobistego")]
            public string IDNumber { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Numer PESEL")]
            [ScaffoldColumn(true)]
            [StringLength(11, ErrorMessage = "Długość numeru PESEL nie może przekraczać {1} znaków. ")]
            public string PIN { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(WebApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Name,
                SecondName = user.SecondName,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                IDNumber = user.IDNumber,
                PIN = user.PIN,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if (Input.SecondName != user.SecondName)
            {
                user.SecondName = Input.SecondName;
            }

            if (Input.Surname != user.Surname)
            {
                user.Surname = Input.Surname;
            }

            if (Input.BirthDate != user.BirthDate)
            {
                user.BirthDate = Input.BirthDate;
            }

            if (Input.IDNumber != user.IDNumber)
            {
                user.IDNumber = Input.IDNumber;
            }

            if (Input.PIN != user.PIN)
            {
                user.PIN = Input.PIN;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
