// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaveroClubhuis.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<CaveroUser> _userManager;

        public ResetPasswordModel(UserManager<CaveroUser> userManager)
        {
            _userManager = userManager;
        }

      
        [BindProperty]
        public InputModel Input { get; set; }

       
        public class InputModel
        {
           // input email
            [Required(ErrorMessage = "Email is vereist")]
            [EmailAddress]
            public string Email { get; set; }

            // input wachtwoord
            [Required(ErrorMessage = "Wachtwoord is vereist")]
            [Display(Name = "Nieuw wachtwoord")]
            [StringLength(100, ErrorMessage = "Het {0} moet tussen de {2} en {1} karakters zijn.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
               
            // input verificatie wachtwoord 
            [DataType(DataType.Password)]
            [Display(Name = "Bevestigings wachtwoord")]
            [Compare("Password", ErrorMessage = "Het nieuwe wachtwoord en bevestigingswachtwoord komen niet overeen.")]
            public string ConfirmPassword { get; set; }

            // authenticatie code
            [Required]
            public string Code { get; set; }

        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                string error = "email is onjuist";
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
