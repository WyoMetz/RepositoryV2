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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocumentRepository
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckWindowState()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Maximize.Visibility = Visibility.Collapsed;
                Restore.Visibility = Visibility.Visible;
            }
            if (this.WindowState == WindowState.Normal)
            {
                Maximize.Visibility = Visibility.Visible;
                Restore.Visibility = Visibility.Collapsed;
            }
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            CheckWindowState();
        }

        private void Minimize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Restore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            Restore.Visibility = Visibility.Collapsed;
            Maximize.Visibility = Visibility.Visible;
        }

        private void Maximize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            Maximize.Visibility = Visibility.Collapsed;
            Restore.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void DiaryUpload_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DiaryUpload());
        }

        private void DocumentUpload_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DocumentUpload());
        }

        private void DiaryResearch_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DiaryResearch());
        }

        private void MemberResearch_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.MemberResearch());
        }

        private void DocumentResearch_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DocumentResearch());
        }

        private void TransResearch_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.TransactionResearch());
        }

        private void OverallStats_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.VerifierView());
        }
    }
}
