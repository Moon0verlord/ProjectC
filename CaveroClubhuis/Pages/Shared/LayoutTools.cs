using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using CaveroClubhuis.Areas.Identity.Data;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Identity;

namespace CaveroClubhuis.Pages.Shared;


public interface ILayoutTools
{
    (string FirstName, string LastName, string ProfileImage) LoadUserInfo(string userId);
    void CheckIn(string userid);
    void CheckOut(string userId);
    bool IsUserCheckedIn(string userId);
    void ToggleCheckIn(string userId);
    bool checkAdmin(string userId);
    Image GenerateInitialsImage(string initials);
    string ImageToBase64(Image image, ImageFormat format);
}

public class LayoutTools : ILayoutTools
{
    private readonly CaveroClubhuisContext _context;
    private readonly UserManager<CaveroUser> _userManager;
    private static readonly Color[] Colors = new[]
    {
        Color.Crimson,
        Color.MediumSeaGreen,
        Color.DodgerBlue,
        Color.PaleVioletRed,
        Color.MediumVioletRed,
    };

    
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
    public (string FirstName, string LastName, string ProfileImage) LoadUserInfo(string userId)
    {
        var user = _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.FirstName, u.LastName })
            .FirstOrDefault();

        if (user != null)
        {
            string initials = $"{user.FirstName?.FirstOrDefault()}{user.LastName?.FirstOrDefault()}";
            var image = GenerateInitialsImage(initials);
            string base64Image = ImageToBase64(image, ImageFormat.Png);
            return (user.FirstName, user.LastName, base64Image);
        }
        return (null, null, null);
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
    
    public Image GenerateInitialsImage(string initials)
    {
        var bitmap = new Bitmap(100, 100);

        // Deterministically select a color based on initials
        int colorIndex = Math.Abs(initials.GetHashCode()) % Colors.Length;
        var backgroundColor = Colors[colorIndex];

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(backgroundColor);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            using (var font = new Font("Arial", 40, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                var textSize = graphics.MeasureString(initials, font);
                graphics.DrawString(initials, font, Brushes.White, (100 - textSize.Width) / 2, (100 - textSize.Height) / 2);
            }
        }

        return bitmap;
    }

    
    public string ImageToBase64(Image image, ImageFormat format)
    {
        using (var ms = new MemoryStream())
        {
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }


  
}