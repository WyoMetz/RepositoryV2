using MaterialDesignThemes.Wpf;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DocumentRepository.ViewModels
{
    public class PreparerViewModel : ObservableBase
    {
        public PreparerViewModel()
        {
            Task.Run(() => BuildLists());
        }

        public async void BuildLists()
        {
            UploadVisibility = Visibility.Collapsed;
            IsVisible = true;
            TransactionComment = comment;
            EsrTransactions = await EsrTransaction.GetNotBatched();
            Transactions = EsrTransactions;
            Batches = await batch.GetWorkingBatches();
            DocumentTypes = await EsrTransaction.GetDocTypes();
            IsVisible = false;
        }

        ESRTransaction EsrTransaction = new ESRTransaction();
        UnitDiary Diary = new UnitDiary();
        IList<ESRTransaction> Transactions = new List<ESRTransaction>();
        Comment comment = new Comment();
        Batch batch = new Batch();

        private IList<ESRTransaction> esrTransactions;
        public IList<ESRTransaction> EsrTransactions
        {
            get
            {
                return esrTransactions;
            }
            set
            {
                esrTransactions = value;
                OnPropertyChanged("EsrTransactions");
            }
        }

        private IList<ESRTransaction> batchTransactions;
        public IList<ESRTransaction> BatchTransactions
        {
            get
            {
                return batchTransactions;
            }
            set
            {
                batchTransactions = value;
                OnPropertyChanged("BatchTransactions");
            }
        }


        private ESRTransaction selectedTransaction;
        public ESRTransaction SelectedTransaction
        {
            get
            {
                return selectedTransaction;
            }
            set
            {
                selectedTransaction = value;
                GetComments();
                OnPropertyChanged("SelectedTransaction");
            }
        }

        private async void GetComments()
        {
            UploadVisibility = Visibility.Collapsed;
            if(SelectedTransaction != null)
            {
                Comments = await comment.GetTransactionComments(SelectedTransaction);
                if (!string.IsNullOrEmpty(SelectedTransaction.Transaction.UploadLocation))
                {
                    UploadVisibility = Visibility.Visible;
                }
            }
        }

        public ICommand ViewDoc
        {
            get { return new RelayCommand(execute => ExecuteView()); }
        }

        public async void ExecuteView()
        {
            new FileOperation().OpenDocument(await Diary.GetDiary(SelectedTransaction.Transaction));
        }

        private IList<Batch> batches;
        public IList<Batch> Batches
        {
            get
            {
                return batches;
            }
            set
            {
                batches = value;
                OnPropertyChanged("Batches");
            }
        }

        private Batch selectedBatch;
        public Batch SelectedBatch
        {
            get
            {
                return selectedBatch;
            }
            set
            {
                selectedBatch = value;
                GetBatchTransactions();
                OnPropertyChanged("SelectedBatch");
            }
        }

        private async void GetBatchTransactions()
        {
            if(SelectedBatch != null)
                BatchTransactions = await EsrTransaction.GetBatchTransactions(SelectedBatch);
        }

        private IList<Comment> comments;
        public IList<Comment> Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
                OnPropertyChanged("Comments");
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

        private IList<string> documentTypes;
        public IList<string> DocumentTypes
        {
            get
            {
                return documentTypes;
            }
            set
            {
                documentTypes = value;
                OnPropertyChanged("DocumentTypes");
            }
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
                SearchTransactions();
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
                    SearchTransactions();
                }
                OnPropertyChanged("Filter");
            }
        }

        public async void SearchTransactions()
        {
            List<ESRTransaction> tempTransactions = Transactions.ToList();
            if (filter != null)
            {
                tempTransactions = tempTransactions.Where(x => x.Transaction.DocumentRequired == filter).ToList();
            }
            EsrTransactions = tempTransactions.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
        }

        private SnackbarMessageQueue batchMessage;
        public SnackbarMessageQueue BatchMessage
        {
            get
            {
                return batchMessage;
            }
            set
            {
                batchMessage = value;
                OnPropertyChanged("BatchMessage");
            }
        }

        private SnackbarMessageQueue transactionMessage;
        public SnackbarMessageQueue TransactionMessage
        {
            get
            {
                return transactionMessage;
            }
            set
            {
                transactionMessage = value;
                OnPropertyChanged("TransactionMessage");
            }
        }

        private SnackbarMessageQueue newBatchMessage;
        public SnackbarMessageQueue NewBatchMessage
        {
            get
            {
                return newBatchMessage;
            }
            set
            {
                newBatchMessage = value;
                OnPropertyChanged("NewBatchMessage");
            }
        }

        public ICommand CreateBatch
        {
            get { return new RelayCommand(execute => createBatch()); }
        }

        private async void createBatch()
        {
            batch.CreateNewBatch();
            Batches = await batch.GetWorkingBatches();
        }

        public ICommand AddToBatch
        {
            get { return new RelayCommand(execute => addToBatch()); }
        }

        private async void addToBatch()
        {
            if(SelectedBatch != null)
            {
                SelectedTransaction.AddToBatch(SelectedBatch);
                BatchTransactions = await EsrTransaction.GetBatchTransactions(SelectedBatch);
                Transactions = await EsrTransaction.GetNotBatched();
                SearchTransactions();
            }
        }

        public ICommand RemoveFromBatch
        {
            get { return new RelayCommand(execute => removeFromBatch()); }
        }

        private async void removeFromBatch()
        {
            if (SelectedBatch != null)
            {
                SelectedTransaction.RemoveFromBatch();
                BatchTransactions = await EsrTransaction.GetBatchTransactions(SelectedBatch);
                Transactions = await EsrTransaction.GetNotBatched();
                SearchTransactions();
            }
        }

        private bool batchDialogOpen;
        public bool BatchDialogOpen
        {
            get
            {
                return batchDialogOpen;
            }
            set
            {
                if (batchDialogOpen == value) return;
                batchDialogOpen = value;
                OnPropertyChanged("BatchDialogOpen");
            }
        }

        public ICommand OpenBatchDialog
        {
            get { return new RelayCommand(execute => BatchDialogOpen = true); }
        }

        public ICommand CloseBatchDialog
        {
            get { return new RelayCommand(execute => BatchDialogOpen = false); }
        }

        public ICommand AcceptBatchDialog
        {
            get { return new RelayCommand(execute => AddBatchNumber(), canExecute => CanAcceptBatch()); }
        }

        public ICommand CancelBatchDialog
        {
            get { return new RelayCommand(execute => BatchDialogOpen = false); }
        }

        public async void AddBatchNumber()
        {
            SelectedBatch.SendForConfirmation();
            BatchMessage = new SnackbarMessageQueue();
            BatchMessage.Enqueue($"Batch {SelectedBatch.BatchNumber} has been sent to the Approver.");
            BatchDialogOpen = false;
            SelectedBatch = null;
            BatchTransactions = null;
            Batches = await batch.GetWorkingBatches();
            Transactions = await EsrTransaction.GetNotBatched();
            SearchTransactions();
        }

        private bool CanAcceptBatch()
        {
            if(SelectedBatch != null)
            {
                if(!string.IsNullOrEmpty(SelectedBatch.BatchNumber))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool transactionDialogOpen;
        public bool TransactionDialogOpen
        {
            get
            {
                return transactionDialogOpen;
            }
            set
            {
                if(value != transactionDialogOpen)
                {
                    TransactionComment = new Comment();
                }
                transactionDialogOpen = value;
                OnPropertyChanged("TransactionDialogOpen");
            }
        }

        public ICommand OpenTransactionDialog
        {
            get
            {
                return new RelayCommand(execute => TransactionDialogOpen = true);
            }
        }

        public ICommand CloseTransactionDialog
        {
            get { return new RelayCommand(execute => TransactionDialogOpen = false); }
        }

        public ICommand AcceptTransactionDialog
        {
            get { return new RelayCommand(execute => rejectTransaction(), canExecute => canTransactionDialogAccept()); }
        }

        public ICommand CancelTransactionDialog
        {
            get { return new RelayCommand(execute => TransactionDialogOpen = false); }
        }

        private async void rejectTransaction()
        {
            SelectedTransaction.AddComment(TransactionComment);
            SelectedTransaction.RejectToApprover();
            TransactionDialogOpen = false;
            TransactionMessage = new SnackbarMessageQueue();
            TransactionMessage.Enqueue($"Transaction rejection for {SelectedTransaction.Transaction.Member.LastName} has been sent for verification.");
            Transactions = await EsrTransaction.GetNotBatched();
            SearchTransactions();
            TransactionComment = new Comment();
        }
        
        private Comment transactionComment;
        public Comment TransactionComment
        {
            get
            {
                return transactionComment;
            }
            set
            {
                transactionComment = value;
                OnPropertyChanged("TransactionComment");
            }
        }

        private bool canTransactionDialogAccept()
        {
            if(string.IsNullOrEmpty(TransactionComment.CommentText))
            {
                return false;
            }
            return true;
        }

        private Visibility uploadVisibility;
        public Visibility UploadVisibility
        {
            get
            {
                return uploadVisibility;
            }
            set
            {
                uploadVisibility = value;
                OnPropertyChanged("UploadVisibility");
            }
        }

        public ICommand ViewAttached
        {
            get { return new RelayCommand(execute => viewAttached()); }
        }

        private void viewAttached()
        {
            new FileOperation().OpenDocument(SelectedTransaction);
        }

        public ICommand GetReport
        {
            get { return new RelayCommand(execute => getReport()); }
        }

        private void getReport()
        {
            IList<IReportable> reportables = new List<IReportable>(EsrTransactions);
            new FileWriter().WriteReport(reportables);
        }
    }
}
