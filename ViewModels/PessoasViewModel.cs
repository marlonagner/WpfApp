using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.ViewModels.WpfApp.ViewModels;

namespace WpfApp.ViewModels
{
    //Extende BaseViewModel para herdar a implementação de INotifyPropertyChanged
    public class PessoasViewModel : BaseViewModel
    {
        private readonly PessoaService _service = new PessoaService();

        private string _cpf;
        private string _debugStatus;
        private string _endereco;

        // filtro simples por nome/cpf
        private string _filtro;

        private int _id;
        private string _nome;
        private Pessoa _selecionado;

        //Construtor
        public PessoasViewModel()
        {
            BuscarCommand = new RelayCommand(Buscar);
            LimparCommand = new RelayCommand(Limpar);
            NovoCommand = new RelayCommand(Novo);
            SalvarCommand = new RelayCommand(Salvar, PodeSalvar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExcluir);

            CarregarTudo();
        }

        public RelayCommand BuscarCommand { get; }
        public string CaminhoPessoasJson => Paths.PessoasJson;
        public string Cpf
        {
            get => _cpf;
            set { _cpf = value; OnPropertyChanged(); }
        }

        public string DebugStatus
        {
            get => _debugStatus;
            set { _debugStatus = value; OnPropertyChanged(); }
        }

        public string Endereco
        {
            get => _endereco;
            set { _endereco = value; OnPropertyChanged(); }
        }

        public RelayCommand ExcluirCommand { get; }
        public string Filtro
        {
            get => _filtro;
            set { _filtro = value; OnPropertyChanged(); }
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        public RelayCommand LimparCommand { get; }
        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        public RelayCommand NovoCommand { get; }
        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public RelayCommand SalvarCommand { get; }

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
        //Atualiza o estado dos comandos que dependem de condições
        private void AtualizarCanExecute()
        {
            SalvarCommand.RaiseCanExecuteChanged();
            ExcluirCommand.RaiseCanExecuteChanged();
        }
        //Busca registros pelo filtro
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
        //Carrega todos os registros
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
        //Exclui o registro selecionado
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

        //Limpa o filtro e recarrega todos os registros
        private void Limpar()
        {
            Filtro = "";
            CarregarTudo();
        }

        //Limpa os campos do editor
        private void LimparEditor()
        {
            Id = 0;
            Nome = "";
            Cpf = "";
            Endereco = "";
        }
        //Limpa o editor para um novo cadastro
        private void Novo()
        {
            Selecionado = null;
            LimparEditor();
            DebugStatus = "Modo novo cadastro.";
        }
        //Valida se pode excluir
        private bool PodeExcluir()
        {
            return Selecionado != null && Selecionado.Id > 0;
        }
        //Valida se pode salvar
        private bool PodeSalvar()
        {
            return !string.IsNullOrWhiteSpace(Nome);
        }
        //Salva o registro (novo ou atualização)
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
    }
}