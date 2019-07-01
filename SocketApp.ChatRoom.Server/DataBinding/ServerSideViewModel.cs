using System.ComponentModel;
using System.Threading;

namespace SocketApp.ChatRoom.Server.DataBinding
{
    public class ServerSideViewModel : INotifyPropertyChanged
    {
        private SynchronizationContext SyncContext;

        private string MessageField = "";

        /// <summary>
        /// constructor
        /// </summary>
        public ServerSideViewModel()
        {
        }

        public string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
                this.OnPropertyChanged(nameof(this.Message));
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