# CaveroClubhuis Project

This is a .NET 7.0 project named CaveroClubhuis.
Prerequisites

Before you begin, ensure you have met the following requirements:

You have installed the latest version of [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)

## Installing CaveroClubhuis

To install CaveroClubhuis, follow these steps:

1. Clone the repository to your local machine.
2. Navigate to the project directory.

## NuGet Packages

This project uses several NuGet packages. Here are the key packages and their versions:
```
Syncfusion.EJ2.AspNet.Core Version="23.1.44"
Npgsql.EntityFrameworkCore.PostgreSQL Version="7.0.11"
Microsoft.EntityFrameworkCore.Tools Version="7.0.12"
Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore Version="7.0.11"
Microsoft.AspNetCore.Identity.EntityFrameworkCore Version="7.0.11"
Microsoft.AspNetCore.Identity.UI Version="7.0.11"
```

## Email Domain Verification

In the RegisterModel class, there is a commented out section of code that checks if the email domain of the user trying to register is @cavero. This is done in the OnPostAsync method.
```
// if uncommented users can only sign up using an email with a @cavero domain
// if (!Input.Email.Contains("@cavero"))
// {
//     ModelState.AddModelError(string.Empty, "U kunt zich alleen registreren met een @cavero e-mailadres.");
//     return Page();
// }
```

## Running CaveroClubhuis

To run CaveroClubhuis, follow these steps:

1. Open a terminal in the project directory.
2. Run the following command: ``dotnet run``


This will start the application. You can access it by navigating to https://localhost:5001 in your web browser (or as indicated in your terminal).
