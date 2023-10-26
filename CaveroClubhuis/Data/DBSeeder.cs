namespace CaveroClubhuis.Data;

using System;
using Microsoft.EntityFrameworkCore;

public class DBSeeder
{
    private readonly CaveroClubhuisContext _dbContext;
    public DBSeeder (CaveroClubhuisContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public static void InitializeEvents(CaveroClubhuisContext context)
    {
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.Events.Any())
        {
            return;
        }
        
        // Seed database with initial events
        var initialEvents = new Events[]
        {
            new Events
            {
                Title = "Why Bald People?",
                Description = "Exploring baldness and its effects on the human psyche",
                Date = DateTime.UtcNow.Date.AddDays(10),
                Time = DateTime.UtcNow.AddHours(10),
                UserId = GetRandomUserId(context),
                Location = "Online",
                Approval = true
            },
            new Events
            {
                Title = "Capitol Visit",
                Description = "We will be visiting the capitol to convince the government to ban baldness",
                Date = DateTime.UtcNow.Date.AddDays(2),
                Time = DateTime.UtcNow.AddHours(2),
                UserId = GetRandomUserId(context),
                Location = "Den Haag",
                Approval = false
            },
            new Events
            {
                Title = "Spawning Yakub",
                Description = "We will be attempting the summoning of Yakub.",
                Date = DateTime.UtcNow.Date.AddDays(5),
                Time = DateTime.UtcNow.AddHours(4),
                UserId = GetRandomUserId(context),
                Location = "Arabia",
                Approval = true
            },
            new Events
            {
                Title = "Joining the war on drugs",
                Description = "We will be joining the war on drugs. by selling drugs.",
                Date = DateTime.UtcNow.Date.AddDays(1),
                Time = DateTime.UtcNow.AddHours(8),
                UserId = GetRandomUserId(context),
                Location = "The streets",
                Approval = false
            },
            new Events
            {
                Title = "Tax Fraud part 2",
                Description = "We will be committing tax fraud. again.",
                Date = DateTime.UtcNow.Date.AddDays(7),
                Time = DateTime.UtcNow.AddHours(5),
                UserId = GetRandomUserId(context),
                Location = "Belastingdienst HQ",
                Approval = true
            }
        };
        context.Events.AddRange(initialEvents);
        context.SaveChanges();
    }
    
    // Seed your database with initial in-office entries
    public static void InitializeInOffice(CaveroClubhuisContext context)
    {
        var inOfficeEntry = new InOffice
        {
            UserId = GetRandomUserId(context),
            CheckInDate = DateTime.UtcNow,
            RecurringDays = "Monday, Wednesday, Friday"
        };
        context.InOffice.Add(inOfficeEntry);
        context.SaveChanges();
    }
    
    // Seed your database with initial reviews
    public static void InitializeReviews(CaveroClubhuisContext context)
    {
        var Review = new EventReviews()
        {
            UserId = GetRandomUserId(context),
            EventId = 1,
            FeedbackText = "10/10 would recommend",
        };
        context.EventReviews.Add(Review);
        context.SaveChanges();
    }
    
    // Get a random user id from the database
    public static string GetRandomUserId(CaveroClubhuisContext context)
    {
        {
            var randomUserId = context.Users
                .OrderBy(r => Guid.NewGuid())
                .Select(user => user.Id)
                .FirstOrDefault();

            return randomUserId;
        }

    }
    
    // Get the time of the An in the database
    public static DateTime GetEventTime(CaveroClubhuisContext dbContext)
    {
        var events = dbContext.Events
            .Where(e => e.Id == 1)
            .SingleOrDefault();
        
            return events.Time;
        
    }
}
