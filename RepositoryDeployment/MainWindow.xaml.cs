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
using RepositoryDeployment.Views;

namespace RepositoryDeployment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationHelper.Navigation = ContentFrame.NavigationService;
            ContentFrame.Navigate(new InformationView());
        }

        private void ContentFrame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            UpdateFrameData();
        }

        private void ContentFrame_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateFrameData();
        }

        private void UpdateFrameData()
        {
            var content = ContentFrame.Content as FrameworkElement;
            if (content == null)
                return;
            content.DataContext = ContentFrame.DataContext;
        }
    }
}
