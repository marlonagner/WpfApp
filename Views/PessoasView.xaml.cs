using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class PessoasView : UserControl
    {
        public PessoasView()
        {
            InitializeComponent();
            DataContext = new PessoasViewModel();
        }
    }
}