using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Dto;
using MyMvcApp.Data;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;
// using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;





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




        [HttpGet]    // → Give me the Login page.
        public IActionResult Login()
        {
            return View();
        }




        [HttpGet]     // → Give me the registration page.
        public IActionResult Register()
        {
            return View();
        }





        // explicitly tells ASP.NET: This action handles POST requests.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            TempData["SuccessMessage"] = "Account created successfully. Please log in.";
            return RedirectToAction(nameof(Login));

        }
















        // [ValidateAntiForgeryToken] :
        // Protects POST requests from CSRF (Cross-Site Request Forgery) attacks.
        // Ensures the request came from our application's form.
        // Used mainly on actions that change data (Register, Login, Delete, Update, etc.).




        // Login method..
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> LoginUser(LoginUserDto loginUserDto)
        {
            
            // 1. e.g. when the user submits the form with invalid data compared with Dto/LoginUserDto.cs annotations , we want to show them the same form again, but with their previously entered data still filled in, so they don't have to retype everything.
            if (!ModelState.IsValid)
            {
                return View("Login", loginUserDto);
            }

            // 2. Normalize the input email:
            var email = loginUserDto.Email.Trim().ToLowerInvariant();

            // 3. Find the user.
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

            // 4. If the user doesn't exist or the password is incorrect, show an error message.
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
            {
                // string.Empty → error belongs to the whole form.. not just Email or...
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("Login", loginUserDto);
            }






            // 5. Create claims for the user... - - -  For cookie Authentication. 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            // now, anywhere in our application, we can ask: User.Identity?.Name



            // 6. Create the user's identity.
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );



            // 7. Create the user's principal.
            var principal = new ClaimsPrincipal(identity);

            // 8. Sign in the user.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                principal
            );




            // 9. Login successful
            TempData["SuccessMessage"] = "Login successful!";


            // 10. (Action, Controller)
            // Redirect to the Dashboard page after successful login.
            return RedirectToAction("Index", "Dashboard");

        }












        // Destroying the authentication cookie.
        [HttpPost]     // Post --> cz logging out changes authentication state.
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Logout()
        {
            // 1. Sign out the user.
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // 2. Logout successful
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            
            // 3. Redirect to the Login page after logout.
            return RedirectToAction(nameof(Login));
        }



    }
}
