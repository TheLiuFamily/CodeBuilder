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
    public partial class MessageBox : Window
    {
        private MessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 弹出消息框
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="owner">父级窗体</param>
        public static void ShowDialog(string message, Window owner)
        {
            //蒙板
            Grid layer = new Grid() { Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)) };
            //父级窗体原来的内容
            UIElement original = owner.Content as UIElement;
            owner.Content = null;
            //容器Grid
            Grid container = new Grid();
            container.Children.Add(original);//放入原来的内容
            container.Children.Add(layer);//在上面放一层蒙板
            //将装有原来内容和蒙板的容器赋给父级窗体
            owner.Content = container;

            //弹出消息框
            MessageBox box = new MessageBox() { Owner = owner };
            box.tbc_message.Text = message;
            box.ShowDialog();
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
    }
}
