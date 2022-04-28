﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework.Windows.Implementations.Drag
{
    /// <summary>
    /// Helper class to create view models, particularly for tool/MDI windows.
    /// </summary>
    public class HeaderedItemViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private object _header;
        private object _content;

        public object Header
        {
            get { return _header; }
            set
            {
                if (_header == value) return;
                _header = value;
#if NET40
                OnPropertyChanged("Header");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public object Content
        {
            get { return _content; }
            set
            {
                if (_content == value) return;
                _content = value;
#if NET40
                OnPropertyChanged("Content");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
#if NET40
                OnPropertyChanged("IsSelected");
#endif
#if NET45
                OnPropertyChanged();
#endif
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
