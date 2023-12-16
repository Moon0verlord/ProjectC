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
    public class KalenderModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool IsUserCheckedIn { get; private set; }
        public IList<EventParticipants> AllParticipants { get; set; }

        [BindProperty]
        public string EventId { get;  set; }

        public KalenderModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
        }
        public List<Events> EventsList { get; set; }
        public void OnGet()
        {
            // get name of user
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            EventsList = FetchEvents();
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
            AllParticipants = getAllParticipants();

        }
        public List<Events> FetchEvents()
        {
            // Fetch all events from the database
            return _context.Events.ToList();
        }

        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

        public bool IsUserJoined(int eventId)
        {
            var userId = _userManager.GetUserId(User);
            Console.WriteLine("IsUserJoined: " + userId + " " + eventId);
            // hiermee kijk je dan of de persoon is aangemeld ( werkt is code van jona)
            bool userAlreadyParticipant = _context.EventParticipants
                .Any(ep => ep.EventId == eventId && ep.UserId == userId);

            // return true of false
            return userAlreadyParticipant;
        }

        public async Task<IActionResult> OnPostAdd()
        {
            await Console.Out.WriteLineAsync("AAAAAAAAA");
             Console.WriteLine(EventId);
            var id = Request.Form["eventIdInput"];
            var eventId = Convert.ToInt32(id);
            //await Console.Out.WriteLineAsync(id);
            var userId = _userManager.GetUserId(User);
            //int eventId = Convert.ToInt32(EventId);
            if(EventId == null)
            {
                await Console.Out.WriteLineAsync("dd");
            }
            await Console.Out.WriteLineAsync(EventId);
            // Check if the user is already a participant in the event
            bool userAlreadyParticipant = getAllParticipants()
                .Any(ep => ep.EventId == eventId && ep.UserId == userId);

            if (!userAlreadyParticipant)
            {
                var eventParticipant = new EventParticipants
                {
                    EventId = eventId,
                    UserId = userId
                };

                _context.EventParticipants.Add(eventParticipant);
                await _context.SaveChangesAsync();
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLeave()
        {
            var userId = _userManager.GetUserId(User);
            var id = Request.Form["eventIdInput"];
            // Find and remove the user from the participants list of the event
            var participantToRemove = _context.EventParticipants
                .FirstOrDefault(ep => ep.EventId == id && ep.UserId == userId);

            if (participantToRemove != null)
            {
                _context.EventParticipants.Remove(participantToRemove);
                await _context.SaveChangesAsync();
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            return RedirectToPage();
        }

        public IList<EventParticipants> getAllParticipants()
        {
            var eventIds = _context.Events
                .Select(e => e.Id)
                .ToList();
            var eventParticipants = _context.EventParticipants
                .Where(ep => eventIds.Contains(ep.EventId))
                .ToList();
            return eventParticipants;
        }


    }
}
