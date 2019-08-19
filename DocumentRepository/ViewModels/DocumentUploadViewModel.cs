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
    public class DocumentUploadViewModel : ObservableBase
    {
        public DocumentUploadViewModel()
        {
            IsVisible = true;
            BuildList();
            DialogOpen = false;
        }

        private IList<Marine> Marines = new List<Marine>();
        private Marine Marine = new Marine();
        private Documentation Documentation = new Documentation();
        private IList<Documentation> Documents = new List<Documentation>();
        private IList<string> DocumentTypes = new List<string>();

        private async void BuildList()
        {
            Marines = await Marine.GetMarines();
            DocumentTypes = await Documentation.GetDocumentTypes();
            MarineDocuments = Documents;
            Document = Documentation;
            MarineList = Marines;
            NewMarine = Marine;
            DocTypes = DocumentTypes;
            IsVisible = false;
        }

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
                if(selectedMarine != null)
                {
                    Task.Run(() => GetDocuments());
                    IsVisible = true;
                }
                OnPropertyChanged("SelectedMarine");
            }
        }

        private async void GetDocuments()
        {
            MarineDocuments = await Documentation.GetMarineDocuments(SelectedMarine);
            IsVisible = false;
        }

        private Marine newMarine;
        public Marine NewMarine
        {
            get
            {
                return newMarine;
            }
            set
            {
                newMarine = value;
                OnPropertyChanged("NewMarine");
            }
        }

        private Documentation document;
        public Documentation Document
        {
            get
            {
                return document;
            }
            set
            {
                document = value;
                OnPropertyChanged("Document");
            }
        }

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

        private IList<string> docTypes;
        public IList<string> DocTypes
        {
            get
            {
                return docTypes;
            }
            set
            {
                docTypes = value;
                OnPropertyChanged("DocTypes");
            }
        }

        private string selectedDocType;
        public string SelectedDocType
        {
            get
            {
                return selectedDocType;
            }
            set
            {
                selectedDocType = value;
                Document.DocType = selectedDocType;
                OnPropertyChanged("SelectedDocType");
            }
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
                SearchMarines();
                OnPropertyChanged("Search");
            }
        }

        public async void SearchMarines()
        {
            List<Marine> TempMarines = Marines.ToList();
            MarineList = TempMarines.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
        }

        public ICommand SelectDocument
        {
            get { return new RelayCommand(execute => ExecuteSelect(), canExecute => CanExecuteSelect()); }
        }

        public void ExecuteSelect()
        {
            Document = new Documentation();
            Document.CurrentFilePath = new FileOperation().ChooseFile();
            DocumentName = Path.GetFileName(Document.CurrentFilePath);
        }

        public bool CanExecuteSelect()
        {
            if (SelectedMarine != null)
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

        public async void ExecuteUpload()
        {
            Document.Marine = SelectedMarine;
            Document.UploadLocation = await new FileOperation().CopyFile(Document);
            new Database().CreateEntry(Document);
            Message = new SnackbarMessageQueue();
            Message.Enqueue($"Document for {Document.Marine.LastName}, {Document.Marine.FirstName} has been uploaded.");
            SelectedMarine = null;
            SelectedDocType = null;
            DocumentName = null;
        }

        public bool CanExecuteUpload()
        {
            if (SelectedMarine != null && Document != null && Document.CurrentFilePath != null && Document.DocType != null)
            {
                return true;
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
            new DocumentQuickView(Document.CurrentFilePath).Show();
        }

        public ICommand OpenDialog
        {
            get { return new RelayCommand(execute => OpenDialogExecute()); }
        }
        
        public ICommand AcceptDialog
        {
            get { return new RelayCommand(execute => AcceptDialogExecute(), canExecute => canDialogAccept()); }
        }

        public ICommand CancelDialog
        {
            get { return new RelayCommand(execute => CloseDialogExecute()); }
        }

        private bool dialogOpen;
        public bool DialogOpen
        {
            get
            {
                return dialogOpen;
            }
            set
            {
                if (dialogOpen == value) return;
                dialogOpen = value;
                OnPropertyChanged("DialogOpen");
            }
        }

        private void OpenDialogExecute()
        {
            DialogOpen = true;
        }

        private void CloseDialogExecute()
        {
            DialogOpen = false;
        }

        private void AcceptDialogExecute()
        {
            new Database().CreateEntry(NewMarine);
            SelectedMarine = NewMarine;
            MarineList.Add(NewMarine);
            DialogOpen = false;
        }

        private bool canDialogAccept()
        {
            if(NewMarine.EDIPI != 0 && NewMarine.LastName != null && NewMarine.FirstName != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ICommand ViewDoc
        {
            get { return new RelayCommand(execute => ExecuteView()); }
        }

        public void ExecuteView()
        {
            new FileOperation().OpenDocument(Document);
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
