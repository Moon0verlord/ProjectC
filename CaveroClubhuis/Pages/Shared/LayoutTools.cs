using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Identity;

namespace CaveroClubhuis.Pages.Shared;

public class LayoutTools
{
    private readonly CaveroClubhuisContext _context;
    private readonly UserManager<CaveroUser> _userManager;
    
    
    public LayoutTools(CaveroClubhuisContext context,UserManager<CaveroUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    
    public (string FirstName, string LastName) LoadName(string userId)
    {
        var user = _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.FirstName, u.LastName })
            .FirstOrDefault();

        return user != null ? (user.FirstName, user.LastName) : (null, null);
    }


    public void CheckIn(string userid)
    {
        var inOfficeEntry = new InOffice
        {
            UserId = userid,
            IsRecurring = false,
            CheckInDate = DateTime.UtcNow,
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
    }
    
    public void CheckOut(string userId)
    {
        var inOfficeRecord = _context.InOffice
            .FirstOrDefault(io => io.UserId == userId && io.CheckOutDate == null);

        if (inOfficeRecord != null)
        {
            inOfficeRecord.CheckOutDate = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    
    public bool IsUserCheckedIn(string userId)
    {
        return _context.InOffice.Any(io => io.UserId == userId && io.CheckOutDate == null);
    }
    
    public void ToggleCheckIn(string userId)
    {
        var currentStatus = IsUserCheckedIn(userId);

        if (currentStatus)
        {
            CheckOut(userId);
        }
        else
        {
            CheckIn(userId);
        }
    }


}