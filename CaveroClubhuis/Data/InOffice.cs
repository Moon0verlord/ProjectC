using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Data;

[Keyless]
public class InOffice
{
    [Key, Column(Order = 1)]
    public string UserId { get; set; }

    [Key, Column(Order = 2)]
    public DateTime CheckInDate { get; set; }

    [Required]
    public string RecurringDays { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
}
