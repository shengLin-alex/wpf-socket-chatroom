using Microsoft.Extensions.Logging;
using SocketApp.ChatRoom.Helper;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Client.DataBinding
{
    public class ClientSideViewModel : IClientSideViewModel, INotifyPropertyChanged, IDisposable
    {
        // logger
        private readonly ILogger<ClientSideViewModel> Logger;

        // thread
        private readonly ClientThreadHandler Handler;
        private readonly object SyncRoot = new object();

        // socket setting
        private readonly Socket ClientSocket;
        private const int PORT = 7000;
        private const string IP = "127.0.0.1";

        // data model
        private readonly BindingDataModel BindingData;

        public ClientSideViewModel(ILogger<ClientSideViewModel> logger)
        {
            this.Logger = logger;
            this.BindingData = new BindingDataModel();
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ReceivedMessages = new AsyncObservableCollection<string>(App.Current.Dispatcher);
            this.Handler = new ClientThreadHandler(this);
        }

        public AsyncObservableCollection<string> ReceivedMessages { get; private set; }

        ~ClientSideViewModel()
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
            lock (this.SyncRoot)
            {
                if (disposing)
                {
                    this.Handler?.RequireStop();
                    this.ClientSocket?.Close();
                }
            }
        }

        public string MessageInput
        {
            get
            {
                return this.BindingData.MessageInput;
            }
            set
            {
                this.BindingData.MessageInput = value;
                this.OnPropertyChanged(nameof(this.MessageInput));
            }
        }

        public bool IsSendMessageButtonEnable
        {
            get => this.BindingData.IsSendMessageButtonEnable;
            set
            {
                this.BindingData.IsSendMessageButtonEnable = value;
                this.OnPropertyChanged(nameof(this.IsSendMessageButtonEnable));
            }
        }

        public bool IsConnectButtonEnable
        {
            get => this.BindingData.IsConnectButtonEnable;
            set
            {
                this.BindingData.IsConnectButtonEnable = value;
                this.OnPropertyChanged(nameof(this.IsConnectButtonEnable));
            }
        }

        public ICommand TryConnectToServer => new RelayCommand(this.ConnectToServer, this.CanUpdateControlExecute);

        public ICommand TrySendMessage => new RelayCommand(this.SendMessage, this.CanUpdateControlExecute);

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void ConnectToServer()
        {
            IPAddress ip = IPAddress.Parse(IP);

            try
            {
                this.ClientSocket.Connect(new IPEndPoint(ip, PORT));
                this.Handler.RequireStart();
                Thread receiveThread = new Thread(() => this.Handler.ReceiveMessage(this.ClientSocket))
                {
                    IsBackground = true // make thread background, for avoiding process not really shutdown.
                };
                receiveThread.Start();

                this.IsSendMessageButtonEnable = true;
                this.IsConnectButtonEnable = false;
                this.Logger.LogInformation("Server Start");
            }
            catch (Exception e)
            {
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
            }
        }

        private void SendMessage()
        {
            string text = this.MessageInput;

            Thread sendMessageThread = new Thread(() =>
            {
                try
                {
                    this.ClientSocket.Send(Encoding.UTF8.GetBytes(text));
                }
                catch (Exception e)
                {
                    this.ReceivedMessages.Add(e.Message);
                }
            })
            {
                IsBackground = true
            };

            sendMessageThread.Start();
        }

        private class ClientThreadHandler
        {
            private readonly ClientSideViewModel Outer;

            private volatile bool IsActive;

            public ClientThreadHandler(ClientSideViewModel outer)
            {
                this.Outer = outer;
            }

            public void RequireStart()
            {
                this.IsActive = true;
            }

            public void RequireStop()
            {
                this.IsActive = false;
            }

            public void ReceiveMessage(Socket connection)
            {
                try
                {
                    while (this.IsActive)
                    {
                        byte[] buffer = new byte[1024];
                        int receiveNumber = connection.Receive(buffer);

                        string receiveString = Encoding.UTF8.GetString(buffer, 0, receiveNumber);
                        this.Outer.ReceivedMessages.Add(receiveString);
                    }
                }
                catch (Exception e)
                {
                    this.Outer.ReceivedMessages.Add(e.Message);
                    this.Outer.IsSendMessageButtonEnable = false;
                    this.Outer.IsConnectButtonEnable = false;
                }
            }
        }
    }
}