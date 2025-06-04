using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_Lecturers
{
    public class EditModel : PageModel
    {
        private readonly RecSysContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(RecSysContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Lecturer Lecturer { get; set; }

        [BindProperty]
        public IFormFile ImageUpload { get; set; }

        [BindProperty]
        public bool RemoveImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Lecturer = await _context.Lecturers.FindAsync(id);

            if (Lecturer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var lecturerToUpdate = await _context.Lecturers.FindAsync(id);

            if (lecturerToUpdate == null)
            {
                return NotFound();
            }

            // Update editable fields
            lecturerToUpdate.SvName = Lecturer.SvName;
            lecturerToUpdate.SvExpertise = Lecturer.SvExpertise;
            lecturerToUpdate.Eligibility = Lecturer.Eligibility;
            lecturerToUpdate.Email = Lecturer.Email;

            // Handle image removal
            if (RemoveImage)
            {
                if (!string.IsNullOrEmpty(lecturerToUpdate.ImagePath))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, lecturerToUpdate.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    lecturerToUpdate.ImagePath = null;
                }
            }

            // Handle new image upload
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(lecturerToUpdate.ImagePath))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, lecturerToUpdate.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/lecturers");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                lecturerToUpdate.ImagePath = "/images/lecturers/" + uniqueFileName;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LecturerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool LecturerExists(int id)
        {
            return _context.Lecturers.Any(e => e.SvId == id);
        }
    }
}