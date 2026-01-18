using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models.Enums;


namespace WpfApp.Models
{
    public class Pedido
    {
        public int Id { get; set; }  // auto-increment no Service

        // relacionamento
        public Pessoa Pessoa { get; set; }
        public string TotalPedidoTexto => ValorTotal().ToString("C");

        
        public int QtdItens => Itens?.Count ?? 0;


        // Atributo lista dos vários produtos com a quantidade
        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

        public DateTime DataVenda { get; set; } = DateTime.Now;
        public FormaPagamento FormaPagamento { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        
        public string ResumoProdutos
        {
            get
            {
                // Dados que devem aparecer no grid como exemplo: "BURGUI x1; COCA x2"
                if (Itens == null || Itens.Count == 0) return "";

                return string.Join("; ",
                    Itens
                        .Where(i => i != null && i.Produto != null)
                        .Select(i => $"{i.Produto.Nome} x{i.Quantidade}")
                );
            }
        }

        // Métodos da classe:
        
        public decimal ValorTotal()
        {
            //Soma subtotal de cada item
            return Itens.Sum(i => i.Subtotal);
        }
        
        //Data Da Venda usando DateTime
        public DateTime DataDaVenda()
        {
            return DataVenda;
        }
        //Forma De Pagamento com Enum
        public FormaPagamento FormaDePgto()
        {
            return FormaPagamento;
        }
    }
}