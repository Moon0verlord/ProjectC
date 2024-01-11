using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CaveroClubhuis.Areas.Identity.Pages.Account.Manage
{
    public class TeamModel : PageModel
    {
        private readonly SignInManager<CaveroUser> _signInManager;
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public bool IsUserCheckedIn { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get; set; }
        public List<Teams> AllTeams { get; private set; }

        public string Team {  get; private set; }

        [BindProperty]
        [Display(Name = "Nieuw Team")]
        public List<int> SelectedTeams { get; private set; }
        public TeamModel(
            UserManager<CaveroUser> userManager,
            SignInManager<CaveroUser> signInManager, CaveroClubhuisContext context, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            _signInManager = signInManager;
            SelectedTeams = new List<int>();
        }

                [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

      
        [TempData]
        public string StatusMessage { get; set; }

        

     

        public async Task<IActionResult> OnGetAsync()
        {
            // get name of user



            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
            AllTeams = FetchTeams();
            Team = FetchTeamChoice();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeTeam()
        {
            var user = await _userManager.GetUserAsync(User);
            if(SelectedTeams == null)
            {
                await Console.Out.WriteLineAsync("AAAAAAAAA");
            }
           var team = _context.Teams.Where(e => SelectedTeams.Contains(e.Id))
                .FirstOrDefault();
           
           // Console.WriteLine(SelectedTeams.Count());
            //await Console.Out.WriteLineAsync("AAA");
           //y if (team == null) { await Console.Out.WriteLineAsync("jjj"); }
            //await Console.Out.WriteLineAsync(team.Title);
           
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
           if(team == null) { return NotFound($"Unable to load team with ID ."); }
            var userId = _userManager.GetUserId(User);
           user.Team = team.Title; 

            
            await _context.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
             
                
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);

                return Page();
            }

            

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Je Team is gewijzigd";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }
        public List<Teams> FetchTeams()
        {
            var teams = _context.Teams.Select(x => x).ToList();
            return teams;
        }

        public string FetchTeamChoice()
        {
            var userId = _userManager.GetUserId(User);
            var team = _context.Users.Where(x => x.Id == userId).Select(x => x.Team).FirstOrDefault();
            var teamInfo = _context.Teams.Where(x => x.Title == team).Select(x => x.Title).FirstOrDefault();

            return teamInfo;
        }
    }
}
