using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Repository;

namespace DocumentRepository.ViewModels
{
    public class DocumentResearchViewModel : ObservableBase
    {
        public DocumentResearchViewModel()
        {
            Task.Run(() => BuildList());
            IsVisible = true;
        }

        public async void BuildList()
        {
            Documentations = await Documentation.GetAllDocuments();
            MarineDocuments = Documentations;
            DocTypes = await Documentation.GetDocumentTypes();
            DocumentTypes = DocTypes;
            IsVisible = false;
        }

        #region Objects

        Documentation Documentation = new Documentation();

        IList<string> DocTypes = new List<string>();
        IList<Documentation> Documentations = new List<Documentation>();

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

        #endregion

        #region DataGrids

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

        #endregion

        #region Logic

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
                    SearchDocuments();
                }
                OnPropertyChanged("Filter");
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
                if (!string.IsNullOrEmpty(search))
                {
                    SearchDocuments();
                }
                OnPropertyChanged("Search");
            }
        }

        public async void SearchDocuments()
        {
            List<Documentation> tempDocs = Documentations.ToList();
            if (filter != null)
            {
                tempDocs = tempDocs.Where(x => x.DocType == filter).ToList();
            }
            MarineDocuments = tempDocs.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
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

        #region Commands

        public ICommand ViewDocument
        {
            get { return new RelayCommand(execute => ExecuteViewDocument()); }
        }

        private void ExecuteViewDocument()
        {
            new FileOperation().OpenDocument(SelectedDocument);
        }

        #endregion
    }
}
