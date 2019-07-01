using SocketApp.ChatRoom.Server.DataBinding;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace SocketApp.ChatRoom.Server
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly Socket ServerSocket;

        private readonly List<Socket> ClientSockets;

        private const int PORT = 7000;

        private const string IP = "127.0.0.1";

        // cross thread data binding context.
        private readonly SynchronizationContext SyncContext;
        private readonly ServerSideViewModel DataObject;

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ClientSockets = new List<Socket>();

            // setting data binding
            this.SyncContext = SynchronizationContext.Current;
            this.DataObject = new ServerSideViewModel();
            this.DataObject.SetSynchronizationContext(this.SyncContext);
            this.DataContext = this.DataObject;
        }

        public void Dispose()
        {
            this.ServerSocket.Shutdown(SocketShutdown.Both);
            this.ServerSocket.Close();
        }

        /// <summary>
        /// Start Listen button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartListenButtonClick(object sender, RoutedEventArgs e)
        {
            // localhost ip
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ServerSocket.Bind(new IPEndPoint(ip, PORT)); // bind port 7000
            }
            catch
            {
                this.DataObject.Message += "Server is already listening...\r\n";

                return;
            }

            this.ServerSocket.Listen(10); // up to 10 client
            this.DataObject.Message += "Start Listening...\r\n";

            Thread serverThread = new Thread(() =>
            {
                // server socket listen loop.
                while (true)
                {
                    Socket client = this.ServerSocket.Accept();
                    this.ClientSockets.Add(client);

                    Thread receiveThread = new Thread(this.ReceiveMessage)
                    {
                        IsBackground = true // make thread background, for avoiding process not really shutdown.
                    };
                    receiveThread.Start(client);
                }
            })
            {
                IsBackground = true // make thread background, for avoiding process not really shutdown.
            };
            serverThread.Start();
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
                    byte[] buffer = new byte[1024]; // buffer
                    int receiveNumber = connection.Receive(buffer);
                    string receiveString = Encoding.ASCII.GetString(buffer, 0, receiveNumber);

                    if (!(connection.RemoteEndPoint is IPEndPoint ipEndPoint))
                    {
                        continue;
                    }

                    IPAddress clientIp = ipEndPoint.Address;
                    int clientPort = ipEndPoint.Port;

                    string sendMessage = $"{clientIp} : {clientPort} ---> {receiveString}";
                    foreach (Socket socket in this.ClientSockets) // send message to all client.
                    {
                        socket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    }

                    this.DataObject.Message += $"\r\n{sendMessage}";
                }
                catch
                {
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();

                    break;
                }
            }
        }
    }
}