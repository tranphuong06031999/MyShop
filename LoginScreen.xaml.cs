using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using MyShop.Custom;

namespace MyShop
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var Version = Assembly.GetExecutingAssembly().GetName().Version;

            version.Content = $"v{Version}";
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnLogin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        public void btnCommands_Click(object sender, RoutedEventArgs e)
        {
            Button curButton = sender as Button;
            if (curButton.Tag.Equals("btnClose"))
            {
                this.Close();
            }
            else if (curButton.Tag.Equals("btnMinim"))
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (curButton.Tag.Equals("btnMaxim"))
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }
        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu chưa nhập username hay password
            if (tbUser.Text.Length == 0 && tbPass.Password.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Please enter your account and password" };
                dialog.Sounds();
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }
            else if (tbUser.Text.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Please enter your account" };
                dialog.Sounds();
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }
            else if (tbPass.Password.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Please enter a password" };
                dialog.Sounds();
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            // Nếu thông tin tài khoản nhập vào sai
            if (!tbUser.Text.Equals("admin") || !tbPass.Password.Equals("admin"))
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Account or password is incorrect" };
                dialog.Sounds();
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            //Nếu đăng nhập thành công, ẩn màn hình login
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Close();
        }
    }
}
