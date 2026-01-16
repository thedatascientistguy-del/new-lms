using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        private int GetUserId()
        {
            return (int)HttpContext.Items["UserId"];
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching all books for user {UserId}", userId);
            
            var books = await _bookRepository.GetAllAsync(userId);
            
            _logger.LogInformation("Retrieved {Count} books for user {UserId}", books.Count(), userId);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching book {BookId} for user {UserId}", id, userId);
            
            var book = await _bookRepository.GetByIdAsync(id, userId);
            
            if (book == null)
            {
                _logger.LogWarning("Book {BookId} not found for user {UserId}", id, userId);
                return NotFound("Book not found");
            }

            _logger.LogInformation("Book {BookId} retrieved successfully for user {UserId}", id, userId);
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookRequest request)
        {
            var userId = GetUserId();
            _logger.LogInformation("Creating book '{Title}' by {Author} for user {UserId}", request.Title, request.Author, userId);
            
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

            _logger.LogInformation("Book {BookId} created successfully for user {UserId}", bookId, userId);
            return CreatedAtAction(nameof(GetById), new { id = bookId }, book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            _logger.LogInformation("Attempting to delete book {BookId} for user {UserId}", id, userId);
            
            var deleted = await _bookRepository.DeleteAsync(id, userId);
            
            if (!deleted)
            {
                _logger.LogWarning("Book {BookId} not found or unauthorized for user {UserId}", id, userId);
                return NotFound("Book not found");
            }

            _logger.LogInformation("Book {BookId} deleted successfully for user {UserId}", id, userId);
            return Ok(new { message = "Book deleted successfully" });
        }
    }
}
