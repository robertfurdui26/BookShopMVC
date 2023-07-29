using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShopMVC.Data;
using BookShopMVC.Models;
using System.Reflection.Metadata;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Document = iTextSharp.text.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BookShopMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookShopDbContext _context;

        public BooksController(BookShopDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return _context.BookShoop != null ?
                        View(await _context.BookShoop.ToListAsync()) :
                        Problem("Entity set 'BookShopDbContext.BookShoop'  is null.");
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BookShoop == null)
            {
                return NotFound();
            }

            var bookModel = await _context.BookShoop
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookModel == null)
            {
                return NotFound();
            }

            return View(bookModel);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Author,Created,Price")] BookModel bookModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookModel);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BookShoop == null)
            {
                return NotFound();
            }

            var bookModel = await _context.BookShoop.FindAsync(id);
            if (bookModel == null)
            {
                return NotFound();
            }
            return View(bookModel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Author,Created,Price")] BookModel bookModel)
        {
            if (id != bookModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookModelExists(bookModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookModel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BookShoop == null)
            {
                return NotFound();
            }

            var bookModel = await _context.BookShoop
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookModel == null)
            {
                return NotFound();
            }

            return View(bookModel);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BookShoop == null)
            {
                return Problem("Entity set 'BookShopDbContext.BookShoop'  is null.");
            }
            var bookModel = await _context.BookShoop.FindAsync(id);
            if (bookModel != null)
            {
                _context.BookShoop.Remove(bookModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookModelExists(int id)
        {
            return (_context.BookShoop?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> CreateOrder([Bind("Id,Name,Adress,Email,Phone")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexOrder));
            }
            return View(person);
        }

        // GET: Books

        public async Task<IActionResult> IndexOrder()
        {
            return _context.PersonBookShoop != null ?
                        View(await _context.PersonBookShoop.ToListAsync()) :
                        Problem("Entity set 'BookShopDbContext.BookShoop'  is null.");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            if (_context.BookShoop == null)
            {
                return Problem("Entity set'BookShopDbContext' is null");
            }

            var books = await _context.BookShoop.ToListAsync();

            //creare pdf

            var document = new Document();
            var memoryStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, memoryStream); ;
            document.Open();

            var paragraph = new Paragraph("Books List");
            document.Add(paragraph);

            foreach (var book in books)
            {
                var booksInfo = $"Titlul: {book.Title}\n" +
                    $"Author: {book.Author}\n" +
                    $"Descriere: {book.Description}\n" +
                    $"Created: {book.Created}\n" +
                    $"Price: {book.Price}\n";
                document.Add(new Paragraph(booksInfo));
                document.Add(new Paragraph("--------------------------------------------------"));





            }

            //add table to pdf
            document.Add(paragraph);
            document.Close();

            //conf raspunsul http pt a return fisierul pdf

            var fileName = "Books.pdf";
            var contentType = "application/pdf";
            var fileContent = memoryStream.ToArray();
            return File(fileContent, contentType, fileName);
        }

        public async Task<IActionResult> ExportOrder()
        {
            if (_context.PersonBookShoop == null)
            {
                return Problem("Entity set'BookShopDbContext' is null");
            }

            var order = await _context.PersonBookShoop.ToListAsync();

            //creare pdf

            var document = new Document();
            var memoryStream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, memoryStream); ;
            document.Open();

            var paragraph = new Paragraph("Order List");
            document.Add(paragraph);

            foreach (var person in order)
            {
                var booksInfo = $"Name: {person.Name}\n" +
                    $"Email: {person.Email}\n" +
                    $"Adress: {person.Adress}\n" +
                    $"Phone: {person.Phone}";
                document.Add(new Paragraph(booksInfo));
                document.Add(new Paragraph("--------------------------------------------------"));





            }

            //add table to pdf
            document.Add(paragraph);
            document.Close();

            //conf raspunsul http pt a return fisierul pdf

            var fileName = "OrderDataBase.pdf";
            var contentType = "application/pdf";
            var fileContent = memoryStream.ToArray();
            return File(fileContent, contentType, fileName);
        }

    }


}
