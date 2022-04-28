using System;

namespace Core.Framework.Windows.Datagrid
{
    internal class ColumnActualWidthChangedEventArgs : EventArgs
    {
        public ColumnActualWidthChangedEventArgs( ColumnBase column , double oldValue, double newValue )
        {
            m_column = column;
            m_oldValue = oldValue;
            m_newValue = newValue;
        }

        public ColumnBase Column
        {
            get
            {
                return m_column;
            }
        }

        public double NewValue
        {
            get
            {
                return m_newValue;
            }
        }

        public double OldValue
        {
            get
            {
                return m_oldValue;
            }
        }

        private ColumnBase m_column;
        private double m_oldValue;
        private double m_newValue;
    }
}