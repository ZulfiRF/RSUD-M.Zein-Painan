using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Framework.Helper;
using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    public class CoreListViewAsycn : CoreListView
    {
        public CoreListViewAsycn()
        {
            if (Application.Current == null) return;
            Style = Application.Current.Resources["lvStyle"] as Style;
            VirtualizingStackPanel.SetIsVirtualizing(this, true);
            VirtualizingStackPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
            ScrollViewer.SetIsDeferredScrollingEnabled(this, true);
            IsTextSearchEnabled = false;            
        }
    }
}