using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;

namespace CaveroClubhuis.Data;

public class InOffice
{
    [Key, Column(Order = 1)]
    public int UserId { get; set; }

    [Key, Column(Order = 2)]
    public DateTime CheckInDate { get; set; }

    [Required]
    public string RecurringDays { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
}
