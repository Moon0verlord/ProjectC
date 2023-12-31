using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaveroClubhuis.Pages
{
    
    [Authorize]
    public class TeamModel : PageModel
    {
        
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get;  set; }
        public bool IsUserCheckedIn { get; private set; }
        
        public List<CaveroUser> InOfficeMembers { get; private set; }

        public List<CaveroUser> OtherInOfficeMembers { get; private set; }

        public Teams TeamChoice {  get; private set; }

        public List<Teams> AllTeams { get; private set; }

        public List<int> SearchTeam {  get; private set; }

        public Teams SearchChoice { get; private set; }
        
        public TeamModel(CaveroClubhuisContext context,UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            
        }
        
        public void OnGet()
        {
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
            AllTeams = FetchTeams();
            TeamChoice = FetchTeamChoice();
            InOfficeMembers = FetchInOfficeTeamMembers(TeamChoice);
           

        }

        public IActionResult OnPostSelect()
        {
            
          
            TempData["EnteredEventID"] = SearchTeam[0];

            // weer id enzo neerzetten want hij gaat nog niet langs onget
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
            // return Page ipv Redirectpage zodat niet alles refreshed en start van het begin
            return Page(); // Redirect naar page weer

        }

        public IActionResult OnPostShow()
        {

            // de id van de tempdata in een variabele zetten voor opzoeken juiste event
            int id = (int)TempData["EnteredEventID"];


            var team = _context.Teams.First(x => x.Id == id);
            if (!ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(User);
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }


            OtherInOfficeMembers = FetchInOfficeTeamMembers(team);
            ModelState.Clear();
           
            return RedirectToPage("./Team"); // Redirect naar page weer
        }



        public List<Teams> FetchTeams()
        {
            var teams = _context.Teams.Select(x => x).ToList();
            return teams;
        }

        public Teams FetchTeamChoice()
        {
            var userId = _userManager.GetUserId(User);
            var team = _context.Users.Where(x => x.Id == userId).Select(x => x.Team).FirstOrDefault();
            var teamInfo = _context.Teams.Where(x => x.Title == team).Select(x => x).FirstOrDefault();
            
            return teamInfo;
        }

        public List<CaveroUser> FetchInOfficeTeamMembers(Teams team)
        {

            var now = DateTimeOffset.UtcNow;
            var InOffice = _context.InOffice
        .Join(
            _context.Users,
            i => i.UserId,
            c => c.Id,
            (i, c) => new { InOffice = i, CaveroUser = c }
        )
        .AsEnumerable()
        .Where(ti => !ti.InOffice.CheckOutDate.HasValue || now <= TimeZoneInfo.ConvertTimeToUtc(ti.InOffice.CheckOutDate.Value, TimeZoneInfo.Utc))
        .Select(x => x.CaveroUser)
        .ToList();
            if (team != null)
            {
                //var members = _context.Users.Where(x => x.Team == TeamChoice.Title).Select(x => x).ToList();
                var members = (from u in _context.Users join p in _context.InOffice on u.Id equals p.UserId
                              where u.Team == team.Title select u).ToList();
                var membersInOffice = InOffice.Where(x => members.Contains(x)).ToList();

                return membersInOffice;
            }
            return null;
        }
        
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }




    }
}
