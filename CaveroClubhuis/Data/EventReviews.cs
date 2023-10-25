using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;

namespace CaveroClubhuis.Data;

public class EventReviews
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FeedbackId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string FeedbackText { get; set; }

    [ForeignKey("EventId")]
    public Events Event { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }
}
