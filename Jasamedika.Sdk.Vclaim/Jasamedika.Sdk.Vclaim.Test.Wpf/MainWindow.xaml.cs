using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Jasamedika.Sdk.Vclaim.Attr;
using Jasamedika.Sdk.Vclaim.Event;

namespace Jasamedika.Sdk.Vclaim.Test.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ContextVclaim context;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;

            context = new ContextVclaim
            {
                Url = ConfigurationManager.AppSettings["Url"],
                ConsumerId = ConfigurationManager.AppSettings["ConsumerId"],
                PasswordKey = ConfigurationManager.AppSettings["Password"],
                //DefaultTime = ConfigurationManager.AppSettings["DefaultTime"]
            };

            context.LogLoad += ContextOnLogLoad;

            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            Title = Title + " v." + version;

            BtnSaveConf.Click += BtnSaveConfOnClick;
        }

        private void ContextOnLogLoad(object sender, LogEvent e)
        {
            var log = e.Log;
            if (!string.IsNullOrEmpty(log))
            {
                Dispatcher.Invoke(() =>
                {
                    TbLog.Text = TbLog.Text + "\n" + log;
                });
            }
        }

        private void BtnSaveConfOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            context.Url = TbUrl.Text;
            context.ConsumerId = TbCunId.Text;
            context.PasswordKey = TbPass.Password;
            if (DpDefTime.SelectedDate.HasValue)
                context.DefaultTime = DpDefTime.SelectedDate.Value.ToString("yyyy-MM-dd");
        }

        public MethodInfo CurrentMethodInfo { get; set; }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            TbUrl.Text = context.Url;
            TbCunId.Text = context.ConsumerId;
            TbPass.Password = context.PasswordKey;
            DpDefTime.Text = context.DefaultTime;

            WpMethod.Children.Clear();
            var getMethods = context.GetType().GetMethods();
            var i = 0;
            foreach (var method in getMethods)
            {
                var tipe = method.ReturnType;
                if (tipe == typeof(string[]))
                {
                    i++;
                    var text = new TextBlock();
                    text.Tag = method;
                    text.Cursor = Cursors.Hand;
                    text.Text = i + ". " + method.Name;
                    text.MouseDown -= TextMouseDown;
                    text.MouseDown += TextMouseDown;
                    WpMethod.Children.Add(text);
                }
            }
        }

        private void TextMouseDown(object sender, MouseButtonEventArgs e)
        {
            var text = sender as TextBlock;
            if (text != null)
            {
                var method = text.Tag as MethodInfo;
                if (method != null)
                {
                    CurrentMethodInfo = method;
                    var getAttrs = method.GetCustomAttributes<PropertyLabelAttribute>().ToList();
                    if (getAttrs != null)
                    {
                        TbStatus.Text = text.Text + " >> " + getAttrs.Count + " parameters";

                        WpParams.Children.Clear();
                        foreach (var attr in getAttrs)
                        {
                            var nameAttr = attr.Name;
                            var kodeAttr = attr.Kode;
                            var valueAttr = attr.DefaultValue;
                            var typeData = attr.TypeData;

                            var lblChild = new TextBlock();
                            lblChild.Text = nameAttr;
                            WpParams.Children.Add(lblChild);

                            if (typeData == typeof(string))
                            {
                                var textChild = new TextBox();
                                textChild.Text = valueAttr;
                                textChild.Tag = kodeAttr;
                                textChild.Width = 300;
                                textChild.Height = 25;
                                textChild.Margin = new Thickness(0, 0, 0, 10);
                                WpParams.Children.Add(textChild);
                            }
                            else if (typeData == typeof(DateTime))
                            {
                                var splits = valueAttr.Split('-');
                                var textChild = new DatePicker();
                                textChild.SelectedDate = new DateTime(Convert.ToInt32(splits[0]), Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                                textChild.Tag = kodeAttr;
                                textChild.Width = 300;
                                textChild.Height = 25;
                                textChild.Margin = new Thickness(0, 0, 0, 10);
                                WpParams.Children.Add(textChild);
                            }
                        }
                    }
                }

                var btn = new Button();
                btn.Click -= BtnClick;
                btn.Click += BtnClick;
                btn.Content = "Go";
                btn.Width = 300;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Height = 25;
                WpParams.Children.Add(btn);
            }
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                if (btn != null)
                {
                    btn.Content = "Loading ..";
                }
                var childs = WpParams.Children.OfType<Control>().ToList();
                var prmObj = new List<object>();
                foreach (var child in childs)
                {
                    if (child is TextBox)
                    {
                        prmObj.Add((child as TextBox).Text);
                    }
                    else if (child is DatePicker)
                    {
                        prmObj.Add((child as DatePicker).SelectedDate.Value.ToString("yyyy-MM-dd"));
                    }
                }

                var method = CurrentMethodInfo;
                if (method != null)
                {
                    ThreadPool.QueueUserWorkItem(GoCallBack, new object[]
                    {
                        method, prmObj, btn
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GoCallBack(object state)
        {
            var param = state as object[];
            if (param != null)
            {
                var method = param[0] as MethodInfo;
                var prmObj = param[1] as List<object>;
                var content = "";
                var result = method.Invoke(context, prmObj.ToArray());
                if (result != null)
                {
                    var resultArray = result as string[];
                    if (resultArray != null)
                    {
                        foreach (var s in resultArray)
                        {
                            content = content + s + "\n";
                        }
                    }
                }

                MessageBox.Show(content);
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    var btn = param[2] as Button;
                    if (btn != null)
                    {
                        btn.Content = "Go";
                    }
                }));
            }
        }
    }
}
