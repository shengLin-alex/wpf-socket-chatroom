using SocketApp.ChatRoom.Client.DataBinding;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace SocketApp.ChatRoom.Client
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly Socket ClientSocket;

        private const int PORT = 7000;

        private const string IP = "127.0.0.1";

        // cross thread data binding context.
        private readonly SynchronizationContext SyncContext;
        private readonly ClientSideViewModel DataObject;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.SendMessageButton.IsEnabled = false;

            // setting data binding
            this.SyncContext = SynchronizationContext.Current;
            this.DataObject = new ClientSideViewModel();
            this.DataObject.SetSynchronizationContext(this.SyncContext);
            this.DataContext = this.DataObject;
        }

        public void Dispose()
        {
            this.ClientSocket.Shutdown(SocketShutdown.Both);
            this.ClientSocket.Close();
        }

        private void ReceiveMessage(object clientSocket)
        {
            if (!(clientSocket is Socket connection))
            {
                return;
            }

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receiveNumber = connection.Receive(buffer);

                    string receiveString = Encoding.ASCII.GetString(buffer, 0, receiveNumber);
                    this.DataObject.MessageContent += $"\r\n{receiveString}";
                }
                catch
                {
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();

                    break;
                }
            }
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ClientSocket.Connect(new IPEndPoint(ip, PORT));

                Thread receiveThread = new Thread(this.ReceiveMessage)
                {
                    IsBackground = true // make thread background, for avoiding process not really shutdown.
                };
                receiveThread.Start(this.ClientSocket);

                this.SendMessageButton.IsEnabled = true;
                this.ConnectButton.IsEnabled = false;
            }
            catch
            {
                this.SendMessageButton.IsEnabled = false;
                this.DataObject.MessageContent += "Failed To Connect To Server. Retry Later...\r\n";
            }
        }

        private void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            string text = this.DataObject.MessageInput;

            try
            {
                this.ClientSocket.Send(Encoding.ASCII.GetBytes(text));
            }
            catch
            {
                this.DataObject.MessageContent = "Failed To Connect To Server.\r\n";
                this.ClientSocket.Shutdown(SocketShutdown.Both);
                this.ClientSocket.Close();
            }
        }
    }
}