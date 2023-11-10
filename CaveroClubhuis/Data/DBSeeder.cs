namespace CaveroClubhuis.Data;

using System;
using Microsoft.EntityFrameworkCore;

public class DBSeeder
{
    
    public static void InitializeEvents(CaveroClubhuisContext context)
    {
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.Events.Any())
        {
            Console.WriteLine("Events table has already seeded.");
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
                StartTime = DateTime.UtcNow.AddHours(2).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(4).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Online",
                Approval = true
            },
            new Events
            {
                Title = "Capitol Visit",
                Description = "We will be visiting the capitol to convince the government to ban baldness",
                Date = DateTime.UtcNow.Date.AddDays(2),
                StartTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Den Haag",
                Approval = false
            },
            new Events
            {
                Title = "Spawning Yakub",
                Description = "We will be attempting the summoning of Yakub.",
                Date = DateTime.UtcNow.Date.AddDays(5),
                StartTime = DateTime.UtcNow.AddHours(5).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(6).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Arabia",
                Approval = true
            },
            new Events
            {
                Title = "Joining the war on drugs",
                Description = "We will be joining the war on drugs. by selling drugs.",
                Date = DateTime.UtcNow.Date.AddDays(1),
                StartTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(4).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "The streets",
                Approval = false
            },
            new Events
            {
                Title = "Tax Fraud part 2",
                Description = "We will be committing tax fraud. again.",
                Date = DateTime.UtcNow.Date.AddDays(7),
                StartTime = DateTime.UtcNow.AddHours(6).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Belastingdienst HQ",
                Approval = true
            }
        };
        context.Events.AddRange(initialEvents);
        context.SaveChanges();
    }
    
    // Seed your database with initial in-office entry
    public static void InitializeInOffice(CaveroClubhuisContext context)
    {
        var inOfficeEntry = new InOffice
        {
            UserId = GetRandomUserId(context),
            CheckInDate = DateTime.UtcNow,
        };
        context.InOffice.Add(inOfficeEntry);
        context.SaveChanges();
    }
    
    // Seed your database with initial event participant
    public static void InitializeEventParticipants(CaveroClubhuisContext context)
    {
        var eventParticipant = new EventParticipants
        {
            UserId = GetRandomUserId(context),
            EventId = GetRamdomEventId(context),
            ResponseStatus = Responses.Going,
        };
        context.EventParticipants.Add(eventParticipant);
        context.SaveChanges();
    }
    
    // Seed your database with initial recurring check-in
    public static void InitializeRecurringCheckIn(CaveroClubhuisContext context)
    {
        var recurringCheckIn = new RecurringCheckIn
        {
            UserId = GetRandomUserId(context),
            CheckInTime = DateTime.UtcNow.AddHours(2).TimeOfDay,
            DayOfWeek = DayOfWeek.Monday.ToString(),
        };
        context.RecurringCheckIns.Add(recurringCheckIn);
        context.SaveChanges();
    }
    
    // Seed your database with initial review
    public static void InitializeReviews(CaveroClubhuisContext context)
    {
        var Review = new EventReviews()
        {
            UserId = GetRandomUserId(context),
            EventId = GetRamdomEventId(context),
            FeedbackText = "10/10 would recommend",
        };
        context.EventReviews.Add(Review);
        context.SaveChanges();
    }
    
    // Get a random user id from the database
    private static string GetRandomUserId(CaveroClubhuisContext context)
    {
        {
            var randomUserId = context.Users
                .OrderBy(r => Guid.NewGuid())
                .Select(user => user.Id)
                .FirstOrDefault();

            return randomUserId;
        }

    }
    
    // Get a random event id from the database
    private static int GetRamdomEventId(CaveroClubhuisContext context)
    {
        {
            var randomEventId = context.Events
                .OrderBy(r => Guid.NewGuid())
                .Select(events => events.Id)
                .FirstOrDefault();

            return randomEventId;
        }
    }
    
    // Get the time of the An in the database
    public static TimeSpan GetEventTime(CaveroClubhuisContext dbContext)
    {
        var events = dbContext.Events
            .SingleOrDefault(e => e.Id == 1);
            return events!.StartTime;
    }
}
