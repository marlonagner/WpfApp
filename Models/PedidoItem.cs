namespace WpfApp.Models
{
    public class PedidoItem
    {
         // Atributos
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }

        // Faz o calculo do Subtotal do item
        public decimal Subtotal
        {
            get
            {
                var valor = Produto != null ? Produto.Valor : 0m;
                return valor * Quantidade;
            }
        }
    }
}