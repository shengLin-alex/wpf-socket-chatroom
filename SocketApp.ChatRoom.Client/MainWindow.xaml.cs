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

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.ClientSocket.Connect(new IPEndPoint(ip, 7000));

                Thread receiveThread = new Thread(this.ReceiveMessage)
                {
                    IsBackground = true // make thread background, for avoiding process not really shutdown.
                };
                receiveThread.Start(this.ClientSocket);
            }
            catch
            {
                this.SendMessageButton.IsEnabled = false;
                this.MessageContentLabel.Content += "Failed To Connect To Server.";
            }
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
                    this.MessageContentLabel.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.MessageContentLabel.Content += $"\r\n{receiveString}";
                    }));
                }
                catch
                {
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();

                    break;
                }
            }
        }

        private void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            string text = this.MessageInputText.Text;

            try
            {
                this.ClientSocket.Send(Encoding.ASCII.GetBytes(text));
            }
            catch
            {
                this.MessageContentLabel.Content = "Failed To Connect To Server";
                this.ClientSocket.Shutdown(SocketShutdown.Both);
                this.ClientSocket.Close();
            }
        }
    }
}