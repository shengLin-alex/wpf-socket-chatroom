namespace SocketApp.ChatRoom.Client.DataBinding
{
    public class BindingDataModel
    {
        public string MessageInput { get; set; } = "";
        public bool IsSendMessageButtonEnable { get; set; } = false;
        public bool IsConnectButtonEnable { get; set; } = true;
    }
}