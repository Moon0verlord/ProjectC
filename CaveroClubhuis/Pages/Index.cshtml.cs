using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaveroClubhuis.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly CaveroClubhuisContext _context;
    private readonly UserManager<CaveroUser> _userManager;
    private readonly LayoutTools _layoutTools;
    
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public int PeopleCount { get; private set; }
    public bool IsUserCheckedIn { get; private set; }


    public IndexModel(ILogger<IndexModel> logger,CaveroClubhuisContext context,UserManager<CaveroUser> userManager, LayoutTools layoutTools)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _layoutTools = layoutTools;
    }

    public void OnGet()
    {
        var peopleCount = _context.InOffice.Count();
        
        // get name of user
        var userId = _userManager.GetUserId(User);
        (FirstName, LastName) = _layoutTools.LoadName(userId);
        IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
    }
    
    public async Task<IActionResult> OnPostToggleCheckInAsync()
    {
        var userId = _userManager.GetUserId(User);
        _layoutTools.ToggleCheckIn(userId);

        return RedirectToPage();
    }
    

}