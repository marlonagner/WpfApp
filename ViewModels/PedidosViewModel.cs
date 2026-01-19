using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WpfApp.Models;
using WpfApp.Models.Enums;
using WpfApp.Services;
using WpfApp.ViewModels.WpfApp.ViewModels;

namespace WpfApp.ViewModels
{
    // A classe herda de BaseViewModel
    public class PedidosViewModel : BaseViewModel
    {
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly PessoaService _pessoaService = new PessoaService();
        private readonly ProdutoService _produtoService = new ProdutoService();
        private bool _bloqueado;

        // Cache completo dos pedidos pra filtrar
        private List<Pedido> _cachePedidos = new List<Pedido>();

        private string _debugStatus;

        // Filtro buscar pedidos
        private string _filtroPedidos;

        private FormaPagamento _formaPagamento;

        private PedidoItem _itemSelecionado;

        // Pedido atual (em edição/visualização)
        private Pedido _pedidoAtual = new Pedido();

        private Pedido _pedidoSelecionado;

        // Pedido selecionado (grid de pedidos salvos)
        private Pedido _pedidoSelecionadoSalvo;

        // Campos do cabeçalho do pedido (bindings dos combos)
        private Pessoa _pessoaSelecionada;

        // Adição de item
        private Produto _produtoSelecionado;

        private int _quantidade = 1;

        public PedidosViewModel()
        {
            RecarregarCombosCommand = new RelayCommand(CarregarCombos);
            AtualizarPedidosCommand = new RelayCommand(CarregarPedidosSalvos);

            BuscarPedidosCommand = new RelayCommand(AplicarFiltroPedidos);
            LimparBuscaPedidosCommand = new RelayCommand(LimparFiltroPedidos);

            NovoPedidoCommand = new RelayCommand(NovoPedido);
            AdicionarItemCommand = new RelayCommand(AdicionarItem, PodeAdicionarItem);
            RemoverItemCommand = new RelayCommand(RemoverItem, PodeRemoverItem);
            FinalizarCommand = new RelayCommand(Finalizar, PodeFinalizar);
            ExcluirPedidoCommand = new RelayCommand(ExcluirPedido, PodeExcluirPedido);

            Itens.CollectionChanged += (s, e) =>
            {
                // Mantém o PedidoAtual sincronizado com o grid
                PedidoAtual.Itens = Itens.ToList();
                OnPropertyChanged(nameof(Total));
                AtualizarCanExecute();
            };

            CarregarCombos();
            CarregarPedidosSalvos();
            NovoPedido();
        }

        public RelayCommand AdicionarItemCommand { get; }

        public RelayCommand AtualizarPedidosCommand { get; }

