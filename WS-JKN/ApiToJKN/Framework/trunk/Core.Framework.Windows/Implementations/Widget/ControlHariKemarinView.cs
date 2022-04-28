using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Core.Framework.Windows.Implementations.Widget
{
    /// <summary>
    /// Interaction logic for KunjunganHarianView.xaml
    /// </summary>
    public partial class ControlHariKemarinView
    {
        public ControlHariKemarinView()
        {
            InitializeComponent();
            var id = new CultureInfo("id-ID");
            TbHariIni.Text = DateTime.Now.Date.ToString("dddd, dd MMMM yyy", id);
            TbKemarin.Text = DateTime.Now.Date.AddDays(-1).ToString("dddd, dd MMMM yyy", id);
        }

        public event EventHandler RefreshWidget;
        public void OnRefreshWidget(EventArgs e)
        {
            var handler = RefreshWidget;
            if (handler != null) handler(this, e);
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

        public string NilaiHariIni
        {
            get { return (string)GetValue(NilaiHariIniProperty); }
            set
            {
                SetValue(NilaiHariIniProperty, value);
                TbNilaiHariIni.Text = value;
            }
        }

        public string NilaiKemarin
        {
            get { return (string)GetValue(NilaiKemarinPropertyProperty); }
            set
            {
                SetValue(NilaiKemarinPropertyProperty, value);
                TbNilaiKemarin.Text = value;
            }
        }

        public string SatuanHariIni
        {
            get { return (string)GetValue(SatuanHariIniProperty); }
            set
            {
                SetValue(SatuanHariIniProperty, value);
                TbSatuanHariIni.Text = value;
            }
        }

        public string SatuanKemarin
        {
            get { return (string)GetValue(SatuanKemarinProperty); }
            set
            {
                SetValue(SatuanKemarinProperty, value);
                TbSatuanKemarin.Text = value;
            }
        }

        public bool? IsDown
        {
            get { return (bool)GetValue(IsDownProperty); }
            set
            {
                var result = value;
                SetValue(IsDownProperty, result);
                if (result != null)
                {
                    var isDown = Convert.ToBoolean(result);
                    if (isDown)
                    {
                        VbDown.Visibility = Visibility.Visible;
                        VbUp.Visibility = Visibility.Collapsed;
                        TbValue.Text = "-" + (Convert.ToSingle(NilaiKemarin) - Convert.ToSingle(NilaiHariIni));
                    }
                    else
                    {
                        VbDown.Visibility = Visibility.Collapsed;
                        VbUp.Visibility = Visibility.Visible;
                        TbValue.Text = "+" + (Convert.ToSingle(NilaiHariIni) - Convert.ToSingle(NilaiKemarin));
                    }
                }
                else
                {
                    VbDown.Visibility = Visibility.Collapsed;
                    VbUp.Visibility = Visibility.Collapsed;
                    TbValue.Text = null;
                }
            }
        }

        public static readonly DependencyProperty IsDownProperty =
            DependencyProperty.Register("IsDown", typeof(bool?), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SatuanKemarinProperty =
            DependencyProperty.Register("SatuanKemarin", typeof(string), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SatuanHariIniProperty =
            DependencyProperty.Register("SatuanHariIni", typeof(string), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty NilaiKemarinPropertyProperty =
            DependencyProperty.Register("NilaiKemarin", typeof(string), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty NilaiHariIniProperty =
            DependencyProperty.Register("NilaiHariIni", typeof(string), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty TittleProperty =
            DependencyProperty.Register("Tittle", typeof(string), typeof(ControlHariKemarinView), new UIPropertyMetadata(null));
    }
}
