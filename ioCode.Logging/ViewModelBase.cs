//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ioCode.Logging
{
    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanging, INotifyPropertyChanged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private bool _disposed = false;

        #region <Dispose Methods>

        ~ViewModelBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDisposed(bool disposing)
        {
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            OnDisposed(disposing);
        }

        #endregion // </Dispose Methods>

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            bool changed = !EqualityComparer<T>.Default.Equals(field, newValue);
            if (changed)
            {
                OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }

            return changed;
        }

        protected virtual void OnPropertyChanging(PropertyChangingEventArgs args)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging.Invoke(this, args);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, args);
            }
        }

        protected internal void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        protected internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
