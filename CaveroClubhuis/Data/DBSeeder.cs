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


        // Seed database with initial events
        var initialEvents = new Events[]
        {
            new Events
            {
                Title = "Yakub has been Spawned. What now?",
                Description = "We didn't think this far ahead. What do we do now?",
                Date = DateTime.UtcNow.Date.AddDays(3),
                StartTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(12).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "The Cube",
                Approval = true
            },
            new Events
            {
                Title = "The battle of the balds",
                Description = "The balds have taken over. We can't let this happen we must fight back.",
                Date = DateTime.UtcNow.Date.AddDays(2),
                StartTime = DateTime.UtcNow.AddHours(13).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(15).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Baldlandia",
                Approval = false
            },
            new Events
            {
                Title = "Stealing the Cube",
                Description = "They have a cube. We want the cube. We will take the cube.",
                Date = DateTime.UtcNow.Date.AddDays(5),
                StartTime = DateTime.UtcNow.AddHours(18).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(20).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Arabia",
                Approval = true
            },
            new Events
            {
                Title = "Taking DMT with Joe Rogan",
                Description = "Joseph James Rogan has invited us to take DMT with him and interview God Himself.",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,19), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(9).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Mars?",
                Approval = true
            },
            new Events
            {
                Title = "Joining FeetFinder",
                Description = "We have feet. We want to find other feet. We will join FeetFinder and share feet.",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,12), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(6).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "feetfinder.com",
                Approval = true
            },
            new Events
            {
                Title = "Rigging the 2024 Presidential Election",
                Description = "The russians have asked us to rig the 2024 presidential election. MAGA2024",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,18), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "The US of A",
                Approval = true
            },
            new Events
            {
                Title = "Abolishing the Zoomers",
                Description = "We must delete every Zoomer born after 2002, they are corrupting the youth with their lack of brain cells.",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,17), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(7).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(9).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "Everywhere",
                Approval = true
            },
            new Events
            {
                Title = "What is a skibidi toilet?",
                Description = "What are these kids saying? What is a skibidi toilet? why are they saying it?",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,11), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "On the streets",
                Approval = true
            },
            new Events
            {
                Title = "Whats going on in the sewers of New York?",
                Description = "We have been told people are going dripped out into the sewers of new york city. We must investigate.",
                Date = DateTime.SpecifyKind(new DateTime(2024, 1, 10), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(8).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "New York",
                Approval = true
            },
            new Events
            {
                Title = "Liberating Taiwan",
                Description = "The homie himself Xi Jinping has asked us to liberate Taiwan from the liberals. By our hand Taiwan will be free.",
                Date = DateTime.SpecifyKind(new DateTime(2024,01,19), DateTimeKind.Utc),
                StartTime = DateTime.UtcNow.AddHours(9).TimeOfDay,
                EndTime = DateTime.UtcNow.AddHours(10).TimeOfDay,
                UserId = GetRandomUserId(context),
                Location = "The South China Sea",
                Approval = true
            },
        };
        context.Events.AddRange(initialEvents);
        context.SaveChanges();
    }

    public static void InitializeTeams(CaveroClubhuisContext context)
    {
        context.Database.EnsureCreated();

        if (context.Teams.Any())
        {
            Console.WriteLine("Teams table has already seeded.");
            return;
        }
        var initialTeams = new Teams[]
        {
            new Teams
            {
                Title = ".Net Developers"
            },
            new Teams
            {
                Title = "Java Developers"
            },
            new Teams
            {
                Title = "Testers"
            },
            new Teams
            {
                Title = "Business Consultants"
            },
            new Teams
            {
                Title = "Microsoft 365 Consultants"
            }
        };
        context.Teams.AddRange(initialTeams);
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
