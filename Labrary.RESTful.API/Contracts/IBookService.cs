

namespace Labrary.RESTful.API.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAll();
    }
}
