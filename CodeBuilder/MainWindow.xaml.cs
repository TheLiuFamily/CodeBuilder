﻿using CodeBuilder.Common;
using CodeBuilder.Logic;
using CodeBuilder.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region 常量
        private const string KeyTables = "|Tables";
        private const string KeySPs = "|SPs";
        private const string KeyViews = "|Views";
        private const string KeyFunctions = "|Functions";
        private const string KeyAssemblies = "|Assemblies";
        private const string KeyTriggers = "|Triggers";
        private const string KeyIndexes = "|Indexes";
        private const string KeyJobs = "|Jobs";
        private const string KeyTable = "|Table";
        private const string KeySp = "|SP";
        private const string KeyView = "|View";
        private const string KeyFunction = "|Function";
        private const string KeyTrigger = "|Trigger";
        private const string KeyAssembly = "|Assembly";
        private const string KeyDatabase = "|Database";
        private const string KeyServer = "|Server";
        private const string KeyLoading = "|Loading";
        private const string KeyName = "Name";
        private const string KeySchemaName = "SchemaName";
        private const string KeyState = "State";
        private const string KeySpaceUsed = "SpaceUsed";
        private const string KeyCount = "Count";
        private const string KeyCreateDate = "CreateDate";
        private const string KeyModifyDate = "ModifyDate";
        private const string KeyPath = "Path";
        private const string KeyValue = "Value";
        private const string KeyType = "Type";
        private const string KeyText = "Text";
        private const string CommandRefresh = "tbRefresh";
        private const string CommandActivityStatuses = "tcbActivityStatuses";
        private const string CommandJobs = "Jobs";
        private const string CommandProcesses = "Processes";
        private const string CommandDelete = "Delete";
        private const string CommandVisualize = "Visualize";
        private const int ResultSamplePrefix = 15;
        private const string AnalysisColumnRule = "Rule";
        private const string AnalysisColumnObject = "Object";
        private const string AnalysisColumnReference = "Reference";
        private const string AnalysisColumnCurrent = "Current";
        private const string AnalysisColumnFactor = "Factor";
        private const string AnalysisColumnSuggestion = "Suggestion";
        private const string SizePercentage = "%";
        private const int ImageIndexOnline = 5;

        private const int HealthIndexCagtegory = 0;
        private const int HealthIndexName = 1;
        private const int HealthIndexCurrent = 2;
        private const int HealthIndexReference = 3;
        private const int HealthIndexDescription = 4;
        private const int HealthIndexObject = 5;

        private WorkModes _currentWorkMode = WorkModes.Summary;

        private Timer _tmrActivitiesRefresh;
        private readonly Timer _tmrStartup;
        private bool _isUpdating;
        private ObjectModes _currentObjectMode = ObjectModes.None;
        private ObjectModes _previousObjectMode = ObjectModes.None;
        private string _currentDatabase = string.Empty;
        private string _currentObjectScript = string.Empty;
        private string _currentObjectName = string.Empty;
        private string _currentObjectType = string.Empty;
        private string _previousDatabase = string.Empty;
        private string _previousObjectType = string.Empty;
        private int _userQueryCount;
        private bool _isInSearch;
        private bool _isSearching;
        private int _currentSearchIndex;
        private ServerInfo _currentServerInfo = new ServerInfo();
        private ServerInfo _previousServerInfo = new ServerInfo();
        private MonitorItem _currentMonitorItem;
        private int _healthPrevColIndex = -1;
        private ListSortDirection _healthPrevSortDirection = ListSortDirection.Ascending;
        private int _analysisPrevColIndex = -1;
        private ListSortDirection _analysisPrevSortDirection = ListSortDirection.Ascending;
        #endregion
        //private void btnTest_Click(object sender, RoutedEventArgs e)
        //{
        //    var tableInfo = DbHelper.GetDbNewTable(ConfigurationManager.AppSettings["ConnStr"].ToString(), "NORTHWND", "Customers");
        //    string codeDataAccess = CreateCode.CreateDataAccessClass(tableInfo);
        //}
        private readonly ServerInfo _server = null;
        private ServerInfo info;
        /// <summary>  
        /// 实现换肤  
        /// </summary>  
        private void ChangeSkin(object obj, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                var theme = (e.OriginalSource as Button).Name;
                ChangeTheme(theme);

                //记录换肤，下次启动直接使用
                Configuration cf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cf.AppSettings.Settings["Theme"].Value = theme;
                cf.Save();
            }
        }

        /// <summary>  
        /// 初始化所有皮肤控件  
        /// </summary>  
        private void InitSkins()
        {
            var accents = ThemeManager.Accents;
            Style btnStyle = App.Current.FindResource("btnSkinStyle") as Style;
            foreach (var accent in accents)
            {
                //新建换肤按钮  
                Button btnskin = new Button();
                btnskin.Style = btnStyle;
                btnskin.Name = accent.Name;
                SolidColorBrush scb = accent.Resources["AccentColorBrush"] as SolidColorBrush;
                btnskin.Background = scb;
                skinPanel.Children.Add(btnskin);
            }
        }
        public void LoginSql(object sender, RoutedEventArgs e)
        {
            var dlg = new ConnectionDialog(null, this);
            if (dlg.UShowDialog() == true)
            {
                var server = dlg.Server;
                ServerInfo item;
                if (info == null)
                    item = Settings.Instance.FindServer(server, dlg.UserName);
                else
                    item = Settings.Instance.FindServer(info.Server, dlg.UserName);
                var isNew = false;
                if (item == null)
                {
                    item = new ServerInfo();
                    Settings.Instance.Servers.Add(item);
                    isNew = true;
                }
                item.AuthType = dlg.AuthType;
                item.Server = server;
                item.Password = dlg.Password;
                item.User = dlg.UserName;
                Settings.Instance.Save();
                if (info != null)
                {
                    info.AuthType = item.AuthType;
                    info.Password = item.Password;
                    info.Server = item.Server;
                    info.User = item.User;
                }
                if (isNew)
                    LoadServer(item);
                else if (info == null)
                    MessageBox.ShowDialog(string.Format("Server [{0}] already exists", item.Server), this);
                else
                {
                    foreach (TreeNode node in TheTreeView.Items)
                    {
                        if (node.DisplayName == item.Server)
                        {
                            node.IsSelected = true;
                        }
                    }
                }
            }
        }
        private void LoadServer(ServerInfo info)
        {
            _currentServerInfo = info;
            var state = new ServerState { IsAzure = info.IsAzure, AuthType = info.AuthType, Server = info.Server, Database = info.Database, User = info.User, Password = info.Password, IsReady = false, Key = KeyServer };
            var treeNode = new TreeNode(null, false) { DisplayName = "RootNode", Tag = state };
            var node = new TreeNode(treeNode, false) { DisplayName = info.Server, Tag = state, Icon = ImageClass.Server2 };
            var node1 = new TreeNode(node, false) { DisplayName = "Loading...", Tag = new ServerState { Key = KeyLoading } };
            treeNode.Children.Add(node);
            node.Children.Add(node1);
            DataContext = treeNode;
        }

        public MainWindow()
        {
            InitializeComponent();


            btnSkin.Click += (s, e) => skinUI.IsOpen = true;
            btnLogin.Click += LoginSql;

            skinPanel.AddHandler(Button.ClickEvent, new RoutedEventHandler(ChangeSkin));
            InitSkins();

            ChangeTheme(ConfigurationManager.AppSettings["Theme"]);

            if(Settings.Instance.Servers.FirstOrDefault() != null)
            {
                LoadServer(Settings.Instance.Servers.First());
            }
        }

        private static void ChangeTheme(string theme)
        {
            Accent accent = ThemeManager.GetAccent(theme);
            App.Current.Resources.MergedDictionaries.Last().Source = accent.Resources.Source;
        }





        private void TheTreeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
        {
            new Thread(ShowObjects).Start(e);
        }

        private void ShowObjects(object e)
        {
            this.BeginInvoke(new Action(delegate ()
            {
                var arg = e as PreviewSelectionChangedEventArgs;
                var item = arg.Item as TreeNode;
                var state = item.Tag as ServerState;
                if (item == null || (state != null && !state.IsReady))
                    arg.CancelThis = !ShowObjects(item);
            }));
        }
        private TreeNode GetRootNode(TreeNode node)
        {
            var root = node;
            while (root != null && root.Parent != null)
            {
                root = root.Parent;
            }
            return root;
        }
        internal ServerInfo CurrentServerInfo
        {
            get { return _currentServerInfo; }
        }

        private DataTable GetDatabaseInfo(string database)
        {
            return QueryEngine.GetDatabaseInfo(DefaultServerInfo, database);
        }

        private bool CheckCurrentServer()
        {
            return CurrentServerInfo != null && !string.IsNullOrEmpty(CurrentServerInfo.Server);
        }
        private SqlConnection NewConnection
        {
            get { return SqlHelper.CreateNewConnection(DefaultServerInfo); }
        }

        private bool LoadServer(TreeNode node)
        {
            if (CheckCurrentServer())
            {
                _currentDatabase = string.Empty;
                try
                {
                    var result = SqlHelper.ExecuteScalar("SELECT @@version", DefaultServerInfo);
                    var version = result != null ? result.ToString() : "(N/A)";
                    var lines = version.Split('\t').ToList();
                    if (lines.Count > 1)
                    {
                        var line = lines[1];
                        DateTime date;
                        if (DateTime.TryParse(line, out date))
                        {
                            lines.RemoveAt(1);
                        }
                    }
                    var serverState = node.Tag as ServerState;
                    serverState.IsAzure = lines[0].IndexOf("Azure", StringComparison.InvariantCultureIgnoreCase) != -1;

                    result = SqlHelper.ExecuteScalar(string.Format("SELECT {0}", serverState.IsAzure ? "@@SERVERNAME" : "@@SERVICENAME"), DefaultServerInfo);
                    var serviceName = result != null ? result.ToString() : "(N/A)";

                    result = SqlHelper.ExecuteScalar("SELECT ServerProperty('ProcessID')", DefaultServerInfo);
                    var processId = result != null ? result.ToString() : "(N/A)";

                    using (var connection = NewConnection)
                    {
                        connection.Open();
                        var data = connection.GetSchema("Databases");
                        node.Children.Clear();
                        var databases = GetDatabasesInfo();
                        data.AsEnumerable().OrderBy(r => r.Field<string>("database_name")).ForEach((d) =>
                        {
                            var name = d["database_name"].ToString();
                            var info = GetDatabaseInfo(name);
                            if (info != null && info.Rows.Count > 0)
                            {
                                var row = info.Rows[0];
                                var state = databases.AsEnumerable().First(r => r["name"].ToString() == name);
                                var isReady = state != null && Convert.ToInt32(state["state"]) == 0;
                                var image = isReady ? ImageIndexOnline : 0;
                                var tag = new ServerState { Key = KeyDatabase, IsReady = false, State = isReady };
                                var databaseNode = new TreeNode(node, false) { DisplayName = name, Text = name, Icon = ImageClass.Server, Tag = tag };
                                node.Children.Add(databaseNode);
                            }
                        });
                        node.Children.Cast<TreeNode>().ForEach((n) =>
                        {
                            n.Children.Add(new TreeNode(n, false) { Text = KeyTables, DisplayName = "Tables", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeyTables, IsReady = false } });
                            n.Children.Add(new TreeNode(n, false) { Text = KeyViews, DisplayName = "Views", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeyViews, IsReady = false } });
                            n.Children.Add( new TreeNode(n, false) { Text = KeyFunctions, DisplayName = "Functions", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeyFunctions, IsReady = false } });
                            n.Children.Add( new TreeNode(n, false) { Text = KeySPs, DisplayName = "Stored Procedures", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeySPs, IsReady = false } });
                            n.Children.Add( new TreeNode(n, false) { Text = KeyAssemblies, DisplayName = "Assemblies", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeyAssemblies, IsReady = false } });
                            n.Children.Add( new TreeNode(n, false) { Text = KeyTriggers, DisplayName = "Triggers", Icon = ImageClass.Folder, Tag = new ServerState { Key = KeyTriggers, IsReady = false } }   );
                        });
                        connection.Close();
                    }
                    //LoadDatabase(Node);
                    var counts = node.Children.Count;

                    _currentServerInfo.IsAzure = serverState.IsAzure;

                    if (counts == 1 && (node.Children[0].Tag as ServerState).Key == KeyLoading)
                    {
                        node.IsExpanded = false;
                        return false;
                    }
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    node.IsExpanded = false;
                    MessageBox.ShowDialog(ex.Message, this);
                    return false;
                }
            }
            else
            {
                node.IsExpanded = false;
                return false;
            }
        }

        private ServerInfo GetServerInfo(string catalog)
        {
            return QueryEngine.GetServerInfo(CurrentServerInfo, catalog);
        }

        private ServerInfo DefaultServerInfo
        {
            get { return GetServerInfo(string.Empty); }
        }


        private DataTable GetDatabasesInfo()
        {
            return QueryEngine.GetDatabasesInfo(DefaultServerInfo);
        }

        internal DataTable Query(string sql)
        {
            return Query(sql, DefaultServerInfo);
        }
        internal DataTable Query(string sql, ServerInfo info)
        {
            var data = QuerySet(sql, info);
            if (data != null && data.Tables.Count > 0)
                return data.Tables[0];
            else
                return null;
        }
        private DataSet QuerySet(string sql, ServerInfo info)
        {
            //using (NewWait())
            {
                try
                {
                    return SqlHelper.QuerySet(sql, info);
                }
                catch (Exception ex)
                {
                    MessageBox.ShowDialog(ex.Message, this);
                    return null;
                }
            }
        }
        private DataTable GetObjects(string objectType)
        {
            string types;
            switch (objectType)
            {
                case KeyTriggers:
                    return Query("SELECT '' AS SchemaName, name, create_date AS CreateDate, modify_date AS ModifyDate, type FROM sys.triggers WITH (NOLOCK) WHERE parent_class = 0", CurrentServerInfo);
                default:
                    switch (objectType)
                    {
                        case KeyTables:
                            types = "'U'";
                            break;
                        case KeyViews:
                            types = "'V'";
                            break;
                        case KeyFunctions:
                            types = "'FN', 'IF', 'TF'";
                            break;
                        case KeySPs:
                            types = "'P'";
                            break;
                        default:
                            types = string.Empty;
                            break;
                    }
                    var filter = !string.IsNullOrEmpty(types) ? " WHERE so.type IN (" + types + ")" : string.Empty;
                    return GetObjectsFilter(filter);
            }
        }
        private DataTable GetObjectsFilter(string filter)
        {
            return Query(string.Format("SELECT su.name AS SchemaName, so.name, so.create_date AS CreateDate, so.modify_date AS ModifyDate, so.type FROM sys.objects so WITH (NOLOCK) LEFT JOIN sys.schemas su WITH (NOLOCK) ON so.schema_id = su.schema_id {0} ORDER BY su.name, so.name", filter), CurrentServerInfo);
        }
        private bool ShowObjects(TreeNode node)
        {
            var ready = false;
            DataTable data;
            var key = node.Text;
            var root = GetRootNode(node);
            var serverState = node.Tag as ServerState;
            _previousServerInfo = _currentServerInfo;
            _currentServerInfo = root.Tag as ServerState;
            switch (serverState.Key)
            {
                case KeyServer:
                    ready = LoadServer(node);
                    break;
                case KeyDatabase:
                    _currentDatabase = node.Text;
                    _currentServerInfo.Database = _currentDatabase;
                    var databases = GetDatabasesInfo();
                    var state = databases.AsEnumerable().First(r => r["name"].ToString() == _currentDatabase);
                    if (state != null && Convert.ToInt32(state["state"]) == 0)
                    {
                        var objects = new string[] { KeyTables, KeyViews, KeyFunctions, KeySPs, KeyTriggers };
                        objects.ForEach(o =>
                        {
                            data = GetObjects(o);
                            data.AsEnumerable().ForEach((d) =>
                            {
                                var icon = ImageClass.List2;
                                var type = string.Empty;
                                switch (o)
                                {
                                    case KeyTables:
                                        type = KeyTable;
                                        icon = ImageClass.Table2;
                                        break;
                                    case KeyViews:
                                        type = KeyView;
                                        icon = ImageClass.List2;
                                        break;
                                    case KeySPs:
                                        type = KeySp;
                                        icon = ImageClass.Gear2;
                                        break;
                                    case KeyFunctions:
                                        type = KeyFunction;
                                        icon = ImageClass.Gear2;
                                        break;
                                    case KeyAssemblies:
                                        type = KeyAssembly;
                                        icon = ImageClass.Gear2;
                                        break;
                                    case KeyTriggers:
                                        type = KeyTrigger;
                                        icon = ImageClass.Gear2;
                                        break;
                                    default:
                                        break;
                                }
                                var temp = node.Children.First(q => q.Text == o);
                                var tag = new ServerState { Key = type, IsReady = false };
                                var child = new TreeNode(temp, false) { DisplayName = QueryEngine.GetObjectName(d[KeySchemaName].ToString(), d[KeyName].ToString()),
                                    Icon = icon, Tag = tag };
                                temp.Children.Add(child);
                            });
                        });
                        ready = true;
                    }
                    else
                    {

                    }
                    //else if (ShowQuestion(string.Format("The database [{0}] is currently offline. Do you bring it back to online?", _currentDatabase)))
                    //{
                    //    SetOnlineOffline(_currentDatabase, true);
                    //    ShowObjects(node);
                    //}
                    break;
                default:
                    ready = true;
                    break;
            }
            serverState.IsReady = ready;
            return ready;
        }
    }
}
