using Core.Framework.Windows.Helper;

namespace Core.Framework.Windows.Implementations
{
    using System.Windows.Controls;

    using Core.Framework.Helper;
    using Core.Framework.Windows.Contracts;

    public class DefaultComboBox : ComboBox, IValueElement
    {
        public DefaultComboBox()
        {
            Manager.RegisterFormGrid(this);
        }
        #region Public Properties

        public bool CanFocus
        {
            get { return true; }
        }

        public string Key
        {
            get
            {
                return this.DisplayMemberPath;
            }
        }

        public object Value
        {
            get
            {
                if (this.SelectedItem == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(this.ValuePath))
                {
                    return this.SelectedItem;
                }
                return HelperManager.BindPengambilanObjekDariSource(this.SelectedItem, this.ValuePath);
            }
            set
            {
                this.SelectedItem = value;
                if (this.SelectedItem == null)
                {
                    if (this.ItemsSource != null)
                    {
                        foreach (object item in this.ItemsSource)
                        {
                            object data = HelperManager.BindPengambilanObjekDariSource(item, this.DisplayMemberPath);
                            if (data != null)
                            {
                                if (data.ToString().Equals(value.ToString()))
                                {
                                    this.SelectedItem = item;
                                }
                            }
                        }
                    }
                }
            }
        }

        public string ValuePath { get; set; }

        #endregion
    }
}