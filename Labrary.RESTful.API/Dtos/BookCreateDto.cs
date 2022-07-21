using Labrary.RESTful.API.Validations;

namespace Labrary.RESTful.API.Dtos
{
    public class BookCreateDto
    {
        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public string? BookName { get; set; }
        public string? Tematic { get; set; }
        public IFormFile? Image { get; set; }
    }
}
