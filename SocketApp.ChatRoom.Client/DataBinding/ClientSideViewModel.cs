using System.ComponentModel;
using System.Threading;

namespace SocketApp.ChatRoom.Client.DataBinding
{
    public class ClientSideViewModel : INotifyPropertyChanged
    {
        private SynchronizationContext SyncContext;

        private string MessageInputField = "";

        private string MessageContentField = "";

        public ClientSideViewModel()
        {
        }

        public string MessageInput
        {
            get
            {
                return this.MessageInputField;
            }
            set
            {
                this.MessageInputField = value;
                this.OnPropertyChanged(nameof(this.MessageInput));
            }
        }

        public string MessageContent
        {
            get
            {
                return this.MessageContentField;
            }
            set
            {
                this.MessageContentField = value;
                this.OnPropertyChanged(nameof(this.MessageContent));
            }
        }

        public void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            // SyncContext
            this.SyncContext = syncContext;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            SendOrPostCallback methodDelegate = delegate (object state)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            };
            this.SyncContext.Post(methodDelegate, null);
        }
    }
}