using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaveroClubhuis.Data;

public class CaveroClubhuisContext : IdentityDbContext<CaveroUser>
{

    public DbSet<Events> Events { get; set; }
    public DbSet<EventParticipants> EventParticipants { get; set; }
    public DbSet<EventReviews> EventReviews { get; set; }
    public DbSet<InOffice> InOffice { get; set; }
    public DbSet<Teams> Teams { get; set; }



    public CaveroClubhuisContext(DbContextOptions<CaveroClubhuisContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("Identity");
        //  Ignoring properties that are not needed *TBD*
        // builder.Entity<ApplicationUser>().Ignore(c => c.AccessFailedCount)
        //     .Ignore(c => c.LockoutEnabled)
        //     .Ignore(c => c.LockoutEnd)
        //     .Ignore(c => c.PhoneNumber)
        //     .Ignore(c => c.PhoneNumberConfirmed);
        //Changed email confirmed and 2FA to boolean to match the identity spec
        builder.Entity<CaveroUser>().Property(c => c.EmailConfirmed).HasColumnType("boolean");
        builder.Entity<CaveroUser>().Property(c => c.TwoFactorEnabled).HasColumnType("boolean");
        // Renaming tables to make them more readable
        builder.Entity<CaveroUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserToken<string>>().ToTable("AuthTokens");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("ExternalLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        // making response status a string
        builder.HasPostgresEnum<Responses>();
        builder.Entity<EventParticipants>()
            .Property(e => e.ResponseStatus)
            .HasConversion<string>();

    }
}
