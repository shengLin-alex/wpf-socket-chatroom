using SocketApp.ChatRoom.Client.DataBinding;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : IMainWindow
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