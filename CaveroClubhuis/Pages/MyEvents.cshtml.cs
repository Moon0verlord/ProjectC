using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages
{
    [Authorize]
    public class MyEventsModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        
        
        public MyEventsModel(CaveroClubhuisContext context,UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;

        }
        public void OnGet()
        {
            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
        }
        

    }
}
