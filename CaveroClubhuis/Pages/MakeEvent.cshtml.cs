using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace CaveroClubhuis.Pages
{
    public class MakeEventModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ProfileImage { get; set; }
        public bool IsUserCheckedIn { get; private set; }

        //All the inputs needed for a new event
        [BindProperty]
        [Required(ErrorMessage = "Titel moet ingevuld worden")]
        public string title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Locatie moet ingevuld worden")]
        public string location { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Omschrijving moet ingevuld worden")]
        public string description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Start tijd moet ingevuld worden")]
        public TimeSpan startTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Eind tijd moet ingevuld worden")]
        public TimeSpan endTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Datum moet ingevuld worden")]
        public DateTime? date { get; set; }

        public MakeEventModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;

        }

        public IActionResult? OnGet()
        {
            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);

            //check if user is admin if not return to home page
            if (!_layoutTools.checkAdmin(userId)) return RedirectToPage("/Index");

            return null!;
        }

        public IActionResult? OnPostMakeEvent()
        {
            if (startTime == default(TimeSpan)) { ModelState.AddModelError("startTime", "Start tijd moet ingevuld worden"); }

            if (endTime == default(TimeSpan)) { ModelState.AddModelError("endTime", "Eind tijd moet ingevuld worden"); }
            if (!date.HasValue){ ModelState.AddModelError("date", "Datum moet ingevuld worden"); }

            if (!ModelState.IsValid )
            {
                //If inputs are missing return to the Page()
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }
            
            var newEvent = new Events
            {
                Title = title,
                Description = description,
                Date = DateTime.SpecifyKind(new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0), DateTimeKind.Utc),
                StartTime = startTime,
                EndTime = endTime,
                UserId = _userManager.GetUserId(User)!,
                Location = location,
                Approval = true
            };

            //Add the event to the database and save the database
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            //return to the admin page
            ModelState.Clear();
            return RedirectToPage("/Admin");
        }


        //Post function for check-in or out
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

        //Get available time on specific date
        //If time is given get only the hours after that time and before the next event
        public List<TimeSpan> GetHours(DateTime date, TimeSpan? time)
        {
            date = date.ToUniversalTime();
            //Get all the hours at which a event is
            var times = (from e in _context.Events
                         where e.Date.Year == date.Year
                               && e.Date.Month == date.Month
                               && e.Date.Day == date.Day
                         select new
                         {
                             start = e.StartTime,
                             end = e.EndTime
                         }).ToList();

            //if a time is given only the events that are after returned
            if (time != null)
            {
                times = (from e in _context.Events
                         where e.Date.Year == date.Year
                               && e.Date.Month == date.Month
                               && e.Date.Day == date.Day
                               && time < e.StartTime

                         select new
                         {
                             start = e.StartTime,
                             end = e.EndTime
                         }).ToList();
            }
            //List of hours that are available
            List<TimeSpan> hoursList = new List<TimeSpan>();

            //if a time is given change the current time to that plus 30 minutes
            TimeSpan current = TimeSpan.Zero;
            if (time != null)
            {
                current = time.Value + TimeSpan.FromHours(0.5);
            }
            //limit to how many hours are available
            TimeSpan end = TimeSpan.FromHours(23.5);

            //Add hours to a list
            while (current <= end)
            {
                if (time == null)
                {
                    //add hour to the list of available hours if it isnt between an event.
                    if (!times.Any(t => current >= t.start && current < t.end))
                    {
                        hoursList.Add(current);
                    }
                }
                else
                {
                    //if time is not null only add the hours that are below the start time of the next event
                    if (!times.Any(t => current > t.start))
                    {

                        // Add current to hoursList
                        hoursList.Add(current);
                    }
                }
                current = current.Add(TimeSpan.FromMinutes(30));
            }

            return hoursList;
        }

        public IActionResult OnGetProcessInput(string date, TimeSpan? time)
        {
            // Convert the date from a string to dateTime
            DateTime result = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            //call on the gethours method
            return new JsonResult(GetHours(result, time));
        }

    }
}
