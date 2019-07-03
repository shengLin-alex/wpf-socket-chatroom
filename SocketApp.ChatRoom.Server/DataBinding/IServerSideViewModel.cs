using SocketApp.ChatRoom.Helper;
using System.Windows.Input;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    public interface IServerSideViewModel
    {
        AsyncObservableCollection<string> ClientMessages { get; }

        bool IsStartButtonEnable { get; set; }

        ICommand StartListening { get; }
    }
}