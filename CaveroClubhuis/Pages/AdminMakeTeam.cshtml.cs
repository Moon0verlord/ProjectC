using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Syncfusion.EJ2.Linq;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaveroClubhuis.Pages
{
    public class AdminMakeTeamModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get; set; }
        public bool IsUserCheckedIn { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public string? title { get; set; }

        public AdminMakeTeamModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;

        }

        public IActionResult? OnGet()
        {
            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);

            //check if user is admin if not return to home page
            if (!_layoutTools.checkAdmin(userId)) return RedirectToPage("/Index");

            return null!;
        }

        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

        public IActionResult OnPostMakeTeam()
        {
            if (!ModelState.IsValid) { 
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }

            Teams newTeam = new Teams { Title = title };

            _context.Teams.ForEach(i => Console.WriteLine(i));
            _context.Teams.Add(newTeam);
            _context.SaveChanges();
            ModelState.Clear();
            return RedirectToPage("./Index"); // Redirect naar page weer
        }
    }
}
