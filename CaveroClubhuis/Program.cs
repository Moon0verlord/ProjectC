using CaveroClubhuis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CaveroClubhuis.Data;
using CaveroClubhuis.Controllers;
using CaveroClubhuis.Pages.Shared;
using Npgsql.Replication.TestDecoding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<CaveroClubhuisContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<CaveroUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CaveroClubhuisContext>();


// Add custom services
builder.Services.AddScoped<LayoutTools>();



builder.Services.AddRazorPages();

AccountController acc = new AccountController(builder.Environment);

var app = builder.Build();

// Seed database with with data (Optional)
using (var serviceScope = app.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    // database seeding
    var dbContext = serviceProvider.GetRequiredService<CaveroClubhuisContext>();
    //DBSeeder.InitializeEvents(dbContext); 
    //DBSeeder.InitializeInOffice(dbContext);
    //DBSeeder.InitializeReviews(dbContext);
    //DBSeeder.InitializeRecurringCheckIn(dbContext);
    //DBSeeder.InitializeEventParticipants(dbContext);
    //DBSeeder.InitializeTeams(dbContext);
}


// redirects user based on authentication
app.MapGet("/", (HttpContext ctx) =>
{
    if (ctx.User.Identity.IsAuthenticated)
    {
        return Results.Redirect("/Index");
    }
    else
    {
        return Results.Redirect("/Identity/Account/Login");
    }
});

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VlhhQlJCfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn9Sd0FjXX9ecHNRQWVe");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();