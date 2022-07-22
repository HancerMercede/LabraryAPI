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
            throw new NotImplementedException();
        }
    }
}
