using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CaveroClubhuis.Pages
{
    public class AdminFeedbackModel : PageModel
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
        public List<EventReviews> Reviews { get; set; }
        public List<string> feedbackText { get; set; }

        public AdminFeedbackModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            SelectedEvents = new List<int>();
            Events = FetchEvents();
        }
        public IActionResult OnGet()
        {
            var userId = _userManager.GetUserId(User);
            if (!_layoutTools.checkAdmin(userId)) return RedirectToPage("/Index");

            Events = FetchEvents();
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            return null!;
        }


        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

        public List<Events> FetchEvents()
        {
            DateTime currentDate = DateTime.UtcNow;
            return _context.Events.Where(x => x.Date < currentDate).ToList();
        }

        public IActionResult OnPostseeFeedback()
        {

            // de id van de tempdata in een variabele zetten voor opzoeken juiste event
            int id = (int)TempData["EnteredEventID"];
            ModelState.Clear();
            return RedirectToPage("./Index"); // Redirect naar page weer
        }

        public IActionResult OnPostAskInput()
        {
            EventChoice = _context.Events
                 .Where(e => SelectedEvents.Contains(e.Id))
                 .FirstOrDefault();

            Reviews = _context.EventReviews
                .Where(e => e.EventId == EventChoice.Id)
                .ToList();

            // Check if there are any reviews for the selected event
            if (!Reviews.Any())
            {
                feedbackText = null;
            }
            else
            {
                feedbackText = Reviews.Select(x => x.FeedbackText).ToList();
            }

            title = EventChoice.Title;
            description = EventChoice.Description;
            date = EventChoice.Date;
            startTime = EventChoice.StartTime;
            endTime = EventChoice.EndTime;
            location = EventChoice.Location;
            EventID = EventChoice.Id;
            TempData["EnteredEventID"] = EventChoice.Id;

            // weer id enzo neerzetten want hij gaat nog niet langs onget
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
            // return Page ipv Redirectpage zodat niet alles refreshed en start van het begin
            return Page(); // Redirect naar page weer
        }
    }
}
