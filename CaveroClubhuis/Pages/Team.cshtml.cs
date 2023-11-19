using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages
{
    [Authorize]
    public class TeamModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
