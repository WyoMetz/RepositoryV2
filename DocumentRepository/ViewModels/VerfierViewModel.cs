using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using MoonPdfLib;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace DocumentRepository.ViewModels
{
    public class VerfierViewModel : ObservableBase
    {
        public VerfierViewModel()
        {
            Task.Run(() => BuildLists());
        }

        private async void BuildLists()
        {
            IsVisible = true;
            UnitDiaries = await UnitDiary.GetUnconfirmedDiaries();
            Branches = await UnitDiary.GetBranches();
            diaries = UnitDiaries;
            IsVisible = false;
        }

        UnitDiary UnitDiary = new UnitDiary();
        IList<UnitDiary> diaries = new List<UnitDiary>();

        private IList<UnitDiary> unitDiaries;
        public IList<UnitDiary> UnitDiaries
        {
            get
            {
                return unitDiaries;
            }
            set
            {
                unitDiaries = value;
                OnPropertyChanged("UnitDiaries");
            }
        }

        private IList<string> branches;
        public IList<string> Branches
        {
            get
            {
                return branches;
            }
            set
            {
                branches = value;
                OnPropertyChanged("Branches");
            }
        }

        private MoonPdfPanel pdfPanel;
        public MoonPdfPanel PdfPanel
        {
            get
            {
                return pdfPanel;
            }
            set
            {
                pdfPanel = value;
                OnPropertyChanged("PdfPanel");
            }
        }

        private void setPdf()
        {
            MoonPdfPanel pdf = new MoonPdfPanel();
            if(SelectedDiary.UploadLocation != null)
            {
                pdf.OpenFile(SelectedDiary.UploadLocation);
                PdfPanel = pdf;
            }
        }

        private UnitDiary selectedDiary;
        public UnitDiary SelectedDiary
        {
            get
            {
                return selectedDiary;
            }
            set
            {
                selectedDiary = value;
                OnPropertyChanged("SelectedDiary");
                if(SelectedDiary != null)
                {
                   setPdf();
                }
            }
        }

        public ICommand NextPage
        {
            get { return new RelayCommand(execute => PdfPanel.GotoNextPage(), canExecute => PdfPanel != null); }
        }

        public ICommand PreviousPage
        {
            get { return new RelayCommand(execute => PdfPanel.GotoPreviousPage(), canExecute => PdfPanel != null); }
        }

        private string search;
        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                search = value;
                SearchDiaries();
                OnPropertyChanged("Search");
            }
        }

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                if (filter != null)
                {
                    SearchDiaries();
                }
                OnPropertyChanged("Filter");
            }
        }

        public async void SearchDiaries()
        {
            List<UnitDiary> tempDiaires = diaries.ToList();
            if (filter != null)
            {
                tempDiaires = tempDiaires.Where(x => x.Branch == filter).ToList();
            }
            UnitDiaries = tempDiaires.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
        }

        public ICommand AcceptUpload
        {
            get { return new RelayCommand(execute => Task.Run(() => AcceptUploadExecute()), canExecute => (SelectedDiary != null && IsVisible == false)); }
        }

        public ICommand RejectUpload
        {
            get { return new RelayCommand(execute => RejectUploadExecute(), canExecutef => (SelectedDiary != null && IsVisible == false)); }
        }

        public async Task AcceptUploadExecute()
        {
            IsVisible = true;
            IList<Transaction> transactions = await new Transaction().GetTransactions(SelectedDiary);
            await new ESRInsert().InsertTransactions(transactions);
            SelectedDiary.ConfirmUpload();
            Message = new SnackbarMessageQueue();
            Message.Enqueue($"Document for Diary {SelectedDiary.Number} has been Accepted.");
            UnitDiaries = await UnitDiary.GetUnconfirmedDiaries();
            IsVisible = false;
        }

        public async void RejectUploadExecute()
        {
            IsVisible = true;
            SelectedDiary.RejectUpload();
            Message = new SnackbarMessageQueue();
            Message.Enqueue($"Document for Diary {SelectedDiary.Number} has been Rejected.");
            UnitDiaries = await UnitDiary.GetUnconfirmedDiaries();
            IsVisible = false;
        }

        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        private SnackbarMessageQueue message;
        public SnackbarMessageQueue Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public ICommand GetReport
        {
            get { return new RelayCommand(execute => getReport()); }
        }

        private void getReport()
        {
            IList<IReportable> reportables = new List<IReportable>(UnitDiaries);
            new FileWriter().WriteReport(reportables);
        }
    }
}
