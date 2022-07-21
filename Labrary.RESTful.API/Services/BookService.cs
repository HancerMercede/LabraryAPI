namespace Labrary.RESTful.API.Services
{
    public class BookService : IBookService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ISaveFiles _savefiles;
        private readonly string container= "BooksImages";
        public BookService(DataContext context, IMapper mapper, ISaveFiles savefiles)
        {
           _context = context;
            _mapper = mapper;
            _savefiles = savefiles;
        }
        public async Task<IEnumerable<BookDto>> GetAll()
        {
            var dbEntities = await _context?.Books?.ToListAsync()!;
            var dtos = _mapper.Map<IEnumerable<BookDto>>(dbEntities);
            return dtos;
        }

        public async Task<BookDto> GetById(int Id)
        {
            if (Id == 0)
                ArgumentNullException.ThrowIfNull(typeof(BookDto));

            var dbEntity = await _context.Books?.SingleOrDefaultAsync(b => b.BookId == Id)!;

            if(dbEntity is null)
                ArgumentNullException.ThrowIfNull(typeof(BookDto));

            var dto = _mapper.Map<BookDto>(dbEntity);
            return dto;
        }

        public async Task<BookDto> Create(BookCreateDto model)
        {
            using var _transaction =await  _context.Database.BeginTransactionAsync();
            try
            {
                if (model is null)
                    ArgumentNullException.ThrowIfNull(model);


                var dbEntity = _mapper.Map<Book>(model);

                if (model.Image is not null)
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

               return _mapper.Map<BookDto>(dbEntity);
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
