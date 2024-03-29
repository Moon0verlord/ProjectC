using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CaveroClubhuis.Pages
{
    public class EventsModel : PageModel
    {
        
        public IList<Events> EventsList { get; set; }
        public IList<Events> oldEvents { get; set; }
        public IList<CaveroUser> Atendees { get; set; }
        
        public IList<EventParticipants> AllParticipants { get; set; }

        
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly ILayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        [BindProperty(SupportsGet = true)]
        public int EventId { get; set; }
        public string ProfileImage { get; set; }
        public string UserId { get; set; }
        public string Feedback { get; set; }

        public bool IsUserCheckedIn { get; private set; }



        public EventsModel(CaveroClubhuisContext context, UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
        }
        
        public void OnGet()
        {
            AllParticipants = getAllParticipants();
            Atendees = getUsersPerEvent();
            EventsList = FetchEvents();
            oldEvents = OldEvents();
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            
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
        
        public IList<Events> FetchEvents()
        {
            // Filter the data to get events after the current datetime
            DateTime currentDateTime = DateTime.UtcNow + TimeSpan.FromHours(1);

            var filteredEvents = _context.Events
                .Where(e => e.Date > currentDateTime).OrderBy(e => e.Date)
                .ToList();

            return filteredEvents;
        }

        // only show event that contain the current user
        public IList<Events> OldEvents()
        {
            DateTime currentDateTime = DateTime.UtcNow + TimeSpan.FromHours(1);
            
            var filteredEvents = _context.Events
                .Where(e => e.Date < currentDateTime).OrderBy(e => e.Date)
                .ToList();
            var userId = _userManager.GetUserId(User);
            var userEvents = _context.EventParticipants
                .Where(ep => ep.UserId == userId)
                .ToList();
            var oldEvents = new List<Events>();
            foreach (var userEvent in userEvents)
            {
                var eventToAdd = filteredEvents.Find(e => e.Id == userEvent.EventId);
                if (eventToAdd != null)
                {
                    oldEvents.Add(eventToAdd);
                }
            }
            return oldEvents;

        }

        public IList<CaveroUser> getUsersPerEvent()
        {
            // Get all the participants
            var eventParticipants = getAllParticipants();
            var users = _context.Users
                .Where(u => eventParticipants.Select(ep => ep.UserId).Contains(u.Id))
                .ToList();
            return users;
        }
        
        
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventParticipants other = (EventParticipants)obj;

            return EventId == other.EventId && UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + EventId.GetHashCode();
                hash = hash * 23 + UserId.GetHashCode();
                return hash;
            }
        }

        // add user to event when button is clicked but check if user is already in the event
        public async Task<IActionResult> OnPostAdd()
        {
            var userId = _userManager.GetUserId(User);

            // Check if the user is already a participant in the event
            bool userAlreadyParticipant = getAllParticipants()
                .Any(ep => ep.EventId == EventId && ep.UserId == userId);

            if (!userAlreadyParticipant)
            {
                var eventParticipant = new EventParticipants
                {
                    EventId = EventId,
                    UserId = userId
                };

                _context.EventParticipants.Add(eventParticipant);
                await _context.SaveChangesAsync();
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLeave(int eventId)
        {
            var userId = _userManager.GetUserId(User);

            // Find and remove the user from the participants list of the event
            var participantToRemove = _context.EventParticipants
                .FirstOrDefault(ep => ep.EventId == eventId && ep.UserId == userId);

            if (participantToRemove != null)
            {
                _context.EventParticipants.Remove(participantToRemove);
                await _context.SaveChangesAsync();
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            return RedirectToPage();
        }

        // Send feedback to database
        public async Task<IActionResult> OnPostSubmitFeedback(string feedback, int eventId)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var feedbackToAdd = new EventReviews
            {
                UserId = userId,
                FeedbackText = feedback,
                EventId = eventId
            };
            _context.EventReviews.Add(feedbackToAdd);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
