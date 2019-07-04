using Microsoft.Extensions.Logging;
using SocketApp.ChatRoom.Helper;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Client.DataBinding
{
    public class ClientSideViewModel : IClientSideViewModel, INotifyPropertyChanged, IDisposable
    {
        // logger
        private readonly ILogger<ClientSideViewModel> Logger;

        // socket setting
        private readonly Socket ClientSocket;
        private const int PORT = 7000;
        private const string IP = "127.0.0.1";
        private byte[] ByteData;

        // data model
        private readonly BindingDataModel BindingData;

        public ClientSideViewModel(ILogger<ClientSideViewModel> logger)
        {
            this.Logger = logger;
            this.BindingData = new BindingDataModel();
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ByteData = new byte[1024];

            this.ReceivedMessages = new AsyncObservableCollection<string>(App.Current.Dispatcher);
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
        }

        public string MessageInput
        {
            get => this.BindingData.MessageInput;
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
            try
            {
                IPAddress ip = IPAddress.Parse(IP);
                this.ClientSocket.BeginConnect(new IPEndPoint(ip, PORT), new AsyncCallback(this.OnConnect), this.ClientSocket);

                this.IsSendMessageButtonEnable = true;
                this.IsConnectButtonEnable = false;
                this.Logger.LogInformation("Server Start");
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
            }
        }

        private void OnConnect(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket client)
                {
                    client.EndConnect(asyncResult);
                    client.BeginReceive(this.ByteData, 0, this.ByteData.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), client);
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
            }
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket client)
                {
                    int byteRead = client.EndReceive(asyncResult);
                    string receiveString = Encoding.UTF8.GetString(this.ByteData, 0, byteRead);
                    this.ReceivedMessages.Add(receiveString);

                    client.BeginReceive(this.ByteData, 0, this.ByteData.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), client);
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
            }
        }

        private void SendMessage()
        {
            try
            {
                string inputMessage = this.MessageInput;
                byte[] message = Encoding.UTF8.GetBytes(inputMessage);
                this.ClientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(this.OnSend), this.ClientSocket);
            }
            catch (Exception e)
            {
                this.Logger.LogError($"{e.GetType()};{e.Message}");
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
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
                this.IsSendMessageButtonEnable = false;
                this.ReceivedMessages.Add(e.Message);
            }
        }
    }
}