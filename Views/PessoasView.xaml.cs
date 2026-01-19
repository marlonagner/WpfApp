using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    //Este arquivo define a classe parcial PessoasView que herda de UserControl.
    public partial class PessoasView : UserControl
    {
        public PessoasView()
        {
            InitializeComponent();
            DataContext = new PessoasViewModel();
        }
    }
}