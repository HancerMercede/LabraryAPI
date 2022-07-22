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


        public async Task Delete(int Id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (Id == 0)
                     return;

                var dbEntity = await _context.Books?.SingleOrDefaultAsync(b => b.BookId == Id)!;
                if(dbEntity is not null)
                {
                    _context.Remove(dbEntity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
