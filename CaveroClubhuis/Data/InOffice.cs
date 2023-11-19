using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Data;


public class InOffice
{    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InOfficeId { get; set; } // Primary key

    [Required]
    public string UserId { get; set; }

    // Date & Time is stored in UTC Timezone
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
    
    public DateTime? CheckOutDate { get; set; }
    
    public string? DayOfWeek { get; set; }
    
    // Indicates if the check-in is part of a recurring pattern
    public bool? IsRecurring { get; set; }
}
