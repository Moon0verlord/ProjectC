using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Pages
{
    public class AdminDeleteModel : PageModel
    { 
    private readonly CaveroClubhuisContext _context;
    private readonly UserManager<CaveroUser> _userManager;
    private readonly LayoutTools _layoutTools;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public bool IsUserCheckedIn { get; private set; }
        [BindProperty]
        public List<int> SelectedEvents { get; set; } 

        public List<Events> EventsNames { get; set; }
        public AdminDeleteModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            SelectedEvents = new List<int>();
        }

        public void OnGet()
        {
            EventsNames = FetchEventsName();
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);


        }

      
        public IActionResult OnPostDelete()
        {
                
                // verwijder event uit database
            var fullEvents = _context.Events
                   .Where(e => SelectedEvents.Contains(e.Id))
                   .ToList();
            Console.WriteLine("test");
            foreach (var item in SelectedEvents)
            {
                Console.WriteLine(item);
            }
            _context.RemoveRange(fullEvents);
            _context.SaveChanges();
        
           
            return RedirectToPage("./AdminDelete"); // Redirect naar page weer
        }


        public List<Events> FetchEventsName()
        {
            return _context.Events.Select(x => x).ToList();
        }


        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

    }
}
public class ButtonModel
{
    public string content { get; set; }
    public bool isPrimary { get; set; }
    public string cssClass { get; set; }
}
