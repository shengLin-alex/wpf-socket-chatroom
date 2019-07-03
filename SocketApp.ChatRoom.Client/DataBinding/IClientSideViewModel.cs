using SocketApp.ChatRoom.Helper;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Client.DataBinding
{
    public interface IClientSideViewModel
    {
        AsyncObservableCollection<string> ReceivedMessages { get; }

        string MessageInput { get; set; }

        bool IsSendMessageButtonEnable { get; set; }

        bool IsConnectButtonEnable { get; set; }

        ICommand TryConnectToServer { get; }

        ICommand TrySendMessage { get; }
    }
}