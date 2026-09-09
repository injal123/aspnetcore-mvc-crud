using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using System.Security.Claims;

using MyMvcApp.Dto;
using MyMvcApp.Models;
using Microsoft.AspNetCore.Antiforgery;





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

            // Fetch only the current user's active notes. List of Notes-> .Where,not Firstordefault
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
                Title = createNoteDto.Title.Trim(),
                Content = createNoteDto.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Notes.Add(note);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Note created successfully!";

            return RedirectToAction(nameof(AllNotes));

        }










        // 5. Show Edit Note page.
        [HttpGet]
        public async Task<IActionResult> EditNote(int id)
        {
            // Get the current user's ID from the authentication claims.
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert from string → int
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }


            // Fetch notes.
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => 
                    n.UserId == userId &&
                    n.Id == id &&
                    !n.IsDeleted
                );
            


            if (note == null)
            {
                return NotFound();
            }



            var noteDto = new CreateNoteDto
            {
                Title = note.Title,
                Content = note.Content
            };


            return View(noteDto);

        }











        // 6. Save Changes from Edit.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNote(int id, CreateNoteDto createNoteDto)
        {

            if (!ModelState.IsValid)
            {
                return View(createNoteDto);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }



            var note = await _context.Notes
                .FirstOrDefaultAsync(n => 
                    n.UserId == userId &&
                    n.Id == id &&
                    !n.IsDeleted
                );
            


            if (note == null)
            {
                return NotFound();
            }




            note.Title = createNoteDto.Title.Trim();
            note.Content = createNoteDto.Content.Trim();
            note.UpdatedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Note updated successfully!";
            return RedirectToAction(nameof(AllNotes));
        }












        // 7. DeleteNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNote(int id)
        {
            
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }


            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.UserId == userId &&
                    n.Id == id &&
                    !n.IsDeleted
                );

            
            if (note == null)
            {
                return NotFound();
            }

            note.IsDeleted = true;
            note.UpdatedAt = DateTime.UtcNow;



            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Note moved to trash.";
            return RedirectToAction(nameof(AllNotes));
        }












        // 8. Return Trash Page.
        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var notes = await _context.Notes
                .Where(n => n.UserId == userId && n.IsDeleted)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();

            return View(notes);
        }












        // 9. RestoreNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreNote(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.UserId == userId &&
                    n.Id == id &&
                    n.IsDeleted);

            if (note == null)
            {
                return NotFound();
            }

            note.IsDeleted = false;
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Note restored successfully.";

            return RedirectToAction(nameof(Trash));
        }













        // 10. Delete Permanently
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(n =>
                    n.UserId == userId &&
                    n.Id == id &&
                    n.IsDeleted);

            if (note == null)
            {
                return NotFound();
            }


            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Note permanently deleted.";
            return RedirectToAction(nameof(Trash));
        }





    }
}
