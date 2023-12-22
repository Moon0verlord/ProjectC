using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Identity;

namespace CaveroClubhuis.Pages.Shared;


public interface ILayoutTools
{
    (string FirstName, string LastName) LoadName(string userId);
    void CheckIn(string userid);
    void CheckOut(string userId);

    bool IsUserCheckedIn(string userId);
    void ToggleCheckIn(string userId);
    bool checkAdmin(string userId);
}

public class LayoutTools : ILayoutTools
{
    private readonly CaveroClubhuisContext _context;
    private readonly UserManager<CaveroUser> _userManager;
    
    
    public LayoutTools(CaveroClubhuisContext context,UserManager<CaveroUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    /// <summary>
    /// Loads the first and last name of a user specified by the user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A tuple containing the first and last name of the user, or (null, null) if the user does not exist.</returns>
    public (string FirstName, string LastName) LoadName(string userId)
    {
        var user = _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.FirstName, u.LastName })
            .FirstOrDefault();

        return user != null ? (user.FirstName, user.LastName) : (null, null);
    }


    /// <summary>
    /// Checks in a user to the office.
    /// </summary>
    /// <param name="userid">The id of the user to check in.</param>
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

    /// <summary>
    /// Checks out a user from the office.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
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

    /// <summary>
    /// Checks if a user is currently checked in.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user is checked in, false otherwise.</returns>
    public bool IsUserCheckedIn(string userId)
    {
        return _context.InOffice.Any(io => io.UserId == userId && io.CheckOutDate == null);
    }

    /// Toggles the check-in status of a user identified by their user ID.
    /// @param userId The unique identifier of the user.
    /// /
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

    /// <summary>
    /// Checks if the user with the given userId is an admin.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>True if the user is an admin, otherwise false.</returns>
    public bool checkAdmin(string userId)
    {
        string role = (from r in _context.Roles
                       where r.Id == (_context.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).FirstOrDefault())
                       select r.Name).FirstOrDefault()!;
        if (role == "Admin") return true;
        else return false;

    }

}