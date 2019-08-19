using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Repository;
using System.Diagnostics;

namespace UITests
{
    class UploadViewModel : ObservableBase
    {
        public string filePath { get; set; }
        public CSVCollection Collection { get; set; }
        public UnitDiary diary = new UnitDiary();
        public Documentation documentation = new Documentation();
        public Transaction transaction = new Transaction();
        public Marine marine = new Marine();
        public IList<UnitDiary> unitDiaries = new List<UnitDiary>();
        public IList<Marine> MarinesList = new List<Marine>();
        public IList<Documentation> Documentations = new List<Documentation>();
        public IList<Transaction> TransactionList = new List<Transaction>();

        public UploadViewModel()
        {
            BuildLists();
        }

        public async void BuildLists()
        {
            TransactionList = await transaction.GetAll();
        }

        private int members;
        public int Members
        {
            get
            {
                return members;
            }
            set
            {
                members = value;
                OnPropertyChanged("Members");
            }
        }

        private int certifiers;
        public int Certifiers
        {
            get
            {
                return certifiers;
            }
            set
            {
                certifiers = value;
                OnPropertyChanged("Certifiers");
            }
        }

        private int diaries;
        public int Diaries
        {
            get
            {
                return diaries;
            }
            set
            {
                diaries = value;
                OnPropertyChanged("Diaries");
            }
        }

        private int marines;
        public int Marines
        {
            get
            {
                return marines;
            }
            set
            {
                marines = value;
                OnPropertyChanged("Marines");
            }
        }

        private int transactions;
        public int Transactions
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

        public ICommand SelectCSV
        {
            get { return new RelayCommand(execute => ExecuteSelect("Value"), canExecute => CanExecuteSelect("Value")); }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecuteSelect(object parameter)
        {
            return true;
        }

        public async void ExecuteSelect(object parameter)
        {
            CSVCollection collection = new CSVCollection();
            filePath = new FileOperation().ChooseFile();
            collection = await collection.read(filePath);
            Members = collection.Members.Count;
            Certifiers = collection.Certifiers.Count;
            Diaries = collection.Diaries.Count;
            Marines = collection.Marines.Count;
            Transactions = collection.Transactions.Count;
            Collection = collection;
        }

        public ICommand UploadCSV
        {
            get { return new RelayCommand(execute => ExecuteUpload(""), canExecute => CanExecuteUpload("")); }
        }

        public bool CanExecuteUpload(object parameter)
        {
            if (filePath != null)
            {
                if(Diaries != 0)
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

        public async void ExecuteUpload(object parameter)
        {
            await new Database().InsertInformation(Collection);
            filePath = null;
            Members = 0;
            Certifiers = 0;
            Diaries = 0;
            Marines = 0;
            Transactions = 0;
        }

        public ICommand ViewSource
        {
            get { return new RelayCommand(execute => ExecuteViewSource(), canExecute => CanExecuteUpload("")); }
        }

        public void ExecuteViewSource()
        {
            Process.Start(filePath);
        }

        public ICommand SaveFile
        {
            get { return new RelayCommand(execute => ExecuteSaveFile()); }
        }

        public void ExecuteSaveFile()
        {
            IList<IReportable> reportables = new List<IReportable>(TransactionList);
            new FileWriter().WriteReport(reportables);
        }

        private string fileLoc;
        public string FileLoc
        {
            get
            {
                return fileLoc;
            }
            set
            {
                fileLoc = value;
                OnPropertyChanged("FileLoc");
            }
        }
    }
}
