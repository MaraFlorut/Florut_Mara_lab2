using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Data;
using Lab2.Models;
using Lab2.Migrations;

namespace Lab2.Pages.Books
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Lab2.Data.Lab2Context _context;

        public EditModel(Lab2.Data.Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            //se va include Author conform cu sarcina de la lab 2
            Book = await _context.Book
            .Include(b => b.Publisher)
            .Include(b => b.BookCategories).ThenInclude(b => b.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);


            var book =  await _context.Book.FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }
            //apelam PopulateAssignedCategoryData pentru o obtine informatiile necesare checkbox-
            //urilor folosind clasa AssignedCategoryData
            PopulateAssignedCategoryData(_context, Book);

            Book = book;
            ViewData["PublisherID"] = new SelectList(_context.Set<Publisher>(), "ID",
"PublisherName");
            var authors = _context.Author
            .Select(a => new {
                AuthorID = a.AuthorID,
                FullName = a.FirstName + " " + a.LastName
            })
            .ToList();

            ViewData["AuthorID"] = new SelectList(authors, "AuthorID", "FullName");
            //ViewData["AuthorID"] = new SelectList(_context.Set<Author>(), "ID",
            //"LastName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include related data for the book
            var bookToUpdate = await _context.Book
                .Include(i => i.Publisher)
                .Include(i => i.BookCategories)
                .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            // Validate the model state at the beginning
            if (!ModelState.IsValid)
            {
                PopulateAssignedCategoryData(_context, bookToUpdate); // Optional, depending on your requirements
                return Page();
            }

            // Update the book model
            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "Book",
                i => i.Title,
                i => i.Author,
                i => i.Price,
                i => i.PublishingDate,
                i => i.PublisherID))
            {
                // Update book categories based on the selected categories
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // If we reach here, it means the model update failed for some reason
            PopulateAssignedCategoryData(_context, bookToUpdate); // Optional, to repopulate data for error cases
            return Page();
        }


        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
