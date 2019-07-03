using SocketApp.ChatRoom.Server.DataBinding;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
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