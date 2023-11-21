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
        public IList<CaveroUser> UsersList { get; set; }

        
        private readonly CaveroClubhuisContext _context;
        private readonly UserManager<CaveroUser> _userManager;
        private readonly LayoutTools _layoutTools;
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        
        public bool IsUserCheckedIn { get; private set; }

        
        
        public EventsModel(CaveroClubhuisContext context,UserManager<CaveroUser> userManager, LayoutTools layoutTools)
        {
            _context = context;
            _userManager = userManager;
            _layoutTools = layoutTools;
            

        }
        
        public void OnGet()
        {
            //var atendees = getUsersPerEvent();
            EventsList = FetchEvents();
            oldEvents = OldEvents();
            var userId = _userManager.GetUserId(User);
            (FirstName, LastName) = _layoutTools.LoadName(userId);
            IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);

            
        }
        
        
        public IList<Events> FetchEvents()
        {
            //make it to where it only takes the events later then the current date

            DateTime currentDateTime = DateTime.UtcNow - TimeSpan.FromHours(12);

            // Filter the data to get events after the current datetime
            var filteredEvents = _context.Events
                .Where(e => e.Date > currentDateTime) 
                .ToList();

            return filteredEvents;
        }

        public IList<Events> OldEvents()
        {
            //make it to where it only takes the events later then the current date

            DateTime currentDateTime = DateTime.UtcNow - TimeSpan.FromHours(12);

            // Filter the data to get events after the current datetime
            var filteredEvents = _context.Events
                .Where(e => e.Date <= currentDateTime)
                .ToList();

            return filteredEvents;
        }

        /*public IList<CaveroUser> getUsersPerEvent()
        {
            // first get all the event id's then link it with the EventParticipants table after that link the EventParticipants table with the CaveroUser table to get the users
            var eventIds = _context.Events
                .Select(e => e.Id)
                .ToList();
            var eventParticipants = _context.EventParticipants
                .Where(ep => eventIds.Contains(ep.EventId))
                .ToList();
            var users = _context.Users
                .Where(u => eventParticipants.Select(ep => ep.UserId).Contains(u.Id))
                .ToList();
            return users;
        }*/
        
        
        public async Task<IActionResult> OnPostToggleCheckInAsync()
        {
            var userId = _userManager.GetUserId(User);
            _layoutTools.ToggleCheckIn(userId);

            return RedirectToPage();
        }
    }
}
