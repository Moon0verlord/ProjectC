using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages
{
    public class KalenderModel : PageModel
    {
        private readonly ILogger<KalenderModel> _logger;

        public KalenderModel(ILogger<KalenderModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}