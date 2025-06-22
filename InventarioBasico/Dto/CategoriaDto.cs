namespace InventarioBasico.Dto
{
    public class CategoriaCreateDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
    public class CategoriaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
