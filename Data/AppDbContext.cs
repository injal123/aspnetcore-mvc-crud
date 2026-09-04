

// AppDbContext is basically the bridge between my C# classes and your database.






// using Microsoft.EntityFrameworkCore;
// using MyMvcApp.Models;      // "using classes from  MyMvcApp.Models namespace."


// Using General constructor:
// namespace MyMvcApp.Data
// {
//     public class AppDbContext : DbContext
//     {

//         // The constructor receives DbContextOptions<AppDbContext>
//         // through ASP.NET Core's Dependency Injection, configured in Program.cs.
//         // The options are passed to the base DbContext.
//         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
//         {
//         }

//         // “EF Core, I have a User class. I want you to manage the Users table in my database.”
//         public DbSet<User> Users { get; set;}
//     }
// }






// Microsoft.EntityFrameworkCore

// This package gives you things like:
// DbContext
// DbContextOptions
// DbSet







using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models; 


// Using Primary constructor:

namespace MyMvcApp.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
