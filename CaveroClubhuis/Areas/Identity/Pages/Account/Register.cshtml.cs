// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
//using MyApp.Communication.SMTP;
using System.Configuration;
using CaveroClubhuis.Data;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CaveroUser> _signInManager;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly IUserStore<CaveroUser> _userStore;
        private readonly IUserEmailStore<CaveroUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CaveroClubhuisContext _context;

        public List<Teams> AllTeams { get; private set; }

        public RegisterModel(
            CaveroClubhuisContext context,
            UserManager<CaveroUser> userManager,
            IUserStore<CaveroUser> userStore,
            SignInManager<CaveroUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)

        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;

            AllTeams = FetchTeams();
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            // input first name
            
            [Required(ErrorMessage = "Voornaam is vereist.")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            
            // inpuit last name
            [Required(ErrorMessage = "Achternaam is vereist.")]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            
            // input email
            [Required(ErrorMessage = "Het veld E-mail is vereist.")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // input wachtwoord
            [Required(ErrorMessage = "Het veld Wachtwoord is vereist.")]
            [StringLength(100, ErrorMessage = "De {0} moet minimaal {2} en maximaal {1} tekens lang zijn.", MinimumLength = 6)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", ErrorMessage = "Het wachtwoord moet minstens 8 tekens lang zijn en minstens één kleine letter, één hoofdletter, één cijfer en één speciaal teken bevatten.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            // verificatie wachtwoord
            [Required(ErrorMessage = "Bevestig wachtwoord veld is vereist" )]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Het wachtwoord en het bevestigingswachtwoord komen niet overeen.")]
            public string ConfirmPassword { get; set; }

            // input team
            [Required(ErrorMessage = "Het veld Team is vereist.")]
            [BindProperty]
            [Display(Name = "Selecteer Team")]
            public int SelectedTeams { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/");
    // if uncommented users can only sign up using an email with a @cavero domain
    // if (!Input.Email.Contains("@cavero"))
    // {
    //     ModelState.AddModelError(string.Empty, "U kunt zich alleen registreren met een @cavero e-mailadres.");
    //     return Page();
    // }
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    if (ModelState.IsValid)
    {
        // Check if a user with the same email already exists
        var existingUser = await _userManager.FindByEmailAsync(Input.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "Er bestaat al een account met dit e-mailadres.");
            return Page();
        }

        // If the user does not exist, continue with the registration process
        var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                Console.WriteLine("Test");
                Console.WriteLine(Input.SelectedTeams);
                user.Team = _context.Teams.Where(_ => _.Id == Input.SelectedTeams).FirstOrDefault().Title;

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);

            // gemaakte sender aanroepen
            await SendEmailAsync(Input.Email, "Verifieer uw Clubhuis account",
                $"{BodyVerificationEmail(callbackUrl)}"
            );

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


    
        // nieuwe method voor email verzenden met bool kijken of succesvol
        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink) 
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress("noreplycavero@gmail.com");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = confirmLink;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("noreplycavero@gmail.com", "tbtmeubeppicuaoo");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception) { return false; }



        }

        private CaveroUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<CaveroUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(CaveroUser)}'. " +
                    $"Ensure that '{nameof(CaveroUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<CaveroUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<CaveroUser>)_userStore;
        }

        private string BodyVerificationEmail(string urlLink)
        {
            string root = "wwwroot";
            string file = "emailVerification.html";
            string FullPath = Path.Combine(root, file);
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(FullPath))
                {
                    body = reader.ReadToEnd();
                }
               body = body.Replace("URL", urlLink);
            return body;
        }

        public List<Teams> FetchTeams()
        {
            var teams = _context.Teams.ToList();
            return teams;
        }
    }
}
