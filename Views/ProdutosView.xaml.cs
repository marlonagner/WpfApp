using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    //Estende UserControl para criar uma View personalizada
    public partial class ProdutosView : UserControl
    {
        public ProdutosView()
        {
            InitializeComponent();

            // Garante que a View está usando o ViewModel (sem depender de XAML)
            DataContext = new ProdutosViewModel();
        }
    }
}
