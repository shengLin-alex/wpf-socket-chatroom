using SocketApp.ChatRoom.Helper;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    public class BindingDataModel
    {
        public AsyncObservableCollection<string> ClientMessages { get; set; }

        public bool IsStartButtonEnable { get; set; } = true;
    }
}