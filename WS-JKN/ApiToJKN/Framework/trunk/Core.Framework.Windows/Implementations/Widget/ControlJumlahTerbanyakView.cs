using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations.Widget
{
    /// <summary>
    /// Interaction logic for ControlJumlahTerbanyakView.xaml
    /// </summary>
    public partial class ControlJumlahTerbanyakView
    {
        public ControlJumlahTerbanyakView()
        {
            this.InitializeComponent();
        }

        public string Tittle
        {
            get { return (string)GetValue(TittleProperty); }
            set
            {
                SetValue(TittleProperty, value);
                TbTitle.Text = value;
            }
        }

        public IEnumerable<object> Container
        {
            get { return (IEnumerable<ControlJumlahProgressView>)GetValue(ContainerProperty); }
            set
            {
                var items = value as IEnumerable<ControlJumlahProgressView>;
                SetValue(ContainerProperty, items);
                if (items != null)
                {
                    var take5s = items.OrderByDescending(n => n.Jumlah).Take(5);
                    var first = take5s.FirstOrDefault();
                    int maxProgress = 0;
                    if (first != null)
                    {
                        if (first.Progress >= 0 && first.Progress <= 100)
                            maxProgress = 100;
                        else if (first.Progress > 100 && first.Progress <= 500)
                            maxProgress = 500;
                        else if (first.Progress > 500 && first.Progress <= 1000)
                            maxProgress = 1000;
                        else if (first.Progress > 1000 && first.Progress <= 10000)
                            maxProgress = 10000;
                        else
                            maxProgress = first.Progress;
                    }

                    foreach (var container in take5s)
                    {
                        var result = container;
                        if (result != null)
                        {
                            result.PbJumlah.Maximum = maxProgress;
                            SpContainer.Children.Add(result);
                        }
                    }
                }
            }
        }

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container", typeof(IEnumerable<object>), typeof(ControlJumlahTerbanyakView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty TittleProperty =
            DependencyProperty.Register("Tittle", typeof(string), typeof(ControlJumlahTerbanyakView), new UIPropertyMetadata(null));


    }
}