using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages
{
    public class TeamSearchModel : PageModel
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

        public Teams TeamChoice { get; private set; }

        public List<Teams> AllTeams { get; private set; }

        [BindProperty]
        public List<int> SearchTeam { get; private set; }

        public Teams SearchChoice { get; private set; }

        public TeamSearchModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            SearchTeam = new List<int>();

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

        public IActionResult OnPostAskInput()
        {
            Console.WriteLine("tetetet");
            foreach (var item in SearchTeam)
            {
                Console.WriteLine("test");
                Console.WriteLine(item);
            }
            // zoek team op
            var team = _context.Teams.Where(e => SearchTeam.Contains(e.Id))
                 .FirstOrDefault(); 

            // als team niet bestaat returnen
            if (team == null)
            {
                // Handle the case where the team is not found
                Console.WriteLine("nullll");
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
                return RedirectToPage("./Team");
            }

            
            // Update de dialog met de members
            OtherInOfficeMembers = FetchInOfficeTeamMembers(team);
             SearchChoice=team;
            AllTeams = FetchTeams();
            TeamChoice = FetchTeamChoice();
            InOfficeMembers = FetchInOfficeTeamMembers(TeamChoice);

            // viewdata aanpassen
            ViewData["ShowDialog2"] = true;
            var userId2 = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId2);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId2);
            return Page();
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
                var members = (from u in _context.Users
                               join p in _context.InOffice on u.Id equals p.UserId
                               where u.Team == team.Title
                               select u).ToList();
                var membersInOffice = InOffice.Where(x => members.Contains(x)).ToList();

                return membersInOffice;
            }
            return null;
        }




    }
}
