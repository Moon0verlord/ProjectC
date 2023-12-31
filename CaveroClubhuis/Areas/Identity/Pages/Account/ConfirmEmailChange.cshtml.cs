// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;

namespace CaveroClubhuis.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly SignInManager<CaveroUser> _signInManager;
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public bool IsUserCheckedIn { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get;  set; }
        public ConfirmEmailChangeModel(
            UserManager<CaveroUser> userManager,
            SignInManager<CaveroUser> signInManager, CaveroClubhuisContext context, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = "Error bij veranderen van de email neem contact op met een admin";
                return Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "Error changing user name.";
                return Page();
            }
            var userId2 = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId2!);


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Bedankt voor het verifieren, je email is nu gewijzigd!";
            return Page();
        }
    }
}
