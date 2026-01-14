using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        private int GetUserId()
        {
            return (int)HttpContext.Items["UserId"];
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var books = await _bookRepository.GetAllAsync(userId);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var book = await _bookRepository.GetByIdAsync(id, userId);
            
            if (book == null)
                return NotFound("Book not found");

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookRequest request)
        {
            var userId = GetUserId();
            
            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                PublishedYear = request.PublishedYear,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var bookId = await _bookRepository.CreateAsync(book);
            book.Id = bookId;

            return CreatedAtAction(nameof(GetById), new { id = bookId }, book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _bookRepository.DeleteAsync(id, userId);
            
            if (!deleted)
                return NotFound("Book not found");

            return Ok(new { message = "Book deleted successfully" });
        }
    }
}
