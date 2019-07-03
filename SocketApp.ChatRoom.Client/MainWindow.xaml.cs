using SocketApp.ChatRoom.Client.DataBinding;
using System.Windows;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow(IClientSideViewModel viewModel)
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