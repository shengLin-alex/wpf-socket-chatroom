using Microsoft.Extensions.Logging;
using SocketApp.ChatRoom.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    /// <summary>
    /// Server Side ViewModel
    /// </summary>
    public class ServerSideViewModel : IServerSideViewModel, INotifyPropertyChanged, IDisposable
    {
        private readonly ILogger<ServerSideViewModel> Logger;

        private readonly Socket ServerSocket;
        private readonly List<Socket> ClientSockets;
        private byte[] ByteData;

        private const int PORT = 7000;
        private const string IP = "127.0.0.1";

        private bool IsStartButtonEnableField = true;

        /// <summary>
        /// constructor
        /// </summary>
        public ServerSideViewModel(ILogger<ServerSideViewModel> logger)
        {
            this.Logger = logger;

            this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ClientSockets = new List<Socket>();
            this.ByteData = new byte[1024];

            this.ClientMessages = new AsyncObservableCollection<string>(App.Current.Dispatcher);
        }

        /// <summary>
        /// Client Messages that send to server.
        /// </summary>
        public AsyncObservableCollection<string> ClientMessages { get; }

        public bool IsStartButtonEnable
        {
            get => this.IsStartButtonEnableField;
            set
            {
                this.IsStartButtonEnableField = value;
                this.OnPropertyChanged(nameof(this.IsStartButtonEnable));
            }
        }

        /// <summary>
        /// Binding command for Start Listening
        /// </summary>
        public ICommand StartListening => new RelayCommand(this.StartServer, this.CanUpdateControlExecute);

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
        }

        /// <summary>
        /// Property Changed listener for data property(beside ObservableCollection)
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            // thread-safe call PropertyChanged
            App.Current.Dispatcher.Invoke(() =>
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        private bool CanUpdateControlExecute()
        {
            return true;
        }

        /// <summary>
        /// Start Server socket
        /// </summary>
        private void StartServer()
        {
            // localhost ip
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ServerSocket.Bind(new IPEndPoint(ip, PORT)); // bind port 7000
                this.ServerSocket.Listen(10);
                this.ServerSocket.BeginAccept(new AsyncCallback(this.OnAccept), null);
                this.IsStartButtonEnable = false;
                this.ClientMessages.Add("Start listening...");
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.ClientMessages.Add(e.Message);
            }
        }

        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                Socket client = this.ServerSocket.EndAccept(asyncResult);
                this.ClientSockets.Add(client);
                if (client.RemoteEndPoint is IPEndPoint endPoint)
                {
                    this.ClientMessages.Add($"Connection {endPoint.Address}:{endPoint.Port} connected.");
                }

                // start listening for more clients
                this.ServerSocket.BeginAccept(new AsyncCallback(this.OnAccept), null);
                client.BeginReceive(this.ByteData, 0, this.ByteData.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), client);
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.ClientMessages.Add(e.Message);
            }
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket client)
                {
                    int byteRead = client.EndReceive(asyncResult);
                    this.ClientSockets.RemoveAll(s => !s.IsAvailable()); // remove unavailable sockets
                    string receiveString = Encoding.UTF8.GetString(this.ByteData, 0, byteRead);

                    if (!(client.RemoteEndPoint is IPEndPoint ipEndPoint))
                    {
                        return;
                    }

                    IPAddress clientIp = ipEndPoint.Address;
                    int clientPort = ipEndPoint.Port;
                    string sendMessage = $"{clientIp} : {clientPort} ---> {receiveString}";

                    foreach (Socket conn in this.ClientSockets)
                    {
                        if (!conn.IsAvailable())
                        {
                            continue;
                        }
                        byte[] message = Encoding.UTF8.GetBytes(sendMessage);
                        conn.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(this.OnSend), conn);
                    }
                    this.ClientMessages.Add(sendMessage);
                    client.BeginReceive(this.ByteData, 0, this.ByteData.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), client);
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.ClientMessages.Add(e.Message);
            }
        }

        private void OnSend(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket client)
                {
                    client.EndSend(asyncResult);
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.ClientMessages.Add(e.Message);
            }
        }
    }
}