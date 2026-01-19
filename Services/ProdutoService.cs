using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class ProdutoService
    {
        private readonly JsonRepository<Produto> _repo;

        public ProdutoService()
        {
            _repo = new JsonRepository<Produto>(Paths.ProdutosJson);
        }
        // Adiciona  um produto novo e faz validaçao de exceções
        public Produto Add(Produto novo)
        {
            if (novo == null) throw new ArgumentNullException(nameof(novo));

            var produtos = _repo.Load();

            // LINQ: Max pra gerar próximo ID automatico
            var nextId = produtos.Any() ? produtos.Max(p => p.Id) + 1 : 1;

            novo.Id = nextId;
            produtos.Add(novo);

            _repo.Save(produtos);
            return novo;
        }

        // Deleta o produto caso ele já exista persistido no banco de dados 
        public void Delete(int id)
        {
            var produtos = _repo.Load();

            // LINQ: Remove todos com aquele Id unico que o produto possui.
            produtos = produtos.Where(p => p.Id != id).ToList();

            _repo.Save(produtos);
        }

        // Faz um filtro na lista de produtos e valiações no nome, codigo, valor minimo e maximo.
        public List<Produto> Filter(string nome, string codigo, decimal? valorMin, decimal? valorMax)
        {
            var query = _repo.Load().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var n = nome.Trim().ToLower();
                query = query.Where(p => (p.Nome ?? "").ToLower().Contains(n));
            }

            if (!string.IsNullOrWhiteSpace(codigo))
            {
                var c = codigo.Trim().ToLower();
                query = query.Where(p => (p.Codigo ?? "").ToLower().Contains(c));
            }

            if (valorMin.HasValue)
                query = query.Where(p => p.Valor >= valorMin.Value);

            if (valorMax.HasValue)
                query = query.Where(p => p.Valor <= valorMax.Value);

            return query.OrderBy(p => p.Nome).ToList();
        }

        //Pega toda a lista de produtos no repo e ordena 
        public List<Produto> GetAll()
        {
            return _repo.Load()
                        .OrderBy(p => p.Nome)
                        .ToList();
        }
         // Atualiza os produtos validando se ele já existe ou não
        public void Update(Produto atualizado)
        {
            if (atualizado == null) throw new ArgumentNullException(nameof(atualizado));

            var produtos = _repo.Load();

            var existente = produtos.FirstOrDefault(p => p.Id == atualizado.Id);
            if (existente == null)
                throw new InvalidOperationException("Produto não encontrado.");

            existente.Nome = atualizado.Nome;
            existente.Codigo = atualizado.Codigo;
            existente.Valor = atualizado.Valor;

            _repo.Save(produtos);
        }
    }
}
