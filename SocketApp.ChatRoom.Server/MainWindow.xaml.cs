using SocketApp.ChatRoom.Server.DataBinding;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ServerSideViewModel();
        }
    }
}