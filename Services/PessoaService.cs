using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class PessoaService
    {
        private readonly JsonRepository<Pessoa> _repo;

        public PessoaService()
        {
            _repo = new JsonRepository<Pessoa>(Paths.PessoasJson);
        }

        // Adiciona a pessoa  também fazendo validações
        public void Add(Pessoa pessoa)
        {
            var pessoas = _repo.Load();

            var nextId = pessoas.Count == 0 ? 1 : pessoas.Max(p => p.Id) + 1;
            pessoa.Id = nextId;

            pessoas.Add(pessoa);
            _repo.Save(pessoas);
        }

        // Exclui a pessoa caso ela exista no banco e dados e salva o estado
        public void Delete(int id)
        {
            var pessoas = _repo.Load();

            var existente = pessoas.FirstOrDefault(p => p.Id == id);
            if (existente == null) return;

            pessoas.Remove(existente);
            _repo.Save(pessoas);
        }

        //Pega a lista de pessoas 
        public List<Pessoa> GetAll()
        {
            return _repo.Load()
                .OrderBy(p => p.Nome)
                .ToList();
        }
         // Atualiza a pessoa, validando se a pessoa já existe ou não
        public void Update(Pessoa pessoa)
        {
            var pessoas = _repo.Load();

            var existente = pessoas.FirstOrDefault(p => p.Id == pessoa.Id);
            if (existente == null) return;

            existente.Nome = pessoa.Nome;
            existente.Cpf = pessoa.Cpf;
            existente.Endereco = pessoa.Endereco;

            _repo.Save(pessoas);
        }
    }
}