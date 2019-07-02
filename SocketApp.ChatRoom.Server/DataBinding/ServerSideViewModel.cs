using SocketApp.ChatRoom.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    public class ServerSideViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Socket ServerSocket;
        private readonly List<Socket> ClientSockets;
        private const int PORT = 7000;
        private const string IP = "127.0.0.1";

        private object SyncRoot = new object();

        /// <summary>
        /// constructor
        /// </summary>
        public ServerSideViewModel()
        {
            this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ClientSockets = new List<Socket>();

            this.ClientMessages = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(this.ClientMessages, this.SyncRoot);
        }

        public ObservableCollection<string> ClientMessages { get; private set; }

        public ICommand StartListening
        {
            get
            {
                return new RelayCommand(this.StartServer, this.CanUpdateControlExecute);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            this.ServerSocket.Shutdown(SocketShutdown.Both);
            this.ServerSocket.Close();
        }

        private void OnPropertyChanged(string name)
        {
            // thread-safe call PropertyChanged
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }));
        }

        private bool CanUpdateControlExecute()
        {
            return true;
        }

        private void StartServer()
        {
            // localhost ip
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ServerSocket.Bind(new IPEndPoint(ip, PORT)); // bind port 7000
            }
            catch
            {
                lock (this.SyncRoot)
                {
                    this.ClientMessages.Add("Server is already listening...");
                }

                return;
            }

            this.ServerSocket.Listen(10); // up to 10 client
            lock (this.SyncRoot)
            {
                this.ClientMessages.Add("Start Listening...");
            }

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
                    string receiveString = Encoding.UTF8.GetString(buffer, 0, receiveNumber);

                    if (!(connection.RemoteEndPoint is IPEndPoint ipEndPoint))
                    {
                        continue;
                    }

                    IPAddress clientIp = ipEndPoint.Address;
                    int clientPort = ipEndPoint.Port;

                    string sendMessage = $"{clientIp} : {clientPort} ---> {receiveString}";
                    foreach (Socket socket in this.ClientSockets) // send message to all client.
                    {
                        socket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    }

                    lock (this.SyncRoot)
                    {
                        this.ClientMessages.Add(sendMessage);
                    }
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