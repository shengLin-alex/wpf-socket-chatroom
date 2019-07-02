using SocketApp.ChatRoom.Server.DataBinding;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ServerSideViewModel ViewModel;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.ViewModel = new ServerSideViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}