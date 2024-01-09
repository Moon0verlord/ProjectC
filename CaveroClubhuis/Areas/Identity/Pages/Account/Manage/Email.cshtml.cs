// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel : PageModel
    {
        
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<CaveroUser> _signInManager;
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public bool IsUserCheckedIn { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get;  set; }

        public EmailModel(
            UserManager<CaveroUser> userManager,
            SignInManager<CaveroUser> signInManager, CaveroClubhuisContext context, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
        }

    
        public string Email { get; set; }

    
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
           // input email
            [Required(ErrorMessage = "Alle velden zijn verplicht om in te vullen")]
            [EmailAddress]
            [Display(Name = "Nieuwe email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(CaveroUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

           

            var email = await _userManager.GetEmailAsync(user);
            // custom errors toevoegen aan de Modelstate voor email checks
            if (Input.NewEmail == email)
            {
                ModelState.AddModelError(string.Empty, "De nieuwe email kan niet hetzelfde zijn als de oude");

            }
            else if (!Input.NewEmail.Contains("@") || !Input.NewEmail.Contains("."))
            {
                ModelState.AddModelError(string.Empty, "Een email moet een @ en een . bevatten");
            }
            else // als er geen errors zijn code uitvoeren
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await SendEmailAsync(
                    Input.NewEmail,
                    "Verifeer je email",
                $"{BodyVerificationEmail(callbackUrl)}");

                StatusMessage = "Een verificatie link is verzonden naar uw nieuwe email. Check alstublieft uw email.";
                return RedirectToPage();
            }

           // bij ongeldige modelstate account gegevens weer ophalen voor check-in
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await SendEmailAsync(
                email,
                "Verifeer je email",
                $"{BodyVerificationEmail(callbackUrl)}");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }

 

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
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }
    }
}
