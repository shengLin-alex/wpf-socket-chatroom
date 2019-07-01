using SocketApp.ChatRoom.Server.DataBinding;
using System.Threading;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        // cross thread data binding context.
        private readonly SynchronizationContext SyncContext;

        private readonly ServerSideViewModel ViewModel;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // setting data binding
            this.SyncContext = SynchronizationContext.Current;
            this.ViewModel = new ServerSideViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}