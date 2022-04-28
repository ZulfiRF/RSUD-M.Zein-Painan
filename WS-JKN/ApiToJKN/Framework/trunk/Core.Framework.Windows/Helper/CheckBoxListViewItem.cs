using System;
using System.ComponentModel;

namespace Core.Framework.Windows.Helper
{
    public class CheckBoxListViewItem : INotifyPropertyChanged
    {
        private bool isChecked;
        private string text;
        private string idS;
        private byte idB;
        private int idI;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked == value) return;
                isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public String Text
        {
            get { return text; }
            set
            {
                if (text == value) return; text = value;
                RaisePropertyChanged("Text");
            }
        }

        public String IDS
        {
            get { return idS; }
            set
            {
                if (idS == value) return;
                idS = value;
                RaisePropertyChanged("IDS");
            }
        }

        public Byte IDB
        {
            get { return idB; }
            set
            {
                if (idB == value) return;
                idB = value;
                RaisePropertyChanged("IDB");
            }
        }

        public Int16 IDI
        {
            get { return (short) idI; }
            set
            {
                if (idI == value) return;
                idI = value;
                RaisePropertyChanged("IDI");
            }
        }

        public CheckBoxListViewItem(byte idB, string text, bool check)
        {
            this.IDB = idB;
            this.Text = text;
            this.IsChecked = check;
        }

        public CheckBoxListViewItem(string idS, string text, bool check)
        {
            this.IDS = idS;
            this.Text = text;
            this.IsChecked = check;
        }

        public CheckBoxListViewItem(int idI, string text, bool check)
        {
            this.IDI = (short) idI;
            this.Text = text;
            this.IsChecked = check;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}