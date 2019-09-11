using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Repository;

namespace DocumentRepository.ViewModels
{
    public class RejectionViewModel : ObservableBase
    {
        public RejectionViewModel()
        {
            Task.Run(() => BuildLists());
        }

        public async void BuildLists()
        {
            transactions = await transaction.GetRejectedTransactions();
            EsrTransactions = transactions;
            Branches = await diary.GetBranches();
            UploadVisibility = Visibility.Collapsed;
        }

        IList<ESRTransaction> transactions = new List<ESRTransaction>();
        ESRTransaction transaction = new ESRTransaction();
        Comment comment = new Comment();
        UnitDiary diary = new UnitDiary();

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

        private ESRTransaction selectedTransaction;
        public ESRTransaction SelectedTransaction
        {
            get
            {
                return selectedTransaction;
            }
            set
            {
                UploadVisibility = Visibility.Collapsed;
                selectedTransaction = value;
                if(SelectedTransaction != null)
                {
                    GetComments();
                    if(!string.IsNullOrEmpty(SelectedTransaction.Transaction.UploadLocation))
                    {
                        UploadVisibility = Visibility.Visible;
                    }
                }
                OnPropertyChanged("SelectedTransaction");
            }
        }

        private async void GetComments()
        {
            Comments = await comment.GetTransactionComments(SelectedTransaction);
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

        private void sendMessage(string text)
        {
            Message = new SnackbarMessageQueue();
            Message.Enqueue(text);
        }

        public ICommand AttachDocument
        {
            get { return new RelayCommand(execute => attachDocument(), canExecute => SelectedTransaction != null); }
        }

        private async void attachDocument()
        {
            SelectedTransaction.CurrentFilePath = new FileOperation().ChooseFile();
            TransactionComment = new Comment();
            TransactionComment.CommentText = "Attached Document";
            SelectedTransaction.Transaction.UploadLocation = await new FileOperation().CopyFile(SelectedTransaction);
            SelectedTransaction.AddComment(TransactionComment);
            GetComments();
        }

        public ICommand AddComment
        {
            get { return new RelayCommand(execute => OpenDialog(), canExecute => SelectedTransaction != null); }
        }

        private void OpenDialog()
        {
            TransactionComment = new Comment();
            TransactionDialogOpen = true;
        }

        public ICommand ViewDoc
        {
            get { return new RelayCommand(execute => new FileOperation().OpenDocument(SelectedTransaction)); }
        }

        public ICommand ViewDiary
        {
            get
            {
                return new RelayCommand(execute => viewDiary());
            }
        }

        private async void viewDiary()
        {
            new FileOperation().OpenDocument(await new UnitDiary().GetDiary(SelectedTransaction.Transaction));
        }

        public ICommand SendToVerifier
        {
            get { return new RelayCommand(execute => returnTransaction(), canExecute => SelectedTransaction != null); }
        }

        private async void returnTransaction()
        {
            SelectedTransaction.UpdateDocument();
            sendMessage($"Transaction for {SelectedTransaction.Transaction.Member.LastName} has been sent for Verification.");
            EsrTransactions = await transaction.GetRejectedTransactions();
            SelectedTransaction = null;
            Comments = null;
            UploadVisibility = Visibility.Collapsed;
        }

        public ICommand AcceptTransactionDialog
        {
            get { return new RelayCommand(execute => addComment(), canExecute => canAccept()); }
        }

        private bool canAccept()
        {
            if(TransactionComment != null)
            {
                if(TransactionComment.CommentText != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private async void addComment()
        {
            SelectedTransaction.AddComment(TransactionComment);
            TransactionDialogOpen = false;
            Comments = await comment.GetTransactionComments(SelectedTransaction);
        }

        public ICommand CancelTransactionDialog
        {
            get { return new RelayCommand(execute => TransactionDialogOpen = false); }
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
                transactionDialogOpen = value;
                OnPropertyChanged("TransactionDialogOpen");
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
            List<ESRTransaction> tempDiaires = transactions.ToList();
            if (filter != null)
            {
                tempDiaires = tempDiaires.Where(x => x.Transaction.Branch == filter).ToList();
            }
            //MissingDiaries = tempDiaires.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
            //NumberOfMissing = MissingDiaries.Count;
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
    }
}
