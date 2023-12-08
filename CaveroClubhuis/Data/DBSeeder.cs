namespace CaveroClubhuis.Data;

using System;
using Microsoft.EntityFrameworkCore;

public class DBSeeder
{
    /// <summary>
    /// Initializes the Events table in the provided CaveroClubhuisContext with initial data, if it is not already seeded.
    /// </summary>
    /// <param name="context">The CaveroClubhuisContext to initialize.</param>
    public static void InitializeEvents(CaveroClubhuisContext context)
    {
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.Events.Any())
        {
            Console.WriteLine("Events table has already seeded.");
            return;
        }
        
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


    /// <summary>
    /// Initializes a new entry for "In Office" in the database.
    /// </summary>
    /// <param name="context">The CaveroClubhuisContext instance representing the database context.</param>
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


    /// <summary>
    /// Initializes a new EventParticipants record in the database with random User and Event ids and a 'Going' response status.
    /// </summary>
    /// <param name="context">The CaveroClubhuisContext used for database operations.</param>
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


    /// <summary>
    /// InitializeReviews method is responsible for initializing a new event review in the CaveroClubhuisContext database.
    /// </summary>
    /// <param name="context">The CaveroClubhuisContext object representing the database context.</param>
    /// <remarks>
    /// This method generates a new EventReviews object with UserId, EventId, and FeedbackText properties set to random values.
    /// It then adds the new review to the EventReviews DbSet of the provided context and saves the changes to the database using context.SaveChanges().
    /// </remarks>
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


    /// <summary>
    /// Retrieves a random user id from the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>A random user id.</returns>
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


    /// <summary>
    /// Gets a random event ID from the database using the specified CaveroClubhuisContext.
    /// </summary>
    /// <param name="context">The CaveroClubhuisContext to get the random event ID.</param>
    /// <returns>A random event ID.</returns>
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


    /// <summary>
    /// Get the start time of a specific event from the provided CaveroClubhuisContext.
    /// </summary>
    /// <param name="dbContext">The CaveroClubhuisContext containing the events.</param>
    /// <returns>The start time of the specified event as a TimeSpan.</returns>
    public static TimeSpan GetEventTime(CaveroClubhuisContext dbContext)
    {
        var events = dbContext.Events
            .SingleOrDefault(e => e.Id == 1);
            return events!.StartTime;
    }
}
