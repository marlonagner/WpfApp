using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.ViewModels.WpfApp.ViewModels;

namespace WpfApp.ViewModels
{
    public class PessoasViewModel : BaseViewModel
    {
        private readonly PessoaService _service = new PessoaService();

        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();

        private string _debugStatus;
        public string DebugStatus
        {
            get => _debugStatus;
            set { _debugStatus = value; OnPropertyChanged(); }
        }

        public string CaminhoPessoasJson => Paths.PessoasJson;

        private Pessoa _selecionado;
        public Pessoa Selecionado
        {
            get => _selecionado;
            set
            {
                _selecionado = value;
                OnPropertyChanged();

                if (_selecionado != null)
                {
                    Id = _selecionado.Id;
                    Nome = _selecionado.Nome;
                    Cpf = _selecionado.Cpf;
                    Endereco = _selecionado.Endereco;
                }

                AtualizarCanExecute();
            }
        }

        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        private string _nome;
        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        private string _cpf;
        public string Cpf
        {
            get => _cpf;
            set { _cpf = value; OnPropertyChanged(); }
        }

        private string _endereco;
        public string Endereco
        {
            get => _endereco;
            set { _endereco = value; OnPropertyChanged(); }
        }

        // filtro simples por nome/cpf
        private string _filtro;
        public string Filtro
        {
            get => _filtro;
            set { _filtro = value; OnPropertyChanged(); }
        }

        public RelayCommand BuscarCommand { get; }
        public RelayCommand LimparCommand { get; }
        public RelayCommand NovoCommand { get; }
        public RelayCommand SalvarCommand { get; }
        public RelayCommand ExcluirCommand { get; }

        public PessoasViewModel()
        {
            BuscarCommand = new RelayCommand(Buscar);
            LimparCommand = new RelayCommand(Limpar);
            NovoCommand = new RelayCommand(Novo);
            SalvarCommand = new RelayCommand(Salvar, PodeSalvar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExcluir);

            CarregarTudo();
        }

        private void CarregarTudo()
        {
            try
            {
                Pessoas.Clear();
                foreach (var p in _service.GetAll())
                    Pessoas.Add(p);

                Selecionado = null;
                LimparEditor();

                DebugStatus = "Carregou " + Pessoas.Count + " pessoa(s). Arquivo: " + CaminhoPessoasJson;
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao carregar: " + ex.Message + " | " + CaminhoPessoasJson;
            }
        }

        private void Buscar()
        {
            try
            {
                var termo = (Filtro ?? "").Trim().ToLower();

                var lista = _service.GetAll()
                    .Where(p =>
                        (p.Nome ?? "").ToLower().Contains(termo) ||
                        (p.Cpf ?? "").ToLower().Contains(termo))
                    .ToList();

                Pessoas.Clear();
                foreach (var p in lista)
                    Pessoas.Add(p);

                DebugStatus = "Busca retornou " + Pessoas.Count + " pessoa(s).";
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO na busca: " + ex.Message;
            }
        }

        private void Limpar()
        {
            Filtro = "";
            CarregarTudo();
        }

        private void Novo()
        {
            Selecionado = null;
            LimparEditor();
            DebugStatus = "Modo novo cadastro.";
        }

        private void Salvar()
        {
            try
            {
                var nome = (Nome ?? "").Trim();
                var cpf = (Cpf ?? "").Trim();
                var endereco = (Endereco ?? "").Trim();

                if (Id <= 0)
                {
                    _service.Add(new Pessoa { Nome = nome, Cpf = cpf, Endereco = endereco });
                    DebugStatus = "Pessoa incluída!";
                }
                else
                {
                    _service.Update(new Pessoa { Id = Id, Nome = nome, Cpf = cpf, Endereco = endereco });
                    DebugStatus = "Pessoa atualizada!";
                }

                CarregarTudo();
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao salvar: " + ex.Message;
            }
        }

        private void Excluir()
        {
            try
            {
                if (Selecionado == null) return;

                _service.Delete(Selecionado.Id);
                DebugStatus = "Pessoa excluída!";
                CarregarTudo();
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao excluir: " + ex.Message;
            }
        }

        private bool PodeSalvar()
        {
            return !string.IsNullOrWhiteSpace(Nome);
        }

        private bool PodeExcluir()
        {
            return Selecionado != null && Selecionado.Id > 0;
        }

        private void AtualizarCanExecute()
        {
            SalvarCommand.RaiseCanExecuteChanged();
            ExcluirCommand.RaiseCanExecuteChanged();
        }

        private void LimparEditor()
        {
            Id = 0;
            Nome = "";
            Cpf = "";
            Endereco = "";
        }
    }
}