using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentRepository.ViewModels
{
    public class TransactionResearchViewModel : ObservableBase
    {
        public TransactionResearchViewModel()
        {
            Task.Run(() => BuildLists());
        }

        private async void BuildLists()
        {
            Marines = await marine.GetMarines();
            TransactionSearch = transaction;
            TransactionList = Transactions;
            BranchList = await diary.GetBranches();
            Branches = BranchList;
        }

        #region Objects

        UnitDiary diary = new UnitDiary();
        Marine marine = new Marine();
        Transaction transaction = new Transaction();

        IList<Marine> Marines = new List<Marine>();
        IList<Transaction> Transactions = new List<Transaction>();
        IList<string> BranchList = new List<string>();

        private Marine selectedMarine;
        public Marine SelectedMarine
        {
            get
            {
                return selectedMarine;
            }
            set
            {
                selectedMarine = value;
                Task.Run(() => SetMarineTransactions());
                IsVisible = true;
                OnPropertyChanged("SelectedMarine");
            }
        }

        private Transaction transactionSearch;
        public Transaction TransactionSearch
        {
            get
            {
                return transactionSearch;
            }
            set
            {
                transactionSearch = value;
                OnPropertyChanged("TransactionSearch");
            }
        }

        private int resultNumber;
        public int ResultNumber
        {
            get
            {
                return resultNumber;
            }
            set
            {
                resultNumber = value;
                OnPropertyChanged("ResultNumber");
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
                GetErrorDescription();
                OnPropertyChanged("SelectedTransaction");
            }
        }

        #endregion

        #region Lists

        private IList<Marine> marineList;
        public IList<Marine> MarineList
        {
            get
            {
                return marineList;
            }
            set
            {
                marineList = value;
                OnPropertyChanged("MarineList");
            }
        }

        private IList<Transaction> transactionList;
        public IList<Transaction> TransactionList
        {
            get
            {
                return transactionList;
            }
            set
            {
                transactionList = value;
                ResultNumber = value.Count;
                OnPropertyChanged("TransactionList");
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

        #endregion

        #region Logic

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
                RunSearch();
                OnPropertyChanged("Search");
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
                    FilterTransactions();
                }
                OnPropertyChanged("Filter");
            }
        }

        private void FilterTransactions()
        {
            List<Transaction> tempTransactions = Transactions.ToList();
            if (filter != null)
            {
                tempTransactions = tempTransactions.Where(x => x.Branch == filter).ToList();
            }
            TransactionList = tempTransactions;
        }

        private void RunSearch()
        {
            List<Marine> tempMarines = Marines.ToList();
            MarineList = tempMarines.Where(x => x.ToString().Contains(search.ToUpper())).Take(10).ToList();
        }

        private async void GetErrorDescription()
        {
            if(SelectedTransaction != null)
            {
                SelectedTransaction.ErrorDescription = await new Transaction().GetErrorDescription(transaction);
                UnitDiary diary = await new UnitDiary().GetDiary(selectedTransaction);
                SelectedTransaction.DiaryUploadLocation = diary.UploadLocation;
            }
        }

        private async void SetMarineTransactions()
        {
            if(SelectedMarine != null)
            {
                TransactionList = await new Transaction().GetTransactions(SelectedMarine);
                Transactions = TransactionList;
            }
            IsVisible = false;
        }

        private void SetTransactions()
        {
            Task.Run(() => RunSet());
        }

        private async Task RunSet()
        {
            IsVisible = true;
            TransactionList = await new Transaction().GetTransactions(TransactionSearch);
            Transactions = TransactionList;
            IsVisible = false;
        }

        private bool TransactionSearchable()
        {
            if(transactionSearch != null)
            {
                if (transactionSearch.TTC != 0 || transactionSearch.TTS != 0 || !string.IsNullOrEmpty(transactionSearch.TransactionErrorCode))
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

        #region Commands

        public ICommand GetTransactions
        {
            get { return new RelayCommand(execute => SetTransactions(), canExecute => TransactionSearchable()); }
        }

        public ICommand ViewDoc
        {
            get { return new RelayCommand(execute => ExecuteView(), canExecute => CanExecuteView()); }
        }

        public void ExecuteView()
        {
            new FileOperation().OpenDocument(SelectedTransaction);
        }

        public bool CanExecuteView()
        {
            if (SelectedTransaction != null)
            {
                if (!string.IsNullOrEmpty(SelectedTransaction.DiaryUploadLocation))
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
            get { return new RelayCommand(execute => GetTransactionReport()); }
        }

        private void GetTransactionReport()
        {
            IList<IReportable> report = new List<IReportable>(TransactionList);
            new FileWriter().WriteReport(report);
        }

        #endregion
    }
}
