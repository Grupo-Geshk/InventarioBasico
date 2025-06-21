namespace InventarioBasico.Dto
{
    public class MovimientoCreateDto
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Notas { get; set; }
        public int TipoMovimiento { get; set; } // 1 = Ingreso, 2 = Salida, 3 = Ajuste
    }

}
