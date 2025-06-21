namespace InventarioBasico.Dto
{
    public class ProductoCreateDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int StockActual { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? ImagenUrl { get; set; }
        public Guid CategoriaId { get; set; }
    }
}
