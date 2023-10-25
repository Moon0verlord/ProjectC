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

    public DateTime CheckInDate { get; set; }

    [Required]
    public string RecurringDays { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
}
