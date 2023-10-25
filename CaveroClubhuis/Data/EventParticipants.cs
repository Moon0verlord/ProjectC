using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Data;


public class EventParticipants
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EventParticipantId { get; set; } // Primary key

    public string UserId { get; set; }
    public int EventId { get; set; }

    [Required]
    public Responses ResponseStatus { get; set; }

    [ForeignKey("UserId")]
    public CaveroUser User { get; set; }

    [ForeignKey("EventId")]
    public Events Event { get; set; }
}



public enum Responses
{
    Going,
    NotGoing,
    Maybe
}

