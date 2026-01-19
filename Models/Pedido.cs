using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models.Enums;

namespace WpfApp.Models
{
    public class Pedido
    {
        public DateTime DataVenda { get; set; } = DateTime.Now;

        public FormaPagamento FormaPagamento { get; set; }

        public int Id { get; set; }  // auto-increment no Service

        // Atributo lista dos vários produtos com a quantidade
        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

        // relacionamento
        public Pessoa Pessoa { get; set; }

        // quantidade total de itens no pedido
        public int QtdItens => Itens?.Count ?? 0;

        // ============================
        // RESUMO DOS PRODUTOS (GRID)
        // Exemplo: "BURGUI x1; COCA x2"
        // ============================
        public string ResumoProdutos
        {
            get
            {
                if (Itens == null || Itens.Count == 0)
                    return "";

                return string.Join("; ",
                    Itens
                        .Where(i => i != null && i.Produto != null)
                        .GroupBy(i => i.Produto.Nome)
                        .Select(g => $"{g.Key} x{g.Sum(x => x.Quantidade)}")
                );
            }
        }

        public StatusPedido Status { get; set; } = StatusPedido.Pendente;

        // ============================
        // TOTAL DO PEDIDO FORMATADO
        // ============================
        public string TotalPedidoTexto => ValorTotal().ToString("C");

        // ============================
        // Métodos da classe:
        // ============================

        // Data Da Venda usando DateTime
        public DateTime DataDaVenda()
        {
            return DataVenda;
        }

        // Forma De Pagamento com Enum
        public FormaPagamento FormaDePgto()
        {
            return FormaPagamento;
        }

        // Soma total do pedido usando LINQ
        public decimal ValorTotal()
        {
            // Soma subtotal de cada item
            return Itens.Sum(i => i.Subtotal);
        }
    }
}
