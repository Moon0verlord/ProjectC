using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaveroClubhuis.Pages
{
    public class AdminRemoveTeamModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public bool IsUserCheckedIn { get; private set; }

        [BindProperty]
        public int? chosenTeamID { get; set; } = null;


        public List<Teams> teams { get; set; }

        public AdminRemoveTeamModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            teams = FetchTeams();

        }

        public IActionResult? OnGet()
        {
            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId!);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
            teams = FetchTeams();

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

        public List<Teams> FetchTeams()
        {

            return _context.Teams.ToList();
        }

        public IActionResult OnPostRemoveTeam()
        {
            if (chosenTeamID == null) { ModelState.AddModelError("chosenTeamID", "Team moet geselecteerd worden"); }
            if (!ModelState.IsValid)
            {
                
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName) = _layoutTools.LoadName(userId!);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                teams = FetchTeams();
                return Page();
            }

            Teams chosenTeam = _context.Teams.Where(_ => _.Id == chosenTeamID).FirstOrDefault()!;
            _context.Teams.Remove(chosenTeam);
            _context.SaveChanges();
            return RedirectToPage("/Admin");
        }


        }
}
