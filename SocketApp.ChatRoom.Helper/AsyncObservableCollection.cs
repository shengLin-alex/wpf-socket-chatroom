using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace SocketApp.ChatRoom.Helper
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext CurrentContext = SynchronizationContext.Current;

        public AsyncObservableCollection() : base()
        {
        }

        public AsyncObservableCollection(IEnumerable<T> list) : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.CurrentContext)
            {
                this.RaiseCollectionChanged(e);
            }
            else
            {
                this.CurrentContext.Post(this.RaiseCollectionChanged, e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == this.CurrentContext)
            {
                this.RaisePropertyChanged(e);
            }
            else
            {
                this.CurrentContext.Post(this.RaisePropertyChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }
}