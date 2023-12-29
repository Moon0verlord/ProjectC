using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CaveroClubhuis.Pages
{
    public class AdminDeleteModel : PageModel
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
        public List<int> SelectedEvents { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public List<int> SelectedEvent { get; set; }

        [BindProperty]
        public string Title { get; set; }

        public List<Events> EventsNames { get; set; }
        public AdminDeleteModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            SelectedEvents = new List<int>();
        }

        public IActionResult? OnGet()
        {
            var userId = _userManager.GetUserId(User);
            if (!_layoutTools.checkAdmin(userId)) return RedirectToPage("/Index");

            EventsNames = FetchEventsName();
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            return null!;
        }


        public IActionResult OnPostDelete()
        {

            // verwijder event uit database
            var fullEvents = _context.Events
                   .Where(e => SelectedEvents.Contains(e.Id))
                   .ToList();



            _context.RemoveRange(fullEvents);
            _context.SaveChanges();
            // notificatie opslaan
            TempData["DeleteSuccess"] = "Evenement is succesvol verwijderd";

            return RedirectToPage("./Index"); // Redirect naar page weer
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
