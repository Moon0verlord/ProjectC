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
    private readonly ILayoutTools _layoutTools;
    
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string ProfileImage { get; set; }
    public int PeopleCount { get; private set; }
    public bool IsUserCheckedIn { get; private set; }
    public List<PersonInfo> People { get; private set; }
    public IList<Events> EventsList { get; set; }
    public DateTime MinDate { get; set; }
    public string ErrorMessage { get; set; }
    
    [BindProperty]
    public DateTime StartDate { get; set; }
    [BindProperty]
    public DateTime EndDate { get; set; }
    
    [BindProperty]
    public string daysofweek { get; set; }

    
    public IndexModel(ILogger<IndexModel> logger,CaveroClubhuisContext context,UserManager<CaveroUser> userManager, ILayoutTools layoutTools)
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
        (FirstName, LastName, ProfileImage) = _layoutTools.LoadUserInfo(userId);        IsUserCheckedIn = _layoutTools.IsUserCheckedIn(userId);
        People = CheckInOverview();
        // krijg alle events
        EventsList = FetchEvents();
        MinDate = new DateTime(2023, 1, 1, 12, 0, 0);
        StartDate = DateTime.Now;
        EndDate = DateTime.Now;
        if (TempData["ErrorMessage"] != null)
        {
            ErrorMessage = TempData["ErrorMessage"].ToString();
        }

    }

    /// <summary>
    /// Retrieves a list of PersonInfo objects containing information about people currently checked in.
    /// </summary>
    /// <returns>A list of PersonInfo objects.</returns>
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


    /// <summary>
    /// Performs recurring check-in for a user within a specified time frame and on a specific day of the week.
    /// </summary>
    /// <param name="userid">The ID of the user.</param>
    /// <param name="Start">The start date and time of the recurring check-in.</param>
    /// <param name="End">The end date and time of the recurring check-in.</param>
    /// <param name="dayOfWeek">The day of the week on which the recurring check-in will occur.</param>
    /// <returns>None.</returns>
    public void RecurringCheckIn(string userid, DateTime Start, DateTime End, string dayOfWeek)
    {
        DateTime utcStartDate = TimeZoneInfo.ConvertTimeToUtc(Start);
        DateTime utcEndDate = TimeZoneInfo.ConvertTimeToUtc(End);
        dayOfWeek = "Monday";
        
        var inOfficeEntry = new InOffice
        {
            UserId = userid,
            CheckInDate = utcStartDate,
            CheckOutDate = utcEndDate,
            DayOfWeek = dayOfWeek,
            IsRecurring = true
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
    }


    /// <summary>
    /// Method to handle the toggle check-in action in an asynchronous manner.
    /// </summary>
    /// <returns>An instance of <see cref="Task<IActionResult>"/> representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostToggleCheckInAsync()
    {
        var userId = _userManager.GetUserId(User);
        _layoutTools.ToggleCheckIn(userId);

        return RedirectToPage();
    }
    
    public List<Events> FetchEvents()
    {
        DateTime today = DateTime.UtcNow.Date;
        DateTime endOfWeek = today.AddDays(6); // hierdoor is zondag de laatste dag
        // maak een case om te kijken welke dag het is en op basis daarvan bereken je door tot de zondag van de week indien het zondag is return niks

        // navragen of dit goed is of dat we 6 dagen van vandaag kijken of dat we tot einde van de week van de dag kijken

        // Fetch events van deze week
        var upcomingEvents = _context.Events
            .Where(e => e.Date >= today && e.Date <= endOfWeek) // met de >= today zorg je dat je de events na vandaag neemt tot 6 dagen in de toekomst
            .OrderBy(e => e.Date)  // order by date
            .ToList();

        return upcomingEvents;
    }


    /// <summary>
    /// Performs a recurring check for a user asynchronously.
    /// </summary>
    /// <returns>The <see cref="IActionResult"/> representing the result of the operation.</returns>
    public bool DoesRecurringCheckInExist(string userId, DateTime startDate, DateTime endDate)
    {
        // Convert the dates to UTC
        DateTime utcStartDate = TimeZoneInfo.ConvertTimeToUtc(startDate).Date;

        // Check if a recurring check-in exists for the given user and dates
        return _context.InOffice.Any(i => i.UserId == userId && i.CheckInDate.Date == utcStartDate);
    }
    
    public async Task<IActionResult> OnPostRecurringCheckAsync()
    {
        var userId = _userManager.GetUserId(User);

        if (DoesRecurringCheckInExist(userId, StartDate, EndDate))
        {
            TempData["ErrorMessage"] = "A recurring check-in already exists for the selected start date.";
            return RedirectToPage();
        }

        RecurringCheckIn(userId, StartDate, EndDate, daysofweek);

        return RedirectToPage();
    }
    
}
public class PersonInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Team { get; set; }
}
