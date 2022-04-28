using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations
{
    public class CoreExpander : Expander
    {
        public CoreExpander()
        {
        }

        #region Properties

        protected GroupItem ParentCore { get; set; }

        #endregion

        #region Methods

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            var itemsPresenter = newContent as ItemsPresenter;
            if (itemsPresenter != null)
            {
                object parent = (itemsPresenter.TemplatedParent as GroupItem);
                if (parent != null)
                {
                    //var data = parent.Content;
                    PropertyInfo property = parent.GetType()
                        .GetProperty(
                            "ParentItemStorageProvider",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.CreateInstance);
                    if (property != null)
                    {
                        object parentData = property.GetValue(parent, null);
                        this.ParentCore = parentData as GroupItem;
                    }
                }
            }
            base.OnContentChanged(oldContent, newContent);
        }

        #endregion
    }
}