using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Core.Framework.Helper.Logging;
using Core.Framework.Windows.Contracts;
using Core.Framework.Windows.Implementations;
using TabControl = System.Windows.Controls.TabControl;

namespace Core.Framework.Windows.Helper
{
    public class Validate
    {
        /// <summary>
        /// Method untuk menampilkan mana item yang mandatory / wajib diisi pada form, sesuai dengan Field Attribut domain
        /// </summary>
        /// <param name="formGrid">parameter RequiredGrid</param>
        public static void ValidateOnLoad(RequiredGrid formGrid)
        {
            try
            {
                bool valid = true;
                if (formGrid != null)
                {
                    Manager.Timeout(Dispatcher.CurrentDispatcher, () =>
                    {
                        DependencyObject panel = null;
                        if (formGrid is DependencyObject)
                        {
                            panel = formGrid as DependencyObject;
                        }

                        if (panel == null)
                        {
                            panel = Manager.FindVisualParent<FormGrid>(formGrid);
                        }

                        IEnumerable<IValidateControl> children = Manager.FindVisualChildren<FrameworkElement>(panel).OfType<IValidateControl>();
                        foreach (IValidateControl validateControl in children)
                        {
                            if (validateControl.IsRequired)
                            {
                                if (validateControl.IsNull)
                                {
                                    if (valid)
                                    {
                                        valid = false;
                                        var tabItem =
                                            Manager.FindVisualParent<TabItem>((validateControl as FrameworkElement));
                                        if (tabItem != null)
                                        {
                                            var tabControl = tabItem.Parent as TabControl;
                                            if (tabControl != null)
                                            {
                                                tabControl.SelectedItem = tabItem;
                                            }
                                        }
                                        validateControl.FocusControl();
                                    }
                                    validateControl.IsError = true;
                                }
                                else
                                {
                                    validateControl.IsError = false;
                                }
                            }

                        }
                    });                    
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
