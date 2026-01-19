using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;
using WpfApp.Models.Enums;

namespace WpfApp.Services
{
    public class PedidoService
    {
        private readonly JsonRepository<Pedido> _repo;

        public PedidoService()
        {
            _repo = new JsonRepository<Pedido>(Paths.PedidosJson);
        }
        // Metodo para adicionar novo pedido, aplicando também as condicionais e filtros
        public Pedido Add(Pedido novo)
        {
            if (novo == null) throw new ArgumentNullException(nameof(novo));

            var pedidos = _repo.Load();
            var nextId = pedidos.Any() ? pedidos.Max(p => p.Id) + 1 : 1; // LINQ Max

            novo.Id = nextId;
            pedidos.Add(novo);

            _repo.Save(pedidos);
            return novo;
        }

        // Pega todos os pedidos da lista
        public List<Pedido> GetAll()
        {
            return _repo.Load()
                .OrderByDescending(p => p.DataVenda)
                .ToList();
        }
         // Metodo para atualizar o pedido e faz as validações e condicionais também
        public void Update(Pedido atualizado)
        {
            if (atualizado == null) throw new ArgumentNullException(nameof(atualizado));

            var pedidos = _repo.Load();
            var existente = pedidos.FirstOrDefault(p => p.Id == atualizado.Id);

            if (existente == null)
                throw new InvalidOperationException("Pedido não encontrado.");

            // BLOQUEIO: se já finalizou (Status != Pendente), não pode editar
            if (existente.Status != StatusPedido.Pendente)
                throw new InvalidOperationException("Pedido finalizado: edição bloqueada.");

            existente.Pessoa = atualizado.Pessoa;
            existente.Itens = atualizado.Itens;
            existente.DataVenda = atualizado.DataVenda;
            existente.FormaPagamento = atualizado.FormaPagamento;
            existente.Status = atualizado.Status;

            _repo.Save(pedidos);
        }
    }
}