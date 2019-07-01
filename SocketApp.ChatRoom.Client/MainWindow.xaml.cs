using SocketApp.ChatRoom.Client.DataBinding;
using System.Windows;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ClientSideViewModel();
        }
    }
}