using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using System.Security.Claims;

using MyMvcApp.Dto;
using MyMvcApp.Models;





namespace MyMvcApp.Controllers
{
    // This attribute ensures that only authenticated users can access the actions in this controller...... else redirected, as per the configuration in Program.cs.
    [Authorize]
    public class DashboardController(AppDbContext _context) : Controller
    {




        // 1. Display dashboard page.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the current user's ID from the authentication claims.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert from string → int
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Fetch only the current user's active notes.
            var notes = await _context.Notes
                .Where(n => n.UserId == userId && !n.IsDeleted)     // LINQ & Lambda Expression.
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();    // execute the query and get the results as a list.

            // Send the notes to the Razor View.
            return View(notes);
        }









        // 2. All Notes page.
        [HttpGet]
        public async Task<IActionResult> AllNotes()
        {
            // Get the current user's ID from the authentication claims.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert from string → int
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // Fetch only the current user's active notes.
            var notes = await _context.Notes
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();

            return View(notes);
        }









        // 3. Show CreateNote page.
        [HttpGet]
        public IActionResult CreateNote()
        {
            return View();
        }








        // 4. Handle CreateNote form submission.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNote(CreateNoteDto createNoteDto)
        {
            
            if (!ModelState.IsValid)
            {
                return View(createNoteDto);
            }


            // Get the current user's ID from the authentication claims.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert from string → int
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }



            // Create New Note in that user of id userId.
            var note = new Note
            {
                Title = createNoteDto.Title,
                Content = createNoteDto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Notes.Add(note);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Note created successfully!";

            return RedirectToAction(nameof(AllNotes));
            



        }





    }
}
