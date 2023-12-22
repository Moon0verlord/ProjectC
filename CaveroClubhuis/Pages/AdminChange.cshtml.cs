using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CaveroClubhuis.Pages
{
    public class AdminChangeModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public bool IsUserCheckedIn { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public string? title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public string location { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public string description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public TimeSpan startTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public TimeSpan endTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public DateTime date { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public List<int> SelectedEvents { get; set; }

        public Events EventChoice { get; set; }

        public int EventID { get; set; }

        [BindProperty]
        public string Title { get; set; }

        public List<Events> Events { get; set; }
        public AdminChangeModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
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

            Events = FetchEvents();
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            return null!;

        }


        public IActionResult OnPostChangeEvent()
        {

           // de id van de tempdata in een variabele zetten voor opzoeken juiste event
            int id = (int)TempData["EnteredEventID"];
         

            var eventToUpdate = _context.Events.First(x => x.Id == id);
            if (!ModelState.IsValid)
            {

                var userId = _userManager.GetUserId(User);
                (FirstName, LastName) = _layoutTools.LoadName(userId!);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }

            eventToUpdate.Title=title; 
            eventToUpdate.Description=description;
            eventToUpdate.Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            eventToUpdate.StartTime = startTime;
            eventToUpdate.EndTime = endTime;
            eventToUpdate.Location = location;
            _context.SaveChanges();
            ModelState.Clear();
            TempData["ChangeSuccess"] = "Evenement is succesvol gewijzigd";
            return RedirectToPage("./Index"); // Redirect naar page weer
        }
        public IActionResult OnPostAskInput()
        {

          
            EventChoice = _context.Events
                 .Where(e => SelectedEvents.Contains(e.Id))
                 .FirstOrDefault();

            title = EventChoice.Title;
            description=EventChoice.Description;
            date= EventChoice.Date;
            startTime = EventChoice.StartTime;
            endTime = EventChoice.EndTime;
            location= EventChoice.Location;
            EventID = EventChoice.Id;
             // omdat properties gereset werden tempdata om de id op te slaan
            TempData["EnteredEventID"] = EventChoice.Id;

            // weer id enzo neerzetten want hij gaat nog niet langs onget
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
            // return Page ipv Redirectpage zodat niet alles refreshed en start van het begin
            return Page(); // Redirect naar page weer
        }


        public List<Events> FetchEvents()
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

