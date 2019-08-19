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

namespace IDRAdministration
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

        private void NewCycle_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.NewCycleView());
        }

        private void DocumentControl_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DocumentControlView());
        }

        private void DiaryReset_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.DiaryControlView());
        }

        private void UserAccount_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.UserAccountsView());
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Views.Reports());
        }
    }
}
