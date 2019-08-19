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

namespace DocumentRepository
{
    /// <summary>
    /// Interaction logic for DocumentQuickView.xaml
    /// </summary>
    public partial class DocumentQuickView : Window
    {
        public DocumentQuickView(string PdfPath)
        {
            InitializeComponent();
            OpenPdf(PdfPath);
        }

        public void OpenPdf(string FilePath)
        {
            if(FilePath.ToUpper().EndsWith(".PDF"))
            PdfViewer.OpenFile(FilePath);
        }

        public void nextPage()
        {
            PdfViewer.GotoNextPage();
        }

        public void previousPage()
        {
            PdfViewer.GotoPreviousPage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            previousPage();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            nextPage();
        }
    }
}
