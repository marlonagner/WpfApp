using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfApp.Models;
using WpfApp.Models.Enums;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class PedidosViewModel : BaseViewModel
    {
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly PessoaService _pessoaService = new PessoaService();
        private readonly ProdutoService _produtoService = new ProdutoService();

        // ====== LISTAS (COMBOS) ======
        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public List<FormaPagamento> FormasPagamento { get; } =
            Enum.GetValues(typeof(FormaPagamento)).Cast<FormaPagamento>().ToList();

        // ====== PEDIDOS SALVOS (GRID DE CIMA) ======
        public ObservableCollection<Pedido> PedidosSalvos { get; } = new ObservableCollection<Pedido>();

        private string _filtroPedidos;
        public string FiltroPedidos
        {
            get => _filtroPedidos;
            set { _filtroPedidos = value; OnPropertyChanged(); }
        }

        private Pedido _pedidoSelecionadoSalvo;
        public Pedido PedidoSelecionadoSalvo
        {
            get => _pedidoSelecionadoSalvo;
            set
            {
                _pedidoSelecionadoSalvo = value;
                OnPropertyChanged();

                if (_pedidoSelecionadoSalvo != null)
                    CarregarPedidoSalvoNoEditor(_pedidoSelecionadoSalvo);

                ExcluirPedidoCommand?.RaiseCanExecuteChanged();
            }
        }

        // ====== PEDIDO ATUAL (EDITOR) ======
        private Pedido _pedidoAtual = new Pedido();
        public Pedido PedidoAtual
        {
            get => _pedidoAtual;
            set { _pedidoAtual = value; OnPropertyChanged(); }
        }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                _pessoaSelecionada = value;
                OnPropertyChanged();
            }
        }

        private FormaPagamento _formaPagamento;
        public FormaPagamento FormaPagamento
        {
            get => _formaPagamento;
            set
            {
                _formaPagamento = value;
                OnPropertyChanged();
            }
        }

        // ====== ITENS DO PEDIDO (GRID DE BAIXO) ======
        public ObservableCollection<PedidoItem> Itens { get; } = new ObservableCollection<PedidoItem>();

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(); }
        }

        private int _quantidade = 1;
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                _quantidade = value < 1 ? 1 : value;
                OnPropertyChanged();
            }
        }

        private PedidoItem _itemSelecionado;
        public PedidoItem ItemSelecionado
        {
            get => _itemSelecionado;
            set { _itemSelecionado = value; OnPropertyChanged(); }
        }

        // ====== STATUS ======
        private string _debugStatus;
        public string DebugStatus
        {
            get => _debugStatus;
            set { _debugStatus = value; OnPropertyChanged(); }
        }

        public decimal Total => Itens.Sum(i => i.Subtotal);

        // ====== COMMANDS ======
        public RelayCommand RecarregarCombosCommand { get; }
        public RelayCommand NovoPedidoCommand { get; }
        public RelayCommand FinalizarCommand { get; }

        public RelayCommand AdicionarItemCommand { get; }
        public RelayCommand RemoverItemCommand { get; }

        public RelayCommand AtualizarPedidosCommand { get; }
        public RelayCommand BuscarPedidosCommand { get; }
        public RelayCommand LimparBuscaPedidosCommand { get; }

        public RelayCommand ExcluirPedidoCommand { get; }

        public PedidosViewModel()
        {
            RecarregarCombosCommand = new RelayCommand(RecarregarCombos);
            NovoPedidoCommand = new RelayCommand(NovoPedido);
            FinalizarCommand = new RelayCommand(Finalizar);

            AdicionarItemCommand = new RelayCommand(AdicionarItem);
            RemoverItemCommand = new RelayCommand(RemoverItem);

            AtualizarPedidosCommand = new RelayCommand(AtualizarPedidos);
            BuscarPedidosCommand = new RelayCommand(BuscarPedidos);
            LimparBuscaPedidosCommand = new RelayCommand(LimparBuscaPedidos);

            ExcluirPedidoCommand = new RelayCommand(ExcluirPedido, PodeExcluirPedido);

            // carga inicial
            RecarregarCombos();
            AtualizarPedidos();
            NovoPedido();
        }

        // =========================
        // COMBOS
        // =========================
        private void RecarregarCombos()
        {
            Pessoas.Clear();
            foreach (var p in _pessoaService.GetAll())
                Pessoas.Add(p);

            Produtos.Clear();
            foreach (var pr in _produtoService.GetAll())
                Produtos.Add(pr);

            DebugStatus = $"Pessoas: {Pessoas.Count} | Produtos: {Produtos.Count}";
        }

        // =========================
        // PEDIDOS SALVOS
        // =========================
        private void AtualizarPedidos()
        {
            var lista = _pedidoService.GetAll();

            PedidosSalvos.Clear();
            foreach (var p in lista)
                PedidosSalvos.Add(p);

            DebugStatus = $"Pedidos carregados: {PedidosSalvos.Count}";
        }

        private void BuscarPedidos()
        {
            var termo = (FiltroPedidos ?? "").Trim();

            var todos = _pedidoService.GetAll();

            if (string.IsNullOrWhiteSpace(termo))
            {
                PedidosSalvos.Clear();
                foreach (var p in todos) PedidosSalvos.Add(p);
                DebugStatus = $"Pedidos: {PedidosSalvos.Count}";
                return;
            }

            // filtra por Id ou por Nome da pessoa
            var filtrado = todos.Where(p =>
                    p.Id.ToString().Contains(termo) ||
                    (p.Pessoa != null && (p.Pessoa.Nome ?? "").ToUpper().Contains(termo.ToUpper()))
                )
                .ToList();

            PedidosSalvos.Clear();
            foreach (var p in filtrado) PedidosSalvos.Add(p);

            DebugStatus = $"Filtro aplicado: {PedidosSalvos.Count} pedido(s).";
        }

        private void LimparBuscaPedidos()
        {
            FiltroPedidos = "";
            AtualizarPedidos();
        }

        // =========================
        // EXCLUIR PEDIDO SALVO
        // =========================
        private bool PodeExcluirPedido()
        {
            return PedidoSelecionadoSalvo != null;
        }

        private void ExcluirPedido()
        {
            if (PedidoSelecionadoSalvo == null) return;

            var id = PedidoSelecionadoSalvo.Id;

            var resp = MessageBox.Show(
                $"Excluir o pedido #{id}?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resp != MessageBoxResult.Yes)
                return;

            try
            {
                _pedidoService.Delete(id);
                DebugStatus = $"Pedido #{id} excluído!";
                PedidoSelecionadoSalvo = null;
                AtualizarPedidos();
                NovoPedido();
            }
            catch (Exception ex)
            {
                DebugStatus = "Erro ao excluir: " + ex.Message;
            }
        }

        // =========================
        // NOVO PEDIDO / CARREGAR SALVO
        // =========================
        private void NovoPedido()
        {
            PedidoAtual = new Pedido
            {
                DataVenda = DateTime.Now,
                Status = StatusPedido.Pendente,
                Itens = new List<PedidoItem>()
            };

            Itens.Clear();
            PessoaSelecionada = null;
            FormaPagamento = FormasPagamento.FirstOrDefault();

            Quantidade = 1;
            ProdutoSelecionado = null;
            ItemSelecionado = null;

            OnPropertyChanged(nameof(Total));
            DebugStatus = "Novo pedido iniciado.";
        }

        private void CarregarPedidoSalvoNoEditor(Pedido pedido)
        {
            if (pedido == null) return;

            // clona dados para o editor (e bloqueia se finalizado)
            PedidoAtual = pedido;

            PessoaSelecionada = pedido.Pessoa;
            FormaPagamento = pedido.FormaPagamento;

            Itens.Clear();
            if (pedido.Itens != null)
            {
                foreach (var it in pedido.Itens)
                    Itens.Add(it);
            }

            Quantidade = 1;
            ProdutoSelecionado = null;
            ItemSelecionado = null;

            OnPropertyChanged(nameof(Total));
            DebugStatus = $"Pedido #{pedido.Id} carregado ({pedido.Status}).";
        }

        // =========================
        // ITENS
        // =========================
        private void AdicionarItem()
        {
            if (PedidoAtual == null) return;

            // bloqueio se finalizado
            if (PedidoAtual.Status != StatusPedido.Pendente)
            {
                DebugStatus = "Pedido finalizado: edição bloqueada.";
                return;
            }

            if (ProdutoSelecionado == null)
            {
                DebugStatus = "Selecione um produto.";
                return;
            }

            if (Quantidade <= 0) Quantidade = 1;

            // Se já existe produto no pedido: soma quantidade
            var existente = Itens.FirstOrDefault(i => i.Produto != null && i.Produto.Id == ProdutoSelecionado.Id);
            if (existente != null)
            {
                existente.Quantidade += Quantidade;

                // forçar refresh do DataGrid (PedidoItem.Subtotal é calculado)
                var idx = Itens.IndexOf(existente);
                Itens.RemoveAt(idx);
                Itens.Insert(idx, existente);
            }
            else
            {
                Itens.Add(new PedidoItem
                {
                    Produto = ProdutoSelecionado,
                    Quantidade = Quantidade
                });
            }

            Quantidade = 1;
            ProdutoSelecionado = null;

            OnPropertyChanged(nameof(Total));
            DebugStatus = "Item adicionado.";
        }

        private void RemoverItem()
        {
            if (PedidoAtual == null) return;

            if (PedidoAtual.Status != StatusPedido.Pendente)
            {
                DebugStatus = "Pedido finalizado: edição bloqueada.";
                return;
            }

            if (ItemSelecionado == null) return;

            Itens.Remove(ItemSelecionado);
            ItemSelecionado = null;

            OnPropertyChanged(nameof(Total));
            DebugStatus = "Item removido.";
        }

        // =========================
        // FINALIZAR
        // =========================
        private void Finalizar()
        {
            if (PedidoAtual == null) return;

            if (PessoaSelecionada == null)
            {
                DebugStatus = "Selecione uma pessoa.";
                return;
            }

            if (Itens.Count == 0)
            {
                DebugStatus = "Adicione ao menos 1 item.";
                return;
            }

            try
            {
                PedidoAtual.Pessoa = PessoaSelecionada;
                PedidoAtual.FormaPagamento = FormaPagamento;
                PedidoAtual.DataVenda = DateTime.Now;

                // manda itens do ObservableCollection para a lista do model
                PedidoAtual.Itens = Itens.ToList();

                // finaliza e bloqueia
                PedidoAtual.Status = StatusPedido.Pago;

                // se Id = 0 => novo, senão tenta update (mas update bloqueia se não for pendente)
                if (PedidoAtual.Id <= 0)
                    _pedidoService.Add(PedidoAtual);
                else
                    _pedidoService.Update(PedidoAtual);

                DebugStatus = $"Pedido #{PedidoAtual.Id} finalizado!";
                AtualizarPedidos();
            }
            catch (Exception ex)
            {
                DebugStatus = "Erro ao finalizar: " + ex.Message;
            }
        }
    }
}