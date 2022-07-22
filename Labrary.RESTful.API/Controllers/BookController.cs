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
        private readonly  string? container = "Books";
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

                return CreatedAtRoute("GetBook", new { Id = dbEntity.BookId }, model);
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
