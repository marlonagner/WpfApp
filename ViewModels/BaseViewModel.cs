    using System.ComponentModel;
    using System.Runtime.CompilerServices;


namespace WpfApp.ViewModels
{

    namespace WpfApp.ViewModels
    {
        //A classe estende da interface NotifyPropertyChanged
        public class BaseViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}