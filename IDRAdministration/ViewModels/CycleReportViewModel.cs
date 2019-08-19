using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDRAdministration.ViewModels
{
    public class CycleReportViewModel : ObservableBase
    {
        private string filePath { get; set; }

        public CycleReportViewModel()
        {
            Task.Run(() => BuildLists());
        }

        public async void BuildLists()
        {
            OverlayVisible = false;
            IsVisible = true;
            Arucs = await new Database().GetArucs();
            Years = await new Database().GetYears();
            AppSettings.Aruc = Arucs.FirstOrDefault();
            AppSettings.Year = DateTime.Now.Year;
            CurrentArucs = Arucs;
            CurrentYears = Years;
            CurrentAruc = Arucs.First();
            CurrentYear = DateTime.Now.Year;
            IList<Marine> tempMarinelist = await marine.GetMarines();
            TotalMarines = tempMarinelist.Count;
            IList<User> tempUserList = await user.GetUsers();
            isVisible = false;
        }

        #region Objects

        CSVCollection collection = new CSVCollection();
        UnitDiary diary = new UnitDiary();
        Marine marine = new Marine();
        Transaction transaction = new Transaction();
        User user = new User();
        IList<int> Arucs = new List<int>();
        IList<int> Years = new List<int>();

        #endregion

        #region Lists

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
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

        private int cycleDiaries;
        public int CycleDiaries
        {
            get
            {
                return cycleDiaries;
            }
            set
            {
                cycleDiaries = value;
                OnPropertyChanged("CycleDiaries");
            }
        }

        private int cycleCertifiers;
        public int CycleCertifiers
        {
            get
            {
                return cycleCertifiers;
            }
            set
            {
                cycleCertifiers = value;
                OnPropertyChanged("CycleCertifiers");
            }
        }

        private int cyclePreparers;
        public int CyclePreparers
        {
            get
            {
                return cyclePreparers;
            }
            set
            {
                cyclePreparers = value;
                OnPropertyChanged("CyclePreparers");
            }
        }

        private int cycleMembers;
        public int CycleMembers
        {
            get
            {
                return cycleMembers;
            }
            set
            {
                cycleMembers = value;
                OnPropertyChanged("CycleMembers");
            }
        }

        private int cycleMarines;
        public int CycleMarines
        {
            get
            {
                return cycleMarines;
            }
            set
            {
                cycleMarines = value;
                OnPropertyChanged("CycleMarines");
            }
        }

        private int cycleTransactions;
        public int CycleTransactions
        {
            get
            {
                return cycleTransactions;
            }
            set
            {
                cycleTransactions = value;
                OnPropertyChanged("CycleTransactions");
            }
        }

        private int repositoryDiaries;
        public int RepositoryDiaries
        {
            get
            {
                return repositoryDiaries;
            }
            set
            {
                repositoryDiaries = value;
                OnPropertyChanged("RepositoryDiaries");
            }
        }

        private int lastCycle;
        public int LastCycle
        {
            get
            {
                return lastCycle;
            }
            set
            {
                lastCycle = value;
                OnPropertyChanged("LastCycle");
            }
        }

        private int totalMarines;
        public int TotalMarines
        {
            get
            {
                return totalMarines;
            }
            set
            {
                totalMarines = value;
                OnPropertyChanged("TotalMarines");
            }
        }

        private int totalTransactions;
        public int TotalTransactions
        {
            get
            {
                return totalTransactions;
            }
            set
            {
                totalTransactions = value;
                OnPropertyChanged("TotalTransactions");
            }
        }

        private int totalUsers;
        public int TotalUsers
        {
            get
            {
                return totalUsers;
            }
            set
            {
                totalUsers = value;
                OnPropertyChanged("TotalUsers");
            }
        }

        private int progressTimer;
        public int ProgressTimer
        {
            get
            {
                return progressTimer;
            }
            set
            {
                progressTimer = value;
                OnPropertyChanged("ProgressTimer");
            }
        }

        #endregion

        #region Commands

        public ICommand SelectCSV
        {
            get { return new RelayCommand(execute => ExecuteSelectCSV()); }
        }

        public ICommand UploadCSV
        {
            get { return new RelayCommand(execute => ExecuteUploadCSV(), canExecute => CanExecuteUpload()); }
        }

        #endregion

        #region Logic

        private async void ExecuteSelectCSV()
        {
            filePath = new FileOperation().ChooseFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                collection = await collection.read(filePath);
                CycleMembers = collection.Members.Count;
                CyclePreparers = collection.Preparers.Count;
                CycleCertifiers = collection.Certifiers.Count;
                CycleDiaries = collection.Diaries.Count;
                CycleMarines = collection.Marines.Count;
                CycleTransactions = collection.Transactions.Count;
                FileName = Path.GetFileName(filePath);
            }
        }

        private async void ExecuteUploadCSV()
        {
            Task.Run(() => RunTimer());
            OverlayVisible = true;
            await Task.Run(() => new Database().InsertInformation(collection));
            OverlayVisible = false;
            CycleDiaries = 0;
            CycleMembers = 0;
            CyclePreparers = 0;
            CycleMembers = 0;
            CycleCertifiers = 0;
            CycleMarines = 0;
            CycleTransactions = 0;
        }

        private bool CanExecuteUpload()
        {
            if(CycleDiaries != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task RunTimer()
        {
            ProgressTimer = 1;
            decimal counter = 0;
            foreach (var diary in collection.Diaries)
            {
                counter++;
                decimal totalprogress = (counter / cycleDiaries) * 100;
                ProgressTimer = Convert.ToInt32(totalprogress);
                await Task.Delay(3);
            }
        }

        private bool overlayVisible;
        public bool OverlayVisible
        {
            get
            {
                return overlayVisible;
            }
            set
            {
                overlayVisible = value;
                OnPropertyChanged("OverlayVisible");
            }
        }

        #endregion

        private IList<int> currentYears;
        public IList<int> CurrentYears
        {
            get
            {
                return currentYears;
            }
            set
            {
                currentYears = value;
                OnPropertyChanged("CurrentYears");
            }
        }

        private IList<int> currentArucs;
        public IList<int> CurrentArucs
        {
            get
            {
                return currentArucs;
            }
            set
            {
                currentArucs = value;
                OnPropertyChanged("CurrentArucs");
            }
        }

        private int currentYear;
        public int CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                currentYear = value;
                AppSettings.Year = value;
                Task.Run(() => SetDiaries());
                OnPropertyChanged("CurrentYear");
            }
        }

        private int currentAruc;
        public int CurrentAruc
        {
            get
            {
                return currentAruc;
            }
            set
            {
                currentAruc = value;
                AppSettings.Aruc = value;
                Task.Run(() => SetDiaries());
                OnPropertyChanged("CurrentAruc");
            }
        }

        private async void SetDiaries()
        {
            LastCycle = 0;
            RepositoryDiaries = 0;
            TotalTransactions = 0;
            IList<UnitDiary> tempDiaryList = await diary.GetAll();
            RepositoryDiaries = tempDiaryList.Count;
            IList<Transaction> tempTransList = await transaction.GetAll();
            TotalTransactions = tempTransList.Count;
            if (tempDiaryList.Count != 0)
                LastCycle = tempDiaryList.Last().CycleNumber;
        }
    }
}
