using Microsoft.AspNetCore.Mvc.RazorPages;
using CaveroClubhuis.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CaveroClubhuis.Pages
{
    public class KalenderModel : PageModel
    {
        private readonly CaveroClubhuisContext _context;

        public KalenderModel(CaveroClubhuisContext context)
        {
            _context = context;
        }

        public List<Events> EventsList { get; set; }

        public void OnGet()
        {
            EventsList = FetchEvents();
        }

        public List<Events> FetchEvents()
        {
            // Fetch all events from the database
            return _context.Events.ToList();
        }
    }
}
