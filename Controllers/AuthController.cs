using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Dto;
using MyMvcApp.Data;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
// using BCrypt.Net;






namespace MyMvcApp.Controllers
{
   
   // Controller Class inhereted -------> MVC Controller
   // ControllerBase Class inhereted ---> API Controller
   public class AuthController(AppDbContext _context) : Controller
    {
    
        // // Traditional General constructor dependency injection for AppDbContext.
        // private readonly AppDbContext _context;
        // public AuthController(AppDbContext context)
        // {
        //     _context = context;
        // }




        [HttpGet]    // → Give me the Login page
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]     // → Give me the registration page
        public IActionResult Register()
        {
            return View();
        }





        // explicitly tells ASP.NET: This action handles POST requests.
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterUserDto registerUserDto)
        {

            // 1. e.g. when the user submits the form with invalid data compared with Dto/RegisterUserDto.cs annotations , we want to show them the same form again, but with their previously entered data still filled in, so they don't have to retype everything.
            if (!ModelState.IsValid)
            {
                return View("Register", registerUserDto);
            }



            // 2. Normalize the input email:
            var email = registerUserDto.Email.Trim().ToLowerInvariant();


            // 3. Check if the user already exists in the database..
            // AsNoTracking() optimization -- "I'm only reading this user. I don't plan to modify it."
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);


            // 4.
            if (existingUser != null)
            {
                // 4.1. If the user already exists, we want to show them the same form again, but with their previously entered data still filled in, so they don't have to retype everything.
                
                ModelState.AddModelError(
                    nameof(registerUserDto.Email),
                    "An account with this email already exists."
                );
                return View("Register", registerUserDto);
            }



            // 5. Hash the password before storing it..
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);


            // 6. Create a new User entity and populate it with the data from the DTO..
            var user = new User
            {
                Username = registerUserDto.Username,
                Email = email,
                PasswordHash = hashedPassword
            };

            // Save the user to the database..
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            // 7. Registration successful
            return RedirectToAction(nameof(Login));

 
        }
    }

}