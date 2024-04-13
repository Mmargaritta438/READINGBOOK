using Microsoft.EntityFrameworkCore;
using AuthApi.Models.BookReading;
using AuthAi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController(DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;

        // GET: Books
        [HttpGet]
        public async Task<List<Book>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: Books/Details/5
        [HttpGet]
        public async Task<string> Details([System.Web.Http.FromUri] int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book != null)
            {
                return book.Description;
            }

            return String.Empty;
        }
        // POST: Books/Create
        [HttpPost]
        public async Task<Book> Create(Book book)
        {
            book.Id = 0;
            _context.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // POST: Books/Edit/
        [HttpPost]
        public async Task<Book> Edit(Book book)
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // Delete: Books/Delete/5
        [HttpDelete]
        public async Task Delete([System.Web.Http.FromUri] int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return;
        }

        // Delete: Books/Delete/5
        [HttpDelete]
        public async Task<bool> DeleteConfirmed([System.Web.Http.FromUri] int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
