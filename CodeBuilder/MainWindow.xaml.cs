using CodeBuilder.Common;
using CodeBuilder.Logic;
using CodeBuilder.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeBuilder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //private void btnTest_Click(object sender, RoutedEventArgs e)
        //{
        //    var tableInfo = DbHelper.GetDbNewTable(ConfigurationManager.AppSettings["ConnStr"].ToString(), "NORTHWND", "Customers");
        //    string codeDataAccess = CreateCode.CreateDataAccessClass(tableInfo);
        //}

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

      

        public MainWindow()
        {
            InitializeComponent();

         
            btnSkin.Click += (s, e) => skinUI.IsOpen = true;
            btnLogin.Click += (s, e) => ConnectionDialog.ShowDialog(null, this);

            skinPanel.AddHandler(Button.ClickEvent, new RoutedEventHandler(ChangeSkin));
            InitSkins();

            ChangeTheme(ConfigurationManager.AppSettings["Theme"]);

            // Create some example nodes to play with
            var rootNode = new TreeItemViewModel(null, false) { DisplayName = "rootNode" };
            var node1 = new TreeItemViewModel(rootNode, false) { DisplayName = "element1 (editable)", IsEditable = true, Icon = ImageClass.Edit2 };
            var node2 = new TreeItemViewModel(rootNode, false) { DisplayName = "element2", Icon = ImageClass.History2 };
            var node11 = new TreeItemViewModel(node1, false) { DisplayName = "element11", Remarks = "Look at me!", Icon = ImageClass.Edit2 };
            var node12 = new TreeItemViewModel(node1, false) { DisplayName = "element12 (disabled)", IsEnabled = false, Icon = ImageClass.Edit2 };
            var node13 = new TreeItemViewModel(node1, false) { DisplayName = "element13", Icon = ImageClass.Edit2 };
            var node131 = new TreeItemViewModel(node13, false) { DisplayName = "element131" };
            var node132 = new TreeItemViewModel(node13, false) { DisplayName = "element132" };
            var node14 = new TreeItemViewModel(node1, false) { DisplayName = "element14 with colours" };
            var node15 = new TreeItemViewModel(node1, true) { DisplayName = "element15 (lazy loading)" };

            // Add them all to each other
            rootNode.Children.Add(node1);
            rootNode.Children.Add(node2);
            node1.Children.Add(node11);
            node1.Children.Add(node12);
            node1.Children.Add(node13);
            node13.Children.Add(node131);
            node13.Children.Add(node132);
            node1.Children.Add(node14);
            node1.Children.Add(node15);

            // Use the root node as the window's DataContext to allow data binding. The TreeView
            // will use the Children property of the DataContext as list of root tree items. This
            // property happens to be the same as each item DataTemplate uses to find its subitems.
            DataContext = rootNode;

            // Preset some node states
            node1.IsSelected = true;
            node13.IsSelected = true;
            node14.IsExpanded = true;
        }

        private static void ChangeTheme(string theme)
        {
            Accent accent = ThemeManager.GetAccent(theme);
            App.Current.Resources.MergedDictionaries.Last().Source = accent.Resources.Source;
        }

     

      

        private void TheTreeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
        {
          
                // Selection is not locked, apply other conditions.
                // Require all selected items to be of the same type. If an item of another data
                // type is already selected, don't include this new item in the selection.
                if (e.Selecting && TheTreeView.SelectedItems.Count > 0)
                {
                    e.CancelThis = e.Item.GetType() != TheTreeView.SelectedItems[0].GetType();
                }

            //if (e.Selecting)
            //{
            //    System.Diagnostics.Debug.WriteLine("Preview: Selecting " + e.Item + (e.Cancel ? " - cancelled" : ""));
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("Preview: Deselecting " + e.Item + (e.Cancel ? " - cancelled" : ""));
            //}
        }

        private void ClearChildrenButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selection = new object[TheTreeView.SelectedItems.Count];
            TheTreeView.SelectedItems.CopyTo(selection, 0);
            foreach (TreeItemViewModel node in selection)
            {
                if (node.Children != null)
                {
                    node.Children.Clear();
                }
            }
        }

        private void AddChildButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                if (!node.HasDummyChild)
                {
                    node.Children.Add(new TreeItemViewModel(node, false) { DisplayName = "newborn child" });
                    node.IsExpanded = true;
                }
            }
        }

        private void ExpandNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsExpanded = true;
            }
        }

        private void HideNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems.OfType<TreeItemViewModel>().ToArray())
            {
                node.IsVisible = false;
            }
        }

        private void ShowNodesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                DoShowAll(node, (n) => true);
            }
        }

        private void DoShowAll(TreeItemViewModel node, Func<TreeItemViewModel, bool> selector)
        {
            node.IsVisible = selector(node);
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DoShowAll(child, selector);
                }
            }
        }

        private void SelectNoneButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                DoSelectAll(node, (n) => false);
            }
        }

        private void SelectSomeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Random rnd = new Random();
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                DoSelectAll(node, (n) => rnd.Next(0, 2) > 0);
            }
        }

        private void SelectAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                DoSelectAll(node, (n) => true);
            }
        }

        private void ToggleSelectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.Items)
            {
                DoSelectAll(node, (n) => !n.IsSelected);
            }
        }

        private void DoSelectAll(TreeItemViewModel node, Func<TreeItemViewModel, bool> selector)
        {
            node.IsSelected = selector(node);
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    DoSelectAll(child, selector);
                }
            }
        }

        private void ExpandMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsExpanded = true;
            }
        }

        private void RenameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems)
            {
                node.IsEditing = true;
                break;
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeItemViewModel node in TheTreeView.SelectedItems.Cast<TreeItemViewModel>().ToArray())
            {
                node.Parent.Children.Remove(node);
            }
        }

    }
}
