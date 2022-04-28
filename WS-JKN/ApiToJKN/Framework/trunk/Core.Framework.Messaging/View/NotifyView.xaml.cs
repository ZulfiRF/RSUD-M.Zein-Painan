using System.Windows.Controls;

namespace Core.Framework.Messaging.View
{
    /// <summary>
    /// Interaction logic for NotifyView.xaml
    /// </summary>
    public partial class NotifyView : UserControl
    {
        public NotifyView()
        {
            InitializeComponent();
        }


        public string NextForm { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }

        public void SetMessage(string nextForm, string message, int count)
        {
            NextForm = nextForm;
            Message = message;
            Count = count;
            TbMsg.Text = message;
            TbCount.Text = count.ToString();
        }
    }
}
