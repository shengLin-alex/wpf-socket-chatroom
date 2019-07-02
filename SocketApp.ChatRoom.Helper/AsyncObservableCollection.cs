using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace SocketApp.ChatRoom.Helper
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext CurrentContext = SynchronizationContext.Current;

        private readonly Dispatcher Dispatcher;

        public AsyncObservableCollection(Dispatcher dispatcher) : base()
        {
            this.Dispatcher = dispatcher;
        }

        public AsyncObservableCollection(IEnumerable<T> list, Dispatcher dispatcher) : base(list)
        {
            this.Dispatcher = dispatcher;
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

        protected override void SetItem(int index, T item)
        {
            this.ExecuteOrInvoke(() => base.SetItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this.ExecuteOrInvoke(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void ClearItems()
        {
            this.ExecuteOrInvoke(base.ClearItems);
        }

        protected override void InsertItem(int index, T item)
        {
            this.ExecuteOrInvoke(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            this.ExecuteOrInvoke(() => base.RemoveItem(index));
        }

        private void RaiseCollectionChanged(object param)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }

        private void ExecuteOrInvoke(Action action)
        {
            if (this.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                this.Dispatcher.Invoke(action);
            }
        }
    }
}