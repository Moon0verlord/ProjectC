using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CaveroClubhuis.Pages
{
    [Authorize]
    public class EventsModel : PageModel
    {
        
        public IList<Events> EventsList { get; set; }
        
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        
        public bool IsUserCheckedIn { get; private set; }

        
        
        public EventsModel(CaveroClubhuisContext context,UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            

        }
        
        public void OnGet()
        {
            EventsList = FetchEvents();
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            
        }
        
        
        public IList<Events> FetchEvents()
        {
            return _context.Events.ToList();
        }
        
        
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }
    }
}
