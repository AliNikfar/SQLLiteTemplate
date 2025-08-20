using DigitalLibrary.Data;
using DigitalLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CRUD : Controller
    {
        private readonly AppDBContext _db;

        public CRUD(AppDBContext db)
        {

            _db = db;
        }
        [HttpPost]
        public async Task<ActionResult<List<Book>>> AddBooks(Book book)
        {
            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();
            return Ok(_db.Books.AsNoTracking().ToListAsync()); 
        }
        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            return Ok(await _db.Books.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null)
                return BadRequest("not found");
            else
                return Ok(book);
        }
        [HttpPost("{id}")]
        public async Task<ActionResult<Book>> Remove(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null)
                return BadRequest("not found");
            else
            {
                _db.Remove(book);
                await _db.SaveChangesAsync();
                return Ok(await _db.Books.AsNoTracking().ToListAsync());
            }

        }

        [HttpPost]
        public async Task<ActionResult<Book>> Update(Book book)
        {
            var isexiest = await _db.Books.AsNoTracking().Where(p=>p.Id == book.Id).FirstOrDefaultAsync();
            if (isexiest is null)
                return BadRequest("not found");
            else
            {
                _db.Update(book);
                await _db.SaveChangesAsync();
                return Ok(await _db.Books.AsNoTracking().ToListAsync());
            }

        }

    }
}
