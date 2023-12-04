using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using CaveroClubhuis.Pages.Shared;
using Microsoft.AspNetCore.Identity;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Calendars;

namespace CaveroClubhuis.Pages;

[Authorize]
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
    public List<PersonInfo> People { get; private set; }
    public DateTime MinDate { get; set; }
    public string ErrorMessage { get; set; }
    
    [BindProperty]
    public DateTime StartDate { get; set; }
    [BindProperty]
    public DateTime EndDate { get; set; }
    
    [BindProperty]
    public string daysofweek { get; set; }

    
    public IndexModel(ILogger<IndexModel> logger,CaveroClubhuisContext context,UserManager<CaveroUser> userManager, LayoutTools layoutTools)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _layoutTools = layoutTools;
    }

    public void OnGet()
    {
        //DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        var now = DateTimeOffset.UtcNow;
        PeopleCount = _context.InOffice.AsEnumerable().Where(x => !x.CheckOutDate.HasValue || now <= TimeZoneInfo.ConvertTimeToUtc((DateTime)x.CheckOutDate.Value, TimeZoneInfo.Utc)).Count();
        // get name of user
        var userId = _userManager.GetUserId(User);
        (FirstName, LastName) = _layoutTools.LoadName(userId);
        IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
        
        People = CheckInOverview();
        MinDate = new DateTime(2023, 1, 1, 12, 0, 0);
        StartDate = DateTime.Now;
        EndDate = DateTime.Now;
        if (TempData["ErrorMessage"] != null)
        {
            ErrorMessage = TempData["ErrorMessage"].ToString();
        }

    }

    public List<PersonInfo> CheckInOverview()
    {
        var now = DateTimeOffset.UtcNow;
        var people = _context.InOffice
        .Join(
            _context.Users,
            i => i.UserId,
            c => c.Id,
            (i, c) => new { InOffice = i, CaveroUser = c }
        )
        .AsEnumerable() 
        .Where(ti => !ti.InOffice.CheckOutDate.HasValue || now <= TimeZoneInfo.ConvertTimeToUtc(ti.InOffice.CheckOutDate.Value, TimeZoneInfo.Utc))
        .Select(ti => new PersonInfo
        {
            FirstName = ti.CaveroUser.FirstName,
            LastName = ti.CaveroUser.LastName,
            Team = ti.CaveroUser.Team
        })
        .ToList();
        return people;
        
    }
    
    
    public bool RecurringCheckIn(string userid, DateTime Start, DateTime End, string dayOfWeek)
    {
        DateTime utcStartDate = TimeZoneInfo.ConvertTimeToUtc(Start);
        DateTime utcEndDate = TimeZoneInfo.ConvertTimeToUtc(End);
        
        var inOfficeEntry = new InOffice
        {
            UserId = userid,
            CheckInDate = utcStartDate,
            CheckOutDate = utcEndDate,
            IsRecurring = true,
            DayOfWeek = dayOfWeek
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
        return true;
    }
    
    
    public async Task<IActionResult> OnPostToggleCheckInAsync()
    {
        var userId = _userManager.GetUserId(User);
        _layoutTools.ToggleCheckIn(userId);

        return RedirectToPage();
    }
    
    

    public IActionResult OnPostCheckIn()
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Model is not valid");
            return Page();
        }
    
        var userId = _userManager.GetUserId(User);
        bool checkInSuccessful = RecurringCheckIn(userId, StartDate,EndDate,daysofweek);
    
        if (!checkInSuccessful)
        {
            TempData["ErrorMessage"] = "You are already checked in on this date.";
            return RedirectToPage();
        }
        
        return RedirectToPage();
    }

    
}
public class PersonInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Team { get; set; }
}
