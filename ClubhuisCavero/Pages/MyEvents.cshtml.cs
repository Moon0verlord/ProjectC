using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages
{
    public class MyEventsModel : PageModel
    {
        private readonly ILogger<MyEventsModel> _logger;

        public MyEventsModel(ILogger<MyEventsModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}