namespace Labrary.RESTful.API.Dtos
{
    public class BookCreateDto
    {
        public string BookName { get; set; }
        public string Tematic { get; set; }
        public byte[] Image { get; set; }
    }
}
