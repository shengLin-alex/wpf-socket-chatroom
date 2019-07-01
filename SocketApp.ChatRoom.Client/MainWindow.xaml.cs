using SocketApp.ChatRoom.Client.DataBinding;
using System.Threading;
using System.Windows;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // cross thread data binding context.
        private readonly SynchronizationContext SyncContext;

        private readonly ClientSideViewModel ViewModel;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // setting data binding
            this.SyncContext = SynchronizationContext.Current;
            this.ViewModel = new ClientSideViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}