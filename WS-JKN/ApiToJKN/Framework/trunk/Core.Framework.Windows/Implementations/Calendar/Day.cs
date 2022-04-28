/*
    Jarloo
    http://www.jarloo.com
 
    This work is licensed under a Creative Commons Attribution-ShareAlike 3.0 Unported License  
    http://creativecommons.org/licenses/by-sa/3.0/ 

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Framework.Windows.Implementations.Calendar
{
    public class Day : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime date;
        private string notes;
        private bool enabled;
        private bool isTargetMonth;
        private bool isToday;
        private string key;

        public Day()
        {
            //NotesCollection =new string[]
            //                     {
            //                         "a","b","c"
            //                     };
        }

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Key"));
            }
        }

        public bool IsToday
        {
            get { return isToday; }
            set
            {
                isToday = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsToday"));
            }
        }

        public bool IsTargetMonth
        {
            get { return isTargetMonth; }
            set
            {
                isTargetMonth = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsTargetMonth"));
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Notes"));
            }
        }

        private IEnumerable<string> notesCollection;
        public IEnumerable<string> NotesCollection
        {
            get { return notesCollection; }
            set
            {
                notesCollection = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NotesCollection"));
            }
        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }
    }
}