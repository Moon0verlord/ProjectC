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
    public class AdminModel : PageModel
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

        public string makeDisplay { get; private set; }

        public string errorMessage { get; private set; } = "";

        public AdminModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;

        }
        public IActionResult? OnGet()
        {
            //check if user is admin if not return to home page
            if (!checkAdmin()) return RedirectToPage("/Index");

            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId!);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);


            makeDisplay = "none";
            return null!;
        }

        public bool checkAdmin()
        {
            //Return true if the user is an admin else return false
            var userId = _userManager.GetUserId(User);
            string role = (from r in _context.Roles
                           where r.Id == (_context.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).FirstOrDefault())
                           select r.Name).FirstOrDefault()!;
            if (role == "Admin") return true;
            else return false;
               
        }

        public IActionResult? OnPostMakeEvent() {
            if (!ModelState.IsValid) {
                makeDisplay = "flex";

                var userId = _userManager.GetUserId(User);
                (FirstName, LastName) = _layoutTools.LoadName(userId!);
                IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId!);
                return Page();
            }
            var newEvent = new Events { 
                Title = title,
                Description = description,
                Date = date.ToUniversalTime(),
                StartTime = startTime,
                EndTime = endTime,
                UserId = _userManager.GetUserId(User)!,
                Location = location,
                Approval = true
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            ModelState.Clear();
            return RedirectToPage("/Admin");
        }

        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }
    }
}
