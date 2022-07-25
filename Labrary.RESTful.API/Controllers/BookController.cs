using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Labrary.RESTful.API.Controllers
{

    [ApiController]
    [Route("api/books")]
    [Produces("appliccation/json")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ISaveFiles _savefiles;
        private readonly string? container = "Books";
        public BookController(IBookService bookService, DataContext context, IMapper mapper, ISaveFiles savefiles)
        {
            _bookService = bookService;
            _context = context;
            _mapper = mapper;
            _savefiles = savefiles;
        }
        [HttpPost]
        [Route("/create")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post(BookCreateDto model)
        {
            using var _transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (model is null)
                {
                    ArgumentNullException.ThrowIfNull(model);
                    return NotFound();
                }

                var dbEntity = _mapper.Map<Book>(model);

                if (!string.IsNullOrWhiteSpace(model.Image))
                {
                    var Image = Convert.FromBase64String(model.Image);
                    dbEntity.Image = await _savefiles.GuardarArchivo(Image, "jpg", container!);
                }
                _context.Add(dbEntity);
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                var dto = _mapper.Map<BookDto>(dbEntity);
                return NoContent();
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
           
        }
        [HttpPut("update/{Id:int}")]
        [ProducesResponseType(202)]
        public async Task<ActionResult<BookDto>> Put(int Id, BookUpdateDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (Id is 0)
                    return NotFound();
                if (model is null)
                    return NotFound();

                var dbEntity = await _context?.Books?.SingleOrDefaultAsync(b => b.BookId == Id)!;

                if (dbEntity is null)
                    return NotFound();

                if (string.IsNullOrWhiteSpace(model.Image))
                {
                    var updateImage = Convert.FromBase64String(model.Image!);
                    dbEntity.Image = await _savefiles.EditarArchivo(updateImage, "jpg", container!,dbEntity.Image);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Accepted("GetBook", _mapper.Map<BookDto>(dbEntity));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> Delete(int Id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (Id == 0)
                    return NotFound("There is nothing with this Id");

                var exists = await _context.Books?.AnyAsync(b => b.BookId == Id)!;
                if (exists)
                { 
                 var dbEntity = await _context.Books.FirstOrDefaultAsync(b => b.BookId == Id);
                    if (dbEntity == null)
                        return NotFound();
                    _context.Books.Remove(dbEntity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return NoContent();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
