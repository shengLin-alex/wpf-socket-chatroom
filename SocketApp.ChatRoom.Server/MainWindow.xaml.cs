using SocketApp.ChatRoom.Server.DataBinding;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow(IServerSideViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        public MainWindow()
        {
        }

        bool? IMainWindow.ShowDialog()
        {
            return base.ShowDialog();
        }
    }
}