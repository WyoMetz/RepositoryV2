using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RepositoryDeployment.ViewModels
{
    public class HelperViewModel: ObservableBase
    {
        public HelperViewModel()
        {
            GetMaxPages();
            PageIndex = 0;
            MoveNext();
        }

        private int PageIndex { get; set; }
        private int MaxPages { get; set; }

        #region Logic


        private string GetText()
        {
            string text;
            using(StreamReader reader = new StreamReader($@".\HelperResources\Page{PageIndex}.txt"))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }

        private void GetMaxPages()
        {
            string line;
            string[] values;
            using (StreamReader reader = new StreamReader($@".\HelperResources\PageIndex.txt"))
            {
                line = reader.ReadLine();
            }
            values = line.Split(';');
            MaxPages = int.Parse(values[1]);
        }

        private BitmapImage GetImage()
        {
            BitmapImage image = new BitmapImage(new Uri(Path.GetFullPath($@".\HelperResources\Image{PageIndex}.png")));
            return image;
        }

        private void MoveNext()
        {
            PageIndex++;
            HelpText = GetText();
            HelpImage = GetImage();
        }

        private void MoveBack()
        {
            PageIndex--;
            HelpText = GetText();
            HelpImage = GetImage();
        }

        private bool CanNext()
        {
            if(PageIndex == MaxPages)
            {
                return false;
            }
            return true;
        }

        private bool CanBack()
        {
            if(PageIndex != 1)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Objects

        private string helpText;
        public string HelpText
        {
            get
            {
                return helpText;
            }
            set
            {
                helpText = value;
                OnPropertyChanged("HelpText");
            }
        }

        private BitmapImage helpImage;
        public BitmapImage HelpImage
        {
            get
            {
                return helpImage;
            }
            set
            {
                helpImage = value;
                OnPropertyChanged("HelpImage");
            }
        }

        #endregion

        #region Commands

        public ICommand Next
        {
            get { return new RelayCommand(execute => MoveNext(), canExecute => CanNext()); }
        }

        public ICommand Back
        {
            get { return new RelayCommand(execute => MoveBack(), canExecute => CanBack()); }
        }

        #endregion
    }
}
