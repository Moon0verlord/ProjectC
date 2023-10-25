using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaveroClubhuis.Data;
using Microsoft.AspNetCore.Identity;

namespace CaveroClubhuis.Areas.Identity.Data;

// Add profile data for application users by adding properties to the CaveroUser class
public class CaveroUser : IdentityUser
{
    [PersonalData]
    public string? FirstName { get; set; }
    [PersonalData]
    public string? LastName { get; set; }
    [PersonalData]
    public string? Team { get; set; }
    
}

