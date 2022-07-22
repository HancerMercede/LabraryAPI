using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Labrary.RESTful.API.Controllers
{
    [Route("api/books")]
    [ApiController]
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
        [ProducesResponseType(200)]
        public async Task<IActionResult> Post([FromForm] BookCreateDto model)
        {
            using var _transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (model is null)
                    ArgumentNullException.ThrowIfNull(model);


                var dbEntity = _mapper.Map<Book>(model);

                if (model.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray();
                        var extention = Path.GetExtension(model.Image.FileName);
                        dbEntity.Image = await _savefiles.GuardarArchivo(content, extention, container, model.Image.ContentType);
                    }
                }
                _context.Add(dbEntity);
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                var dto = _mapper.Map<BookDto>(dbEntity);
                return new CreatedAtRouteResult("GetBook", new { Id = dbEntity.BookId }, dto);
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
           
        }
        [HttpPut("update/{Id:int}")]
        [ProducesResponseType(202)]
        public async Task<ActionResult<BookDto>> Put(int Id, [FromForm]BookUpdateDto model)
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

                if (model.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray();
                        var extention = Path.GetExtension(model.Image.FileName);
                        dbEntity.Image = await _savefiles.EditarArchivo(content, extention, container, dbEntity.Image, model.Image.ContentType);
                    }
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
    }
}
