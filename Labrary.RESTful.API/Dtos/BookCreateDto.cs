using Labrary.RESTful.API.Validations;

namespace Labrary.RESTful.API.Dtos
{
    public class BookCreateDto
    {
        
       
        public string? BookName { get; set; }
        public string? Tematic { get; set; }
        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public string? Image { get; set; }
    }
}
