namespace CaveroClubhuis.Data;

using System;
using Microsoft.EntityFrameworkCore;

public class DBSeeder
{
    public static void Initialize(CaveroClubhuisContext context)
    {
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.Events.Any())
        {
            return;
        }
        

        
        // User ID to associate with the events
        // User ID is still hardcoded 
        string userId = "6a5dca18-52c6-4aa5-a4f7-60e08c0ecdd3";

        // Seed your database with initial events
        var initialEvents = new Events[]
        {
            new Events
            {
                Title = "Why Bald People?",
                Description = "Exploring baldness and its effects on the human psyche",
                Date = DateTime.UtcNow, 
                Time = DateTime.UtcNow.AddHours(2), 
                UserId = userId,
                Location = "Online",
                Approval = true
            },
            new Events
            {
                Title = "Capitol Visit",
                Description = "We will be visiting the capitol to convince the government to ban baldness",
                Date = DateTime.UtcNow, 
                Time = DateTime.UtcNow.AddHours(2), 
                UserId = userId,
                Location = "Den Haag",
                Approval = false
            },
            new Events
            {
                Title = "Spawning Yakub",
                Description = "We will be attempting the summoning of Yakub.",
                Date = DateTime.UtcNow, 
                Time = DateTime.UtcNow.AddHours(4), 
                UserId = userId,
                Location = "Arabia",
                Approval = true
            },
            new Events
            {
                Title = "Joining the war on drugs",
                Description = "We will be joining the war on drugs. by selling drugs.",
                Date = DateTime.UtcNow, 
                Time = DateTime.UtcNow.AddHours(8), 
                UserId = userId,
                Location = "The streets",
                Approval = false
            },
            new Events
            {
                Title = "Tax Fraud part 2",
                Description = "We will be committing tax fraud. again.",
                Date = DateTime.UtcNow, 
                Time = DateTime.UtcNow.AddHours(5), 
                UserId = userId,
                Location = "Belastingdienst HQ",
                Approval = true
            }
        };
        
        // Seed your database with initial in-office entries
        var inOfficeEntry = new InOffice
        {
            UserId = userId, 
            CheckInDate = DateTime.UtcNow, 
            RecurringDays = "Monday, Wednesday, Friday" 
        };
        
        
        context.Events.AddRange(initialEvents);
        context.InOffice.Add(inOfficeEntry);
        context.SaveChanges();
    }
}
