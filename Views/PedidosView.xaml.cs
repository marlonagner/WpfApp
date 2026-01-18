using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class PedidosView : UserControl
    {
        public PedidosView()
        {
            InitializeComponent();
            DataContext = new PedidosViewModel();
        }

        private void DgPedidosSalvos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = DataContext as PedidosViewModel;
            if (vm == null) return;

            var pedido = dgPedidosSalvos.SelectedItem as Pedido;
            if (pedido == null) return;

            // força o ViewModel carregar itens/pessoa/forma
            vm.PedidoSelecionadoSalvo = pedido;
        }
    }
}