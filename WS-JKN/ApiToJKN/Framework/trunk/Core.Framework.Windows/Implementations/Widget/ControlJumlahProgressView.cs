using System.Windows;
using System.Windows.Controls;

namespace Core.Framework.Windows.Implementations.Widget
{
	/// <summary>
	/// Interaction logic for ControlJumlahProgressView.xaml
	/// </summary>
	public partial class ControlJumlahProgressView 
	{
		public ControlJumlahProgressView()
		{
			this.InitializeComponent();
		}

        public string Nama
        {
            get { return (string)GetValue(NamaProperty); }
            set
            {
                SetValue(NamaProperty, value);
                TbNama.Text = value;
            }
        }

        public string Jumlah
        {
            get { return (string)GetValue(JumlahProperty); }
            set
            {
                SetValue(JumlahProperty, value);
                TbJumlah.Text = value;
            }
        }

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set
            {
                SetValue(ProgressProperty, value);
                PbJumlah.Value = value;
            }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(ControlJumlahProgressView), new UIPropertyMetadata(0));

        public static readonly DependencyProperty JumlahProperty =
            DependencyProperty.Register("Jumlah", typeof(string), typeof(ControlJumlahProgressView), new UIPropertyMetadata(null));

        public static readonly DependencyProperty NamaProperty =
            DependencyProperty.Register("Nama1", typeof(string), typeof(ControlJumlahProgressView), new UIPropertyMetadata(null));

        
	}
}