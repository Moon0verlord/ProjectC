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
    
    public DateTime SelectedDate { get; set; }
    
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
    

    public void CheckIn(string userid, DateTime SelectedDate)
    {
        Console.WriteLine(SelectedDate);
        var inOfficeEntry = new InOffice
        {
            UserId = userid,
            CheckInDate = SelectedDate,
            IsRecurring = false
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
    }
    
    public IActionResult OnPostCheckIn()
    {
        
        var userId = _userManager.GetUserId(User);
        CheckIn(userId, SelectedDate);

        // Optionally, you can perform other logic or redirect the user.
        return RedirectToPage();
    }
    
}
public class PersonInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Team { get; set; }
}


public class DateTimePicker
{
    [Required(ErrorMessage = "Please enter the value")]
    public DateTime? value { get; set; }

}

public class HomeController : Controller
{
    private DateTimePicker DateTimePickerValue;
    public ActionResult Index()
    {
        DateTimePickerValue.value =  new DateTime(2020, 03, 03, 10, 00, 00);
        return View(DateTimePickerValue);
    }
    [HttpPost]
    public ActionResult Index(DateTimePicker model)
    {

        DateTimePickerValue.value = model.value;
        Console.WriteLine($"TIMEEEEEEEEEEEEE{model.value}");
        return View(DateTimePickerValue);
    }
}