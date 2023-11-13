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
            CheckInDate = DateTime.UtcNow,
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
    }
    
    public void CheckOut(string userid)
    {
        var inOfficeEntry = new InOffice
        {
            UserId = userid,
            CheckOutDate = DateTime.UtcNow,
        };
        _context.InOffice.Add(inOfficeEntry);
        _context.SaveChanges();
    }

}