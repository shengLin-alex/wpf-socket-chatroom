using System.Net.Sockets;

namespace SocketApp.ChatRoom.Helper
{
    public static class SocketExtensions
    {
        public static bool IsAvialable(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch
            {
                return false;
            }
        }
    }
}