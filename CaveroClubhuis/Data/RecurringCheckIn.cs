using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;

namespace CaveroClubhuis.Data;

public class RecurringCheckIn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RecurringCheckInId { get; set; } // Primary key

    [Required]
    public string UserId { get; set; }

    // Day of the week (e.g., "Friday")
    [Required]
    public string DayOfWeek { get; set; }
    
    [Required]
    public TimeSpan CheckInTime { get; set; }

    // Optional: End date for the recurrence
    public DateTime? EndDate { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
}
