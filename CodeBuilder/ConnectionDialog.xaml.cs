using CodeBuilder.Common;
using CodeBuilder.Logic;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    public partial class ConnectionDialog : CustomDialog
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

        public bool Status { get; set; }

        private ConnectionDialog()
        {
            InitializeComponent();

            Enum.GetValues(typeof(AuthTypes)).Cast<AuthTypes>().ForEach((s) => cboAuthTypes.Items.Add(s));
            cboAuthTypes.SelectedIndex = 0;
        }
        private MetroWindow _window;
        /// <summary>
        /// 用于保存弹出框父类
        /// </summary>
        public ConnectionDialog(MetroWindow window, ServerInfo info)
            : this()
        {
            _window = window;
            if (info != null)
            {
                AuthType = info.AuthType;
                Server = info.Server;
                UserName = info.User;
                Password = info.Password;
                AuthType = info.AuthType;
            }

        }

        private async void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Server))
            {
                if (!string.IsNullOrEmpty(UserName) || AuthType == AuthTypes.Windows)
                {
                    if (IsSqlServer2005OrAbove())
                    {
                        Status = true;
                        await _window.HideMetroDialogAsync(this);
                    }
                }
                else
                {
                    await _window.ShowMessageAsync("Tips", "Please input user name.");
                    Status = false;
                }
            }
            else
            {
                await _window.ShowMessageAsync("Tips", "Please input server.");
            }
        }

        private async void OnTestConnectionClick(object sender, RoutedEventArgs e)
        {
            if (IsSqlServer2005OrAbove())
            {
                await _window.ShowMessageAsync("Tips", "Connection is successful.");
            }
        }

        private bool IsSqlServer2005OrAbove()
        {
            try
            {
                var version = QueryEngine.GetServerVersion(GetServerInfo);
                var is2005OrAbove = version >= 9;
                if (!is2005OrAbove)
                {
                    _window.ShowMessageAsync("Tips", string.Format("Current version {0}, only SQL Server 2005 or above is supported.", version));
                }
                return is2005OrAbove;
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync("Tips", ex.Message);
                return false;
            }
        }

        private async void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Status = false;
            await _window.HideMetroDialogAsync(this);
        }
    }
}
