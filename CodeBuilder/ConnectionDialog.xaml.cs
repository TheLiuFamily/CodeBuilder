using CodeBuilder.Common;
using CodeBuilder.Logic;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeBuilder
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionDialog : MetroWindow
    {

        public AuthTypes AuthType
        {
            get { return (AuthTypes)cboAuthTypes.SelectedItem; }
            set { cboAuthTypes.SelectedItem = value; }
        }

        public string Server
        {
            get { return cboServers.Text; }
            set { cboServers.Text = value; }
        }

        public string UserName
        {
            get { return txtUserName.Text; }
            set { txtUserName.Text = value; }
        }

        public string Password
        {
            get { return txtPassword.Password; }
            set { txtPassword.Password = value; }
        }

        private ServerInfo GetServerInfo
        {
            get
            {
                return new ServerInfo { AuthType = this.AuthType, Server = this.Server, User = this.UserName, Password = this.Password, Database = "master" };
            }
        }

        private ConnectionDialog()
        {
            InitializeComponent();

            Enum.GetValues(typeof(AuthTypes)).Cast<AuthTypes>().ForEach((s) => cboAuthTypes.Items.Add(s));
            cboAuthTypes.SelectedIndex = 0;

        }

        /// <summary>
        /// 用于保存弹出框父类
        /// </summary>
        private Window _window;
        public ConnectionDialog(ServerInfo info, Window window)
            : this()
        {
            if (info != null)
            {
                AuthType = info.AuthType;
                Server = info.Server;
                UserName = info.User;
                Password = info.Password;
                AuthType = info.AuthType;
            }
            _window = window;

            

        }
        public bool? UShowDialog()
        {
            //蒙板
            Grid layer = new Grid() { Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0)) };
            //父级窗体原来的内容
            UIElement original = _window.Content as UIElement;
            _window.Content = null;
            //容器Grid
            Grid container = new Grid();
            container.Children.Add(original);//放入原来的内容
            container.Children.Add(layer);//在上面放一层蒙板
            //将装有原来内容和蒙板的容器赋给父级窗体
            _window.Content = container;
            this.Owner = _window;
            return this.ShowDialog();
        }
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Server))
            {
                if (!string.IsNullOrEmpty(UserName) || AuthType == AuthTypes.Windows)
                {
                    if (IsSqlServer2005OrAbove())
                    {
                        this.DialogResult = true;
                    }
                }
                else
                    MessageBox.ShowDialog("Please input user name.", this);
            }
            else
                MessageBox.ShowDialog("Please input server.", this);
        }

        private void OnTestConnectionClick(object sender, RoutedEventArgs e)
        {
            if (IsSqlServer2005OrAbove())
                MessageBox.ShowDialog("Connection is successful.", this);
        }

        private bool IsSqlServer2005OrAbove()
        {
            try
            {
                var version = QueryEngine.GetServerVersion(GetServerInfo);
                var is2005OrAbove = version >= 9;
                if (!is2005OrAbove)
                    MessageBox.ShowDialog(string.Format("Current version {0}, only SQL Server 2005 or above is supported.", version), this);
                return is2005OrAbove;
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.Message, this);
                return false;
            }
        }


        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            //容器Grid
            Grid grid = this.Owner.Content as Grid;
            //父级窗体原来的内容
            UIElement original = VisualTreeHelper.GetChild(grid, 0) as UIElement;
            //将父级窗体原来的内容在容器Grid中移除
            grid.Children.Remove(original);
            //赋给父级窗体
            this.Owner.Content = original;
        }

        private void Window_Closed(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
