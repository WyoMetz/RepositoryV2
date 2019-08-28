using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Repository;

namespace DocumentRepository.ViewModels
{
    public class ApproverViewModel : ObservableBase
    {
        public ApproverViewModel()
        {
            Task.Run(() => BuildLists());
        }

        private async void BuildLists()
        {
            Batches = await batch.GetNeedsCompleteBatches();
            PendingTransactions = await transaction.GetNeedsConfirmedTransactions();
            RejectionComment = comment;
        }

        Batch batch = new Batch();
        ESRTransaction transaction = new ESRTransaction();
        Comment comment = new Comment();

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
                if(SelectedBatch != null)
                {
                    SetTransactions();
                }
                OnPropertyChanged("SelectedBatch");
            }
        }

        private async void SetTransactions()
        {
            BatchTransactions = await transaction.GetBatchTransactions(SelectedBatch);
        }

        private Comment rejectionComment;
        public Comment RejectionComment
        {
            get
            {
                return rejectionComment;
            }
            set
            {
                rejectionComment = value;
                OnPropertyChanged("RejectionComment");
            }
        }

        private ESRTransaction esrTransaction;
        public ESRTransaction EsrTransaction
        {
            get
            {
                return esrTransaction;
            }
            set
            {
                esrTransaction = value;
                if(EsrTransaction != null)
                {
                    GetComments();
                }
                OnPropertyChanged("EsrTransaction");
            }
        }

        private async void GetComments()
        {
            Comments = await comment.GetTransactionComments(EsrTransaction);
        }

        private IList<ESRTransaction> pendingTransactions;
        public IList<ESRTransaction> PendingTransactions
        {
            get
            {
                return pendingTransactions;
            }
            set
            {
                pendingTransactions = value;
                OnPropertyChanged("PendingTransactions");
            }
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

        private void sendBatchMessage(string message)
        {
            BatchMessage = new SnackbarMessageQueue();
            BatchMessage.Enqueue(message);
        }

        private void sendTransactionMessage(string message)
        {
            TransactionMessage = new SnackbarMessageQueue();
            TransactionMessage.Enqueue(message);
        }

        public ICommand ApproveBatch
        {
            get { return new RelayCommand(execute => approveBatch()); }
        }

        private void approveBatch()
        {
            transaction.CompleteBatchTransactions(SelectedBatch);
            SelectedBatch.IsCompleted();
            BatchTransactions = null;
            Comments = null;
            sendBatchMessage($"Batch Number {SelectedBatch.BatchNumber} has been completed.");
            BuildLists();
        }

        public ICommand RejectBatch
        {
            get { return new RelayCommand(execute => rejectBatch()); }
        }

        private void rejectBatch()
        {
            selectedBatch.RejectToPreparer();
            BatchTransactions = null;
            Comments = null;
            sendBatchMessage($"Batch Number {SelectedBatch.BatchNumber} has been returned to the Preparers.");
            BuildLists();
        }

        public ICommand ReturnToPreparer
        {
            get { return new RelayCommand(execute => returnToPreparer()); }
        }

        private async void returnToPreparer()
        {
            EsrTransaction.RejectToPreparer();
            sendTransactionMessage($"Transaction for {EsrTransaction.Transaction.Member.LastName} has been returned to the Preparers");
            Comments = null;
            if(SelectedBatch != null)
            {
                BatchTransactions = await transaction.GetBatchTransactions(SelectedBatch);
            }
            PendingTransactions = await transaction.GetNeedsConfirmedTransactions();
        }

        public ICommand RejectToBranches
        {
            get { return new RelayCommand(execute => rejectToBranches(), canExecute => canReject()); }
        }

        private async void rejectToBranches()
        {
            EsrTransaction.AddComment(RejectionComment);
            EsrTransaction.RejectToBranches();
            RejectDialog = false;
            Comments = null;
            sendTransactionMessage($"Transaction for {EsrTransaction.Transaction.Member.LastName} has been Rejected to the Branches.");
            PendingTransactions = await transaction.GetNeedsConfirmedTransactions();
            RejectionComment = new Comment();
        }

        private bool canReject()
        {
            if (string.IsNullOrEmpty(RejectionComment.CommentText))
            {
                return false;
            }
            return true;
        }

        public ICommand CompleteTransaction
        {
            get { return new RelayCommand(execute => completeTransaction()); }
        }

        private async void completeTransaction()
        {
            EsrTransaction.CompleteTransaction();
            Comments = null;
            sendTransactionMessage($"Transaction for {EsrTransaction.Transaction.Member.LastName} has been completed.");
            PendingTransactions = await transaction.GetCompleteTransactions();
        }

        public ICommand RemoveFromBatch
        {
            get { return new RelayCommand(execute => removeFromBatch()); }
        }

        public async void removeFromBatch()
        {
            EsrTransaction.RemoveFromBatch();
            Comments = null;
            sendBatchMessage($"Transaction for {EsrTransaction.Transaction.Member.LastName} has been removed from Batch {SelectedBatch.BatchNumber}.");
            BatchTransactions = await transaction.GetBatchTransactions(SelectedBatch);
        }

        public ICommand RejectDialogOpen
        {
            get { return new RelayCommand(execute => RejectDialog = true); }
        }

        public ICommand RejectDialogClose
        {
            get { return new RelayCommand(execute => RejectDialog = false); }
        }

        private bool rejectDialog;
        public bool RejectDialog
        {
            get
            {
                return rejectDialog;
            }
            set
            {
                rejectDialog = value;
                OnPropertyChanged("RejectDialog");
            }
        }
    }
}
