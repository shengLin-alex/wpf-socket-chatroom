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

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.CurrentContext)
            {
                this.RaiseCollectionChanged(args);
            }
            else
            {
                this.CurrentContext.Post(this.RaiseCollectionChanged, args);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (SynchronizationContext.Current == this.CurrentContext)
            {
                this.RaisePropertyChanged(args);
            }
            else
            {
                this.CurrentContext.Post(this.RaisePropertyChanged, args);
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