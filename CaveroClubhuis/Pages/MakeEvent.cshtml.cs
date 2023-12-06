using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CaveroClubhuis.Pages
{
    public class MakeEventModel: PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public bool IsUserCheckedIn { get; private set; }

        //All the inputs needed for a new event
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

        public MakeEventModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;

        }

        public IActionResult? OnGet()
        {
            var userId = _userManager.GetUserId(User);
            //check if user is admin if not return to home page
            if (!_layoutTools.checkAdmin(userId)) return RedirectToPage("/Index");

            // get name of user
            (FirstName, LastName) = _layoutTools.LoadName(userId!);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);

            return null!;
        }

        public IActionResult? OnPostMakeEvent()
        {
            if (!ModelState.IsValid)
            {
                //If inputs are missing return to the Page()
                var userId = _userManager.GetUserId(User);
                (FirstName, LastName) = _layoutTools.LoadName(userId!);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }
            //Create a new event with the inputs
            var newEvent = new Events
            {
                Title = title,
                Description = description,
                Date = date.ToUniversalTime(),
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
    }
}
