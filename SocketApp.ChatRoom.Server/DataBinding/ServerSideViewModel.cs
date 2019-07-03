using SocketApp.ChatRoom.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    /// <summary>
    /// Server Side ViewModel
    /// </summary>
    public class ServerSideViewModel : IServerSideViewModel, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The server socket
        /// </summary>
        private readonly Socket ServerSocket;

        /// <summary>
        /// The client sockets
        /// </summary>
        private readonly List<Socket> ClientSockets;

        /// <summary>
        /// Server port
        /// </summary>
        private const int PORT = 7000;

        /// <summary>
        /// Server IP address
        /// </summary>
        private const string IP = "127.0.0.1";

        private bool IsStartButtonEnableField = true;

        /// <summary>
        /// constructor
        /// </summary>
        public ServerSideViewModel()
        {
            this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ClientSockets = new List<Socket>();
            this.ClientMessages = new AsyncObservableCollection<string>(App.Current.Dispatcher);
        }

        /// <summary>
        /// Client Messages that send to server.
        /// </summary>
        public AsyncObservableCollection<string> ClientMessages { get; private set; }

        public bool IsStartButtonEnable
        {
            get
            {
                return this.IsStartButtonEnableField;
            }
            set
            {
                this.IsStartButtonEnableField = value;
                this.OnPropertyChanged(nameof(this.IsStartButtonEnable));
            }
        }

        /// <summary>
        /// Binding command for Start Listening
        /// </summary>
        public ICommand StartListening
        {
            get
            {
                return new RelayCommand(this.StartServer, this.CanUpdateControlExecute);
            }
        }

        /// <summary>
        /// Property Changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ~ServerSideViewModel()
        {
            // Finalizer calls Dispose(false)
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ServerSocket.Close();
            }
        }

        /// <summary>
        /// Property Changed listener for data property(beside ObservableCollection)
        /// </summary>
        /// <param name="name"></param>
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

        /// <summary>
        /// Start Server and start a thread for receive message.
        /// </summary>
        private void StartServer()
        {
            // localhost ip
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ServerSocket.Bind(new IPEndPoint(ip, PORT)); // bind port 7000
                this.IsStartButtonEnable = false;
            }
            catch
            {
                this.ClientMessages.Add("Cannot start server, try later...");

                return;
            }

            this.ServerSocket.Listen(20); // up to 10 client
            this.ClientMessages.Add("Start Listening...");

            Thread serverThread = new Thread(() =>
            {
                // server socket listen loop.
                while (true)
                {
                    Socket client = this.ServerSocket.Accept();
                    IPEndPoint endPoint = client.RemoteEndPoint as IPEndPoint;
                    this.ClientMessages.Add($"Connection {endPoint.Address}:{endPoint.Port} connected.");
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

        /// <summary>
        /// Receive client message
        /// </summary>
        /// <param name="clientSocket">client socket</param>
        private void ReceiveMessage(object clientSocket)
        {
            if (!(clientSocket is Socket connection))
            {
                return;
            }

            while (true)
            {
                // remove not available client socket.
                this.ClientSockets.RemoveAll((s) => !s.IsConnected());

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
                        if (!socket.IsConnected()) // skip not available socket.
                        {
                            continue;
                        }

                        socket.Send(Encoding.UTF8.GetBytes(sendMessage));
                    }

                    this.ClientMessages.Add(sendMessage);
                }
                catch (Exception e)
                {
                    if (e.Message == "遠端主機已強制關閉一個現存的連線。")
                    {
                        IPEndPoint endPoint = this.ClientSockets.Find((s) => !s.IsConnected()).RemoteEndPoint as IPEndPoint;
                        this.ClientMessages.Add($"Connection {endPoint.Address}:{endPoint.Port} closed");
                    }
                    else
                    {
                        this.ClientMessages.Add(e.Message);
                    }

                    break;
                }
            }
        }
    }
}