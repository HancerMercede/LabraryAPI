

namespace Labrary.RESTful.API.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAll();
        Task<BookDto> GetById(int Id);
        Task<BookDto> Create(BookCreateDto model);
    }
}
