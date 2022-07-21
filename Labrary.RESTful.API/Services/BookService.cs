namespace Labrary.RESTful.API.Services
{
    public class BookService : IBookService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public BookService(DataContext context, IMapper mapper)
        {
           _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BookDto>> GetAll()
        {
            var dbEntities = await _context?.Books?.ToListAsync()!;
            var dtos = _mapper.Map<IEnumerable<BookDto>>(dbEntities);
            return dtos;
        }
    }
}
