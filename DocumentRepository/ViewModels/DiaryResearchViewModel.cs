using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Repository;

namespace DocumentRepository.ViewModels
{
    public class DiaryResearchViewModel : ObservableBase
    {
        public DiaryResearchViewModel()
        {
            Task.Run(() => BuildLists());
            IsVisible = true;
        }

        private async void BuildLists()
        {
            DiaryList = await Diary.GetUploaded();
            MissingDiaries = DiaryList;
            BranchList = await Diary.GetBranches();
            Branches = BranchList;
            IsVisible = false;
        }

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
            if(selectedTransaction != null)
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
                if (filter != null)
                {
                    SearchDiaries();
                }
                OnPropertyChanged("Filter");
            }
        }

        public async void SearchDiaries()
        {
            List<UnitDiary> tempDiaires = DiaryList.ToList();
            if (filter != null)
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
            if (SelectedDiary != null)
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
            new DocumentQuickView(SelectedDiary.CurrentFilePath).Show();
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
    }
}
