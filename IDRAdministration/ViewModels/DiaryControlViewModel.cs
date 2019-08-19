using MaterialDesignThemes.Wpf;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IDRAdministration.ViewModels
{
    public class DiaryControlViewModel : ObservableBase
    {
        public DiaryControlViewModel()
        {
            Task.Run(() => BuildLists());
            IsVisible = true;
        }

        private async void BuildLists()
        {
            Arucs = await new Database().GetArucs();
            Years = await new Database().GetYears();
            CurrentArucs = Arucs;
            CurrentYears = Years;
            AppSettings.Aruc = Arucs.FirstOrDefault();
            AppSettings.Year = DateTime.Now.Year;
            CurrentAruc = Arucs.FirstOrDefault();
            CurrentYear = DateTime.Now.Year;
            DiaryList = await Diary.GetUploaded();
            MissingDiaries = DiaryList;
            BranchList = await Diary.GetBranches();
            Branches = BranchList;
            IsVisible = false;
        }

        IList<int> Arucs = new List<int>();
        IList<int> Years = new List<int>();
        UnitDiary Diary = new UnitDiary();
        private IList<UnitDiary> DiaryList = new List<UnitDiary>();
        private IList<string> BranchList = new List<string>();

        #region Diaries&Transactions

        private IList<UnitDiary> missingDiaries;
        public IList<UnitDiary> MissingDiaries
        {
            get
            {
                return missingDiaries;
            }
            set
            {
                missingDiaries = value;
                OnPropertyChanged("MissingDiaries");
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
                if (selectedDiary != null)
                {
                    Task.Run(() => SetTransactions());
                    IsVisible = true;
                }
                OnPropertyChanged("SelectedDiary");
            }
        }

        public async void SetTransactions()
        {
            Transactions = await new Transaction().GetTransactions(selectedDiary);
            SetCart();
            IsVisible = false;
        }

        private IList<Transaction> transactions;
        public IList<Transaction> Transactions
        {
            get
            {
                return transactions;
            }
            set
            {
                transactions = value;
                OnPropertyChanged("Transactions");
            }
        }

        private Transaction selectedTransaction;
        public Transaction SelectedTransaction
        {
            get
            {
                return selectedTransaction;
            }
            set
            {
                selectedTransaction = value;
                GetErrorCode();
                OnPropertyChanged("SelectedTransaction");
            }
        }

        private async void GetErrorCode()
        {
            if (selectedTransaction != null)
                selectedTransaction.ErrorDescription = await new Transaction().GetErrorDescription(selectedTransaction);
        }

        private string documentname;
        public string DocumentName
        {
            get
            {
                return documentname;
            }
            set
            {
                documentname = value;
                OnPropertyChanged("DocumentName");
            }
        }

        #endregion

        #region SEARCH

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
                if(filter != null)
                {
                    SearchDiaries();
                }
                OnPropertyChanged("Filter");
            }
        }

        public async void SearchDiaries()
        {
            List<UnitDiary> tempDiaires = DiaryList.ToList();
            if(filter != null)
            {
                tempDiaires = tempDiaires.Where(x => x.Branch == filter).ToList();
            }
            MissingDiaries = tempDiaires.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
        }

        #endregion

        #region CART

        public int cycle;
        public int Cycle
        {
            get
            {
                return cycle;
            }
            set
            {
                cycle = value;
                OnPropertyChanged("Cycle");
            }
        }

        private int accepted;
        public int Accepted
        {
            get
            {
                return accepted;
            }
            set
            {
                accepted = value;
                OnPropertyChanged("Accepted");
            }
        }

        private int rejected;
        public int Rejected
        {
            get
            {
                return rejected;
            }
            set
            {
                rejected = value;
                OnPropertyChanged("Rejected");
            }
        }

        private int total;
        public int Total
        {
            get
            {
                return total;
            }
            set
            {
                total = value;
                OnPropertyChanged("Total");
            }
        }

        #endregion

        #region Commands&Logic

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

        public void SetCart()
        {
            Cycle = SelectedDiary.CycleNumber;
            Accepted = Transactions.Where(x => x.TransactionErrorCode == string.Empty).ToList().Count;
            Rejected = Transactions.Where(x => x.TransactionErrorCode != string.Empty).ToList().Count;
            Total = Transactions.Count;
        }

        public ICommand SelectDocument
        {
            get { return new RelayCommand(execute => ExecuteSelect(), canExecute => CanExecuteSelect()); }
        }

        public void ExecuteSelect()
        {
            SelectedDiary.CurrentFilePath = new FileOperation().ChooseFile();
            DocumentName = Path.GetFileName(SelectedDiary.CurrentFilePath);
        }

        public bool CanExecuteSelect()
        {
            if(SelectedDiary != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand UploadDocument
        {
            get { return new RelayCommand(execute => ExecuteUpload(), canExecute => CanExecuteUpload()); }
        }

        public async void ExecuteUpload()
        {
            SelectedDiary.UploadLocation = await new FileOperation().CopyFile(SelectedDiary);
            new Database().UpdateEntry(SelectedDiary);
            MissingDiaries.Remove(SelectedDiary);
            Message = new SnackbarMessageQueue();
            Message.Enqueue($"Document for Diary {SelectedDiary.Number} has been uploaded.");
            SelectedDiary = null;
            DocumentName = null;
        }

        public bool CanExecuteUpload()
        {
            if (SelectedDiary != null)
            {
                if (SelectedDiary.CurrentFilePath != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public ICommand ViewDocument
        {
            get { return new RelayCommand(execute => OpenDocument(), canExecute => CanExecuteUpload()); }
        }

        public void OpenDocument()
        {
            //new DocumentQuickView(SelectedDiary.CurrentFilePath).Show();
        }

        public ICommand ResetDocument
        {
            get { return new RelayCommand(execute => Reset()); }
        }

        public async void Reset()
        {
            MessageBoxResult callResult = MessageBox.Show($"Are you sure you want to reset Diary Number {SelectedDiary.Number}?\nThis cannot be undone once confirmed.", "Confirm Diary Reset", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            if(callResult == MessageBoxResult.Yes)
            {
                new Database().DeleteEntry(SelectedDiary);
                DiaryList = await Diary.GetUploaded();
                MissingDiaries = DiaryList;
            }
        }

        public ICommand ViewDoc
        {
            get { return new RelayCommand(execute => ExecuteView(), canExecute => CanExecuteView()); }
        }

        public void ExecuteView()
        {
            new FileOperation().OpenDocument(SelectedDiary);
        }

        public bool CanExecuteView()
        {
            if (SelectedDiary != null)
            {
                if (!string.IsNullOrEmpty(SelectedDiary.UploadLocation))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public ICommand GetReport
        {
            get { return new RelayCommand(execute => GetMissingReport()); }
        }

        private void GetMissingReport()
        {
            IList<IReportable> report = new List<IReportable>(MissingDiaries);
            new FileWriter().WriteReport(report);
        }

        #endregion

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

        private IList<int> currentYears;
        public IList<int> CurrentYears
        {
            get
            {
                return currentYears;
            }
            set
            {
                currentYears = value;
                OnPropertyChanged("CurrentYears");
            }
        }

        private IList<int> currentArucs;
        public IList<int> CurrentArucs
        {
            get
            {
                return currentArucs;
            }
            set
            {
                currentArucs = value;
                OnPropertyChanged("CurrentArucs");
            }
        }

        private int currentYear;
        public int CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                currentYear = value;
                AppSettings.Year = value;
                SetDiaries();
                OnPropertyChanged("CurrentYear");
            }
        }

        private int currentAruc;
        public int CurrentAruc
        {
            get
            {
                return currentAruc;
            }
            set
            {
                currentAruc = value;
                AppSettings.Aruc = value;
                SetDiaries();
                OnPropertyChanged("CurrentAruc");
            }
        }

        private async void SetDiaries()
        {
            MissingDiaries = await Diary.GetUploaded();
        }
    }
}