        public bool Bloqueado
        {
            get => _bloqueado;
            set { _bloqueado = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        public RelayCommand BuscarPedidosCommand { get; }

        public string DebugStatus
        {
            get => _debugStatus;
            set { _debugStatus = value; OnPropertyChanged(); }
        }

        public RelayCommand ExcluirPedidoCommand { get; }

        public string FiltroPedidos
        {
            get => _filtroPedidos;
            set { _filtroPedidos = value; OnPropertyChanged(); }
        }

        public RelayCommand FinalizarCommand { get; }

        public FormaPagamento FormaPagamento
        {
            get => _formaPagamento;
            set { _formaPagamento = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        public Array FormasPagamento => Enum.GetValues(typeof(FormaPagamento));

        public PedidoItem ItemSelecionado
        {
            get => _itemSelecionado;
            set { _itemSelecionado = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        // Itens do pedido atual (mostra no grid de itens)
        public ObservableCollection<PedidoItem> Itens { get; } = new ObservableCollection<PedidoItem>();

        public RelayCommand LimparBuscaPedidosCommand { get; }

        public RelayCommand NovoPedidoCommand { get; }

        public Pedido PedidoAtual
        {
            get => _pedidoAtual;
            set
            {
                _pedidoAtual = value ?? new Pedido();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        //Excluir pedido selecionado
        public Pedido PedidoSelecionado
        {
            get => _pedidoSelecionado;
            set
            {
                _pedidoSelecionado = value;
                OnPropertyChanged();
                ExcluirPedidoCommand?.RaiseCanExecuteChanged();
            }
        }

        public Pedido PedidoSelecionadoSalvo
        {
            get => _pedidoSelecionadoSalvo;
            set
            {
                _pedidoSelecionadoSalvo = value;
                OnPropertyChanged();

                if (_pedidoSelecionadoSalvo != null)
                    CarregarPedidoSalvoNaTela(_pedidoSelecionadoSalvo);

                AtualizarCanExecute();
                ExcluirPedidoCommand?.RaiseCanExecuteChanged();
            }
        }

        // Pedidos salvos (mostra no grid de pedidos)
        public ObservableCollection<Pedido> PedidosSalvos { get; } = new ObservableCollection<Pedido>();

        // Combos
        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set { _pessoaSelecionada = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }

        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(); AtualizarCanExecute(); }
        }
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                _quantidade = value < 1 ? 1 : value;
                OnPropertyChanged();
                AtualizarCanExecute();
            }
        }
        // Commands
        public RelayCommand RecarregarCombosCommand { get; }

        public RelayCommand RemoverItemCommand { get; }

        // Total:  (Pedido.ValorTotal -> Itens.Sum)
        public decimal Total => PedidoAtual.ValorTotal();


        // ======== LOAD ========
        //Adicionar item ao pedido atual
        private void AdicionarItem()
        {
            if (ProdutoSelecionado == null) return;

            var existente = Itens.FirstOrDefault(i => i.Produto != null && i.Produto.Id == ProdutoSelecionado.Id);
            if (existente != null)
            {
                // Força refresh substituindo o item
                var index = Itens.IndexOf(existente);
                Itens[index] = new PedidoItem
                {
                    Produto = existente.Produto,
                    Quantidade = existente.Quantidade + Quantidade
                };
            }
            else
            {
                Itens.Add(new PedidoItem
                {
                    Produto = ProdutoSelecionado,
                    Quantidade = Quantidade
                });
            }

            ProdutoSelecionado = null;
            Quantidade = 1;

            PedidoAtual.Itens = Itens.ToList();
            OnPropertyChanged(nameof(Total));

            DebugStatus = "Item adicionado. Total: " + Total.ToString("C");
        }

        //Aplicar filtro na lista de pedidos salvos
        private void AplicarFiltroPedidos()
        {
            var termo = (FiltroPedidos ?? "").Trim();
            IEnumerable<Pedido> filtrados = _cachePedidos;

            if (!string.IsNullOrWhiteSpace(termo))
            {
                if (int.TryParse(termo, out var id))
                {
                    filtrados = filtrados.Where(p => p.Id == id);
                }
                else
                {
                    filtrados = filtrados.Where(p =>
                        p.Pessoa != null &&
                        !string.IsNullOrWhiteSpace(p.Pessoa.Nome) &&
                        p.Pessoa.Nome.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }

            PedidosSalvos.Clear();
            foreach (var p in filtrados)
                PedidosSalvos.Add(p);

            DebugStatus = $"Filtro aplicado: {PedidosSalvos.Count} pedido(s).";
        }

        //Atualiza o CanExecute de todos os comandos que dependem de estado
        private void AtualizarCanExecute()
        {
            AdicionarItemCommand.RaiseCanExecuteChanged();
            RemoverItemCommand.RaiseCanExecuteChanged();
            FinalizarCommand.RaiseCanExecuteChanged();
            ExcluirPedidoCommand.RaiseCanExecuteChanged();
        }

        //Carrega pessoas e produtos para os combos
        private void CarregarCombos()
        {
            Pessoas.Clear();
            foreach (var p in _pessoaService.GetAll())
                Pessoas.Add(p);

            Produtos.Clear();
            foreach (var pr in _produtoService.GetAll())
                Produtos.Add(pr);

            DebugStatus = $"Combos ok. Pessoas: {Pessoas.Count} | Produtos: {Produtos.Count}";
        }

        //Carrega um pedido salvo na tela para visualização/edição
        private void CarregarPedidoSalvoNaTela(Pedido pedido)
        {
            if (pedido == null) return;

            //Clia um clone simples
            var clone = new Pedido
            {
                Id = pedido.Id,
                Pessoa = pedido.Pessoa,
                DataVenda = pedido.DataVenda,
                FormaPagamento = pedido.FormaPagamento,
                Status = pedido.Status,
                Itens = (pedido.Itens ?? new List<PedidoItem>())
                    .Select(i => new PedidoItem
                    {
                        Produto = i.Produto,       // produto já vem completo do JSON que foi persistido
                        Quantidade = i.Quantidade
                    })
                    .ToList()
            };

            PedidoAtual = clone;

            // Pessoa no combo 
            if (clone.Pessoa != null)
                PessoaSelecionada = Pessoas.FirstOrDefault(p => p.Id == clone.Pessoa.Id) ?? clone.Pessoa;
            else
                PessoaSelecionada = null;

            FormaPagamento = clone.FormaPagamento;

            // Carrega itens no ObservableCollection (Grid)
            Itens.Clear();
            foreach (var it in clone.Itens)
                Itens.Add(it);

            // Bloqueia se já finalizado
            Bloqueado = clone.Status != StatusPedido.Pendente;

            ItemSelecionado = null;
            ProdutoSelecionado = null;
            Quantidade = 1;

            OnPropertyChanged(nameof(Total));

            DebugStatus = Bloqueado
                ? $"Pedido #{clone.Id} carregado (finalizado - bloqueado)."
                : $"Pedido #{clone.Id} carregado (pendente).";
        }

        //Carrega os pedidos salvos do serviço
        private void CarregarPedidosSalvos()
        {
            _cachePedidos = _pedidoService.GetAll();

            PedidosSalvos.Clear();
            foreach (var ped in _cachePedidos)
                PedidosSalvos.Add(ped);

            DebugStatus = $"Pedidos carregados: {PedidosSalvos.Count} | JSON: {Paths.PedidosJson}";
        }

        //Excluir pedido selecionado
        private void ExcluirPedido()
        {
            try
            {
                if (PedidoSelecionadoSalvo == null) return;

                var id = PedidoSelecionadoSalvo.Id;

                // remove da lista em tela
                var toRemoveTela = PedidosSalvos.FirstOrDefault(p => p.Id == id);
                if (toRemoveTela != null)
                    PedidosSalvos.Remove(toRemoveTela);

                // remove do cache (senão filtro/refresh traz de volta)
                _cachePedidos = _cachePedidos.Where(p => p.Id != id).ToList();

                // Fallback: sobrescreve o JSON diretamente (funciona mesmo sem Delete/SaveAll no service)
                var json = JsonConvert.SerializeObject(_cachePedidos, Formatting.Indented);
                File.WriteAllText(Paths.PedidosJson, json);

                DebugStatus = $"Pedido #{id} excluído com sucesso.";

                // limpa seleção e tela
                PedidoSelecionadoSalvo = null;
                PedidoSelecionado = null;

                NovoPedido();
                AtualizarCanExecute();
                ExcluirPedidoCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao excluir pedido: " + ex.Message;
            }
        }

        //Finaliza o pedido atual e salva
        private void Finalizar()
        {
            try
            {
                // Monta objeto pra salvar
                var salvar = new Pedido
                {
                    Pessoa = PessoaSelecionada,
                    Itens = Itens.Select(i => new PedidoItem
                    {
                        Produto = i.Produto,       // salva produto completo 
                        Quantidade = i.Quantidade
                    }).ToList(),
                    DataVenda = DateTime.Now,
                    FormaPagamento = FormaPagamento,
                    Status = StatusPedido.Pago
                };

                _pedidoService.Add(salvar);

                Bloqueado = true;
                DebugStatus = "Pedido finalizado e salvo! Total: " + salvar.ValorTotal().ToString("C");

                // Recarrega lista
                CarregarPedidosSalvos();
            }
            catch (Exception ex)
            {
                DebugStatus = "ERRO ao finalizar: " + ex.Message;
            }
        }

        // ======== FILTRO ========
        private void LimparFiltroPedidos()
        {
            FiltroPedidos = "";

            PedidosSalvos.Clear();
            foreach (var p in _cachePedidos)
                PedidosSalvos.Add(p);

            DebugStatus = $"Filtro limpo: {PedidosSalvos.Count} pedido(s).";
        }

        // ======== NOVO / CARREGAR PEDIDO ========

        private void NovoPedido()
        {
            Bloqueado = false;

            // Pedido novo (não salvo)
            PedidoAtual = new Pedido
            {
                DataVenda = DateTime.Now,
                Status = StatusPedido.Pendente
            };

            PessoaSelecionada = null;
            FormaPagamento = default(FormaPagamento);

            ProdutoSelecionado = null;
            Quantidade = 1;

            Itens.Clear();
            ItemSelecionado = null;

            PedidoSelecionadoSalvo = null;
            PedidoSelecionado = null;

            OnPropertyChanged(nameof(Total));
            DebugStatus = "Novo pedido iniciado.";
        }

        // ======== ITENS ========

        //Valida se pode adicionar item ao pedido atual
        private bool PodeAdicionarItem()
        {
            if (Bloqueado) return false;
            if (ProdutoSelecionado == null) return false;
            if (Quantidade < 1) return false;
            return true;
        }

        //Valida se pode excluir o pedido selecionado
        private bool PodeExcluirPedido()
        {
            // precisa ter um pedido selecionado no grid de pedidos salvos
            return PedidoSelecionadoSalvo != null;
        }
        //Valida se pode finalizar o pedido atual
        private bool PodeFinalizar()
        {
            if (Bloqueado) return false;
            if (PessoaSelecionada == null) return false;
            if (Itens.Count == 0) return false;
            return true;
        }
        //Valida se pode remover item do pedido atual
        private bool PodeRemoverItem()
        {
            if (Bloqueado) return false;
            return ItemSelecionado != null;
        }

        //Remove item selecionado do pedido atual
        private void RemoverItem()
        {
            if (ItemSelecionado == null) return;

            Itens.Remove(ItemSelecionado);
            ItemSelecionado = null;

            PedidoAtual.Itens = Itens.ToList();
            OnPropertyChanged(nameof(Total));

            DebugStatus = "Item removido. Total: " + Total.ToString("C");
        }
       
    }
}