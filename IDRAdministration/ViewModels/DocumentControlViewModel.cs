using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDRAdministration.ViewModels
{
    public class DocumentControlViewModel : ObservableBase, ITransactable
    {
        public DocumentControlViewModel()
        {
            BuildLists();
        }

        public async void BuildLists()
        {
            DocumentTypes = await new Documentation().GetDocumentTypes();
            DocTypes = DocumentTypes;
        }

        public string DatabaseConnection()
        {
            return @"\Repository.db";
        }

        public string Create()
        {
            return $@"INSERT INTO DocumentTypes (Type) VALUES ('{NewType}');";
        }

        public string Update()
        {
            throw new NotImplementedException();
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }

        IList<string> DocumentTypes = new List<string>();

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

        private string newType;
        public string NewType
        {
            get
            {
                return newType;
            }
            set
            {
                newType = value;
                OnPropertyChanged("NewType");
            }
        }

        public ICommand Save
        {
            get { return new RelayCommand(execute => SaveType(), canExecute => CanSave()); }
        }

        private async void SaveType()
        {
            new Database().CreateEntry(this);
            DocumentTypes = await new Documentation().GetDocumentTypes();
            DocTypes = DocumentTypes;
        }

        private bool CanSave()
        {
            if (!string.IsNullOrEmpty(newType))
            {
                return true;
            }
            return false;
        }
    }
}
