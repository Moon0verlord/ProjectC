using CaveroClubhuis.Areas.Identity.Data;

namespace CaveroClubhuis.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Events
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }
    // Date & Time is stored in UTC Timezone
    [Required]
    public DateTime Date { get; set; }
    // Date & Time is stored in UTC Timezone
    [Required]
    public DateTime Time { get; set; }

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    public CaveroUser User { get; set; }

    public string Location { get; set; }
    
    public bool Approval { get; set; }
}