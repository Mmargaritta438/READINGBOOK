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
        [HttpGet("GetBooks")]
        public async Task<List<Book>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: Books/Details/5
        [HttpGet("GetBooks")]
        public async Task<string> Details(int id)
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
        [HttpPost("PostBooks")]
        public async Task<Book> Create(Book book)
        {
            book.Id = 0;
            _context.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // POST: Books/Edit/
        [HttpPost("PostBooks")]
        public async Task<Book> Edit(Book book)
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        // Delete: Books/Delete/5
        [HttpDelete("DeleteBooks")]
        public async Task Delete(int id)
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
        [HttpDelete("DeleteBooks")]
        public async Task<bool> DeleteConfirmed(int id)
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
