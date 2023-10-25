using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Data;

[Keyless]
public class EventParticipants
{
    [Key, Column(Order = 1)]
    public string UserId { get; set; }

    [Key, Column(Order = 2)]
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

