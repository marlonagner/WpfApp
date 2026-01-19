using System.Windows.Controls;
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
    }
}