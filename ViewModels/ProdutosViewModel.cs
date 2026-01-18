using System;
using System.Collections.ObjectModel;
using System.Globalization;
using WpfApp.Models;
using WpfApp.Services;
using WpfApp.ViewModels.WpfApp.ViewModels;

namespace WpfApp.ViewModels
{
    public class ProdutosViewModel : BaseViewModel
    {
        private readonly ProdutoService _service = new ProdutoService();

        // Lista que alimenta o DataGrid
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();

        // Debug visível na tela (pra saber se salvou, quantos itens, e qual arquivo está usando)
        private string _debugStatus;
        public string DebugStatus
        {
            get => _debugStatus;
            set { _debugStatus = value; OnPropertyChanged(); }
        }

        // Caminho real do JSON que o app está usando (via Paths)
        public string CaminhoProdutosJson => Paths.ProdutosJson;

        // Produto selecionado no DataGrid
        private Produto _selecionado;
        public Produto Selecionado
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
                    Codigo = _selecionado.Codigo;
                    ValorTexto = _selecionado.Valor.ToString(CultureInfo.CurrentCulture);
                }

                AtualizarCanExecute();
            }
        }

        // ====== Campos do Editor ======
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

        private string _codigo;
        public string Codigo
        {
            get => _codigo;
            set { _codigo = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        // Texto pra não sofrer com vírgula/ponto
        private string _valorTexto;
        public string ValorTexto
        {
            get => _valorTexto;
            set { _valorTexto = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        // ====== Filtros ======
        private string _filtroNome;
        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); }
        }

        private string _filtroCodigo;
        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set { _filtroCodigo = value; OnPropertyChanged(); }
        }

        private string _filtroMin;
        public string FiltroMin
        {
            get => _filtroMin;
            set { _filtroMin = value; OnPropertyChanged(); }
        }

        private string _filtroMax;
        public string FiltroMax
        {
            get => _filtroMax;
            set { _filtroMax = value; OnPropertyChanged(); }
        }

        // ====== Commands ======
        public RelayCommand BuscarCommand { get; }
        public RelayCommand LimparFiltrosCommand { get; }
        public RelayCommand IncluirCommand { get; }
        public RelayCommand SalvarCommand { get; }
        public RelayCommand ExcluirCommand { get; }

        public ProdutosViewModel()
        {
            BuscarCommand = new RelayCommand(Buscar);
            LimparFiltrosCommand = new RelayCommand(LimparFiltros);
            IncluirCommand = new RelayCommand(Incluir);
            SalvarCommand = new RelayCommand(Salvar, PodeSalvar);
            ExcluirCommand = new RelayCommand(Excluir, PodeExcluir);

            DebugStatus = "VM OK. Arquivo: " + CaminhoProdutosJson;

            CarregarTudo();
        }
        //Método de carregar produtos
        private void CarregarTudo()
        {
            try
            {
                Produtos.Clear();

                var lista = _service.GetAll();
                foreach (var p in lista)
                    Produtos.Add(p);

                Selecionado = null;
                LimparEditor();

                DebugStatus = "Carregou " + Produtos.Count + " item(ns). Arquivo: " + CaminhoProdutosJson;
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao carregar: " + ex.Message + " | Arquivo: " + CaminhoProdutosJson;
            }
        }
        
        
        // Metodo de buscar produto
        private void Buscar()
        {
            try
            {
                var min = TryParseDecimal(FiltroMin);
                var max = TryParseDecimal(FiltroMax);

                var lista = _service.Filter(FiltroNome, FiltroCodigo, min, max);

                Produtos.Clear();
                foreach (var p in lista)
                    Produtos.Add(p);

                Selecionado = null;
                LimparEditor();

                DebugStatus = "Busca retornou " + Produtos.Count + " item(ns). Arquivo: " + CaminhoProdutosJson;
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO na busca: " + ex.Message + " | Arquivo: " + CaminhoProdutosJson;
            }
        }
        //Método de limpar filtros
        private void LimparFiltros()
        {
            FiltroNome = "";
            FiltroCodigo = "";
            FiltroMin = "";
            FiltroMax = "";

            CarregarTudo();
        }
        //Método de incluir produto (Não é o de salvar)
        private void Incluir()
        {
            Selecionado = null;
            LimparEditor();
            DebugStatus = "Modo inclusão. Preencha e clique Salvar. Arquivo: " + CaminhoProdutosJson;
        }
         //Método para salvar produto
        private void Salvar()
        {
            try
            {
                var nome = (Nome ?? "").Trim();
                var codigo = (Codigo ?? "").Trim();

                var valorNullable = TryParseDecimal(ValorTexto);
                if (!valorNullable.HasValue)
                {
                    DebugStatus = "Valor inválido. Use 10,00 (vírgula).";
                    return;
                }

                var valor = valorNullable.Value;

                if (Id <= 0)
                {
                    _service.Add(new Produto
                    {
                        Nome = nome,
                        Codigo = codigo,
                        Valor = valor
                    });

                    DebugStatus = "Incluiu e salvou em: " + CaminhoProdutosJson;
                }
                else
                {
                    _service.Update(new Produto
                    {
                        Id = Id,
                        Nome = nome,
                        Codigo = codigo,
                        Valor = valor
                    });

                    DebugStatus = "Atualizou e salvou em: " + CaminhoProdutosJson;
                }

                CarregarTudo();
                DebugStatus += " | Itens no grid: " + Produtos.Count;
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao salvar: " + ex.Message + " | Arquivo: " + CaminhoProdutosJson;
            }
        }

        
         // Metodo de excluir produto
        private void Excluir()
        {
            try
            {
                if (Selecionado == null)
                {
                    DebugStatus = "Selecione um produto no grid para excluir.";
                    return;
                }

                _service.Delete(Selecionado.Id);
                DebugStatus = "Excluiu e salvou em: " + CaminhoProdutosJson;

                CarregarTudo();
                DebugStatus += " | Itens no grid: " + Produtos.Count;
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao excluir: " + ex.Message + " | Arquivo: " + CaminhoProdutosJson;
            }
        }
        // Método que analista e valida se pode salvar
        private bool PodeSalvar()
        {
            if (string.IsNullOrWhiteSpace(Nome)) return false;
            if (string.IsNullOrWhiteSpace(Codigo)) return false;

            var valor = TryParseDecimal(ValorTexto);
            return valor.HasValue;
        }
        //Método booleano confirma se pode excluir
        private bool PodeExcluir()
        {
            return Selecionado != null && Selecionado.Id > 0;
        }
        // Método que confirma se o atualizar pode ser executado
        private void AtualizarCanExecute()
        {
            // com RelayCommand padrão WPF, isso força reavaliar botões
            SalvarCommand.RaiseCanExecuteChanged();
            ExcluirCommand.RaiseCanExecuteChanged();
        }
        // Metodo para limpar editor
        private void LimparEditor()
        {
            Id = 0;
            Nome = "";
            Codigo = "";
            ValorTexto = "";
        }
        // Metodo que valida o tipo de entrada do valor do produto, se usou virgula ou ponto
        private decimal? TryParseDecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            texto = texto.Trim();

            // Se o usuário digitou ponto e NÃO digitou vírgula,
            // assume que o ponto é decimal (padrão "americano": 10.50)
            if (texto.Contains(".") && !texto.Contains(","))
            {
                if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var vInv))
                    return vInv;
            }

            // Caso normal brasileiro (10,50) e também "1.234,56"
            var ptbr = new CultureInfo("pt-BR");
            if (decimal.TryParse(texto, NumberStyles.Number, ptbr, out var vPt))
                return vPt;

            // Último fallback (casos raros)
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var v2))
                return v2;

            return null;
        }
    }
}