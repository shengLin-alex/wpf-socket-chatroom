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
    public class ClientSideViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Socket ClientSocket;
        private const int PORT = 7000;
        private const string IP = "127.0.0.1";

        private BindingDataModel BindingData;

        public ClientSideViewModel()
        {
            this.BindingData = new BindingDataModel();
            this.ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Dispose()
        {
            this.ClientSocket.Shutdown(SocketShutdown.Both);
            this.ClientSocket.Close();
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

        public string MessageContent
        {
            get
            {
                return this.BindingData.MessageContent;
            }
            set
            {
                this.BindingData.MessageContent = value;
                this.OnPropertyChanged(nameof(this.MessageContent));
            }
        }

        public bool IsSendMessageButtonEnable
        {
            get
            {
                return this.BindingData.IsSendMessageButtonEnable;
            }
            set
            {
                this.BindingData.IsSendMessageButtonEnable = value;
                this.OnPropertyChanged(nameof(this.IsSendMessageButtonEnable));
            }
        }

        public bool IsConnectButtonEnable
        {
            get
            {
                return this.BindingData.IsConnectButtonEnable;
            }
            set
            {
                this.BindingData.IsConnectButtonEnable = value;
                this.OnPropertyChanged(nameof(this.IsConnectButtonEnable));
            }
        }

        public ICommand TryConnectToServer
        {
            get
            {
                return new RelayCommand(this.ConnectToServer, this.CanUpdateControlExecute);
            }
        }

        public ICommand TrySendMessage
        {
            get
            {
                return new RelayCommand(this.SendMessage, this.CanUpdateControlExecute);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void ConnectToServer()
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

                this.IsSendMessageButtonEnable = true;
                this.IsConnectButtonEnable = false;
            }
            catch
            {
                this.IsSendMessageButtonEnable = false;
                this.MessageContent += "Failed To Connect To Server. Retry Later...\r\n";
            }
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
                    this.MessageContent += $"\r\n{receiveString}";
                }
                catch
                {
                    connection.Shutdown(SocketShutdown.Both);
                    connection.Close();

                    break;
                }
            }
        }

        private void SendMessage()
        {
            string text = this.MessageInput;

            try
            {
                this.ClientSocket.Send(Encoding.ASCII.GetBytes(text));
            }
            catch
            {
                this.MessageContent = "Failed To Connect To Server.\r\n";
                this.ClientSocket.Shutdown(SocketShutdown.Both);
                this.ClientSocket.Close();
            }
        }
    }
}