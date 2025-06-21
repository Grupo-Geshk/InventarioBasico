namespace InventarioBasico.Dto
{
    public class ProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int StockActual { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? ImagenUrl { get; set; }
        public string CategoriaNombre { get; set; }
    }

}
