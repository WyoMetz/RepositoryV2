using Repository;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentRepository.ViewModels
{
    public class MemberResearchViewModel : ObservableBase
    {
        public MemberResearchViewModel()
        {
            Task.Run(() => BuildLists());
            IsVisible = true;
        }

        public async void BuildLists()
        {
            MarineList = await Marine.GetMarines();
            ListMarines = MarineList;
            MarineDocuments = DocumentList;
            Transactions = TransactionList;
            IsVisible = false;
        }

        #region Objects
        Marine Marine = new Marine();
        Documentation Documentation = new Documentation();
        Transaction Transaction = new Transaction();
        UnitDiary Diary = new UnitDiary();

        private IList<Marine> MarineList = new List<Marine>();
        private IList<Transaction> TransactionList = new List<Transaction>();
        private IList<Documentation> DocumentList = new List<Documentation>();

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
                if (selectedMarine != null)
                {
                    IsVisible = true;
                    Task.Run(() => RunTransactions());
                    Task.Run(() => GetDocuments());
                }
                OnPropertyChanged("SelectedMarine");
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
                if(selectedTransaction == null)
                {
                    selectedTransaction = value;
                    if (selectedTransaction != null)
                    {
                        IsVisible = true;
                        Task.Run(() => GetDiaryInfo());
                    }
                    GetErrorCode();
                    OnPropertyChanged("SelectedTransaction");
                }
                if (value != null && value.DiaryNumber != selectedTransaction.DiaryNumber)
                {
                    selectedTransaction = value;
                    if (selectedTransaction != null)
                    {
                        IsVisible = true;
                        Task.Run(() => GetDiaryInfo());
                    }
                    GetErrorCode();
                    OnPropertyChanged("SelectedTransaction");
                }
            }
        }

        private async void GetErrorCode()
        {
            if (selectedTransaction != null)
                selectedTransaction.ErrorDescription = await new Transaction().GetErrorDescription(selectedTransaction);
        }

        private Documentation selectedDocument;
        public Documentation SelectedDocument
        {
            get
            {
                return selectedDocument;
            }
            set
            {
                selectedDocument = value;
                OnPropertyChanged("SelectedDocument");
            }
        }

        private UnitDiary unitDiary;
        public UnitDiary UnitDiary
        {
            get
            {
                return unitDiary;
            }
            set
            {
                unitDiary = value;
                OnPropertyChanged("UnitDiary");
            }
        }

        #endregion

        #region Lists

        private IList<Marine> listMarines;
        public IList<Marine> ListMarines
        {
            get
            {
                return listMarines;
            }
            set
            {
                listMarines = value;
                OnPropertyChanged("ListMarines");
            }
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

        private IList<Documentation> marineDocuments;
        public IList<Documentation> MarineDocuments
        {
            get
            {
                return marineDocuments;
            }
            set
            {
                marineDocuments = value;
                OnPropertyChanged("MarineDocuments");
            }
        }
        #endregion

        #region Searches

        private string marineSearch;
        public string MarineSearch
        {
            get
            {
                return marineSearch;
            }
            set
            {
                marineSearch = value;
                SearchMarines();
                OnPropertyChanged("MarineSearch");
            }
        }

        private string transactionSearch;
        public string TransactionSearch
        {
            get
            {
                return transactionSearch;
            }
            set
            {
                transactionSearch = value;
                SearchTransactions();
                OnPropertyChanged("TransactionSearch");
            }
        }

        private string documentSearch;
        public string DocumentSearch
        {
            get
            {
                return documentSearch;
            }
            set
            {
                documentSearch = value;
                SearchDocuments();
                OnPropertyChanged("DocumentSearch");
            }
        }

        private void SearchMarines()
        {
            List<Marine> TempMarines = MarineList.ToList();
            ListMarines = TempMarines.Where(x => x.ToString().Contains(marineSearch.ToUpper())).ToList();
        }

        private void SearchTransactions()
        {
            if(TransactionList == null)
            {
                TransactionList = Transactions;
            }
            List<Transaction> TempTransactions = TransactionList.ToList();
            Transactions = TempTransactions.Where(x => x.ToString().Contains(transactionSearch.ToUpper())).ToList();
        }

        private void SearchDocuments()
        {
            if(DocumentList == null)
            {
                DocumentList = MarineDocuments;
            }
            List<Documentation> TempDocuments = DocumentList.ToList();
            MarineDocuments = TempDocuments.Where(x => x.ToString().Contains(documentSearch.ToUpper())).ToList();
        }

        #endregion

        #region Commands

        public ICommand ViewDiary
        {
            get { return new RelayCommand(execute => ExecuteViewDiary(), canExecute => CanViewDiary()); }
        }

        private bool CanViewDiary()
        {
            if (UnitDiary != null)
            {
                if (!string.IsNullOrEmpty(UnitDiary.UploadLocation))
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

        private void ExecuteViewDiary()
        {
            new FileOperation().OpenDocument(UnitDiary);
        }

        public ICommand ViewDocument
        {
            get { return new RelayCommand(execute => ExecuteViewDocument()); }
        }

        private void ExecuteViewDocument()
        {
            new FileOperation().OpenDocument(SelectedDocument);
        }

        #endregion

        #region Logic

        private async void RunTransactions()
        {
            TransactionList = await Transaction.GetTransactions(SelectedMarine);
            Transactions = TransactionList;
            IsVisible = false;
        }

        private async void GetDocuments()
        {
            DocumentList = await Documentation.GetMarineDocuments(SelectedMarine);
            MarineDocuments = DocumentList;
            IsVisible = false;
        }

        private async void GetDiaryInfo()
        {
            UnitDiary = await Diary.GetDiary(SelectedTransaction);
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

        #endregion
    }
}
