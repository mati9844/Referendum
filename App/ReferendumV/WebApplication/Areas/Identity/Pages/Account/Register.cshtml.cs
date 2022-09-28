using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WebApplication.Areas.Identity.Data;

namespace WebApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<WebApplicationUser> _signInManager;
        private readonly UserManager<WebApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<WebApplicationUser> userManager,
            SignInManager<WebApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Imię jest wymagane")]
            [DataType(DataType.Text)]
            [Display(Name = "Imię")]
            public string Name { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Drugie imię")]
            public string SecondName { get; set; }
            [Required(ErrorMessage = "Nazwisko jest wymagane")]
            [DataType(DataType.Text)]
            [Display(Name = "Nazwisko")]
            public string Surname { get; set; }
            [Required(ErrorMessage = "Data urodzenia jest wymagana")]
            [DataType(DataType.Date)]
            [Display(Name = "Data urodzenia")]
            public DateTime BirthDate { get; set; }
            [Required(ErrorMessage = "Numer dowodu osobistego jest wymagany")]
            [DataType(DataType.Text)]
            [Display(Name = "Numer dowodu osobistego")]
            public string IDNumber { get; set; }
            [Required(ErrorMessage = "Numer PESEL jest wymagany")]
            [DataType(DataType.Text)]
            [Display(Name = "Numer PESEL")]
            [ScaffoldColumn(true)]
            [StringLength(11, ErrorMessage = "Długość numeru PESEL nie może przekraczać {1} znaków. ")]
            public string PIN { get; set; }
            [Required(ErrorMessage = "Adres email jest wymagany")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Hasło jest wymagane")]
            [StringLength(100, ErrorMessage = "Hasło {0} musi mieć od {2} do {1} znaków.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Powtórz hasło")]
            [Compare("Password", ErrorMessage = "Hasła muszą być identyczne")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new WebApplicationUser {
                    Name = Input.Name,
                    SecondName = Input.SecondName,
                    Surname = Input.Surname,
                    BirthDate = Input.BirthDate,
                    IDNumber = Input.IDNumber,
                    PIN = Input.PIN,
                    UserName = Input.PIN, 
                    Email = Input.Email 
                };
                user.Id = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Użytkownik został utworzony");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Potwierdzenie",
                        $"Potwierdź rejestrację przez email <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>kliknij tutaj</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
