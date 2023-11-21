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
        private readonly LayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public bool IsUserCheckedIn { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Veld moet ingevuld worden")]
        public string title { get; set; }

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
        public List<int> SelectedEvents { get; set; }

        public Events EventChoice { get; set; }

        [BindProperty]
        public string Title { get; set; }

        public List<Events> EventsNames { get; set; }
        public AdminChangeModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, LayoutTools layoutTools)
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


        public IActionResult OnPostChangeEvent()
        {

            Console.WriteLine("test");


            return RedirectToPage("./AdminDelete"); // Redirect naar page weer
        }
        public IActionResult OnPostAskInput()
        {

            Console.WriteLine("test");
            EventChoice = _context.Events
                 .Where(e => SelectedEvents.Contains(e.Id))
                 .FirstOrDefault();

            title = EventChoice.Title;
            description=EventChoice.Description;
            date= EventChoice.Date;
            startTime = EventChoice.StartTime;
            endTime = EventChoice.EndTime;
            location= EventChoice.Location;


            Console.WriteLine(EventChoice.Title);
        
            return Page(); // Redirect naar page weer
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

