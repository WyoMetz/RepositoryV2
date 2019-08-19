using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Repository;

namespace UITests
{
    class ViewModel : ObservableBase
    {
        public ViewModel()
        {
            BuildList();
        }

        private async void BuildList()
        {
            UnitDiary unitDiary = new UnitDiary();
            Marine marine = new Marine();
            DiaryList = await unitDiary.GetAll();
            MarineList = await marine.GetMarines();
            Marines = MarineList;
            UnitDiaries = DiaryList;
        }

        private IList<Marine> MarineList = new List<Marine>();
        private IList<UnitDiary> DiaryList = new List<UnitDiary>();

        private IList<UnitDiary> unitDiaries;
        public IList<UnitDiary> UnitDiaries
        {
            get
            {
                return unitDiaries;
            }
            set
            {
                unitDiaries = value;
                OnPropertyChanged("UnitDiaries");
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

        private UnitDiary selectedDiary;
        public UnitDiary SelectedDiary
        {
            get
            {
                return selectedDiary;
            }
            set
            {
                selectedDiary = value;
                if(selectedDiary != null)
                {
                    SetTransactions();
                }
                OnPropertyChanged("SelectedDiary");
            }
        }

        public async void SetTransactions()
        {
            Transactions = await new Transaction().GetTransactions(selectedDiary);
        }

        public async void SetMarineTransactions()
        {
            Transactions = await new Transaction().GetTransactions(marine);
        }

        private IList<Marine> marines;
        public IList<Marine> Marines
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

        private Marine marine;
        public Marine SelectedMarine
        {
            get
            {
                return marine;
            }
            set
            {
                marine = value;
                if(marine != null)
                {
                    SetMarineTransactions();
                }
                OnPropertyChanged("SelectedMarine");
            }
        }

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
                SearchDiaries();
                OnPropertyChanged("MarineSearch");
            }
        }

        public async void SearchMarines()
        {
            List<Marine> tempList = MarineList.ToList();
            Marines = tempList.Where(x => x.ToString().Contains(marineSearch.ToUpper())).ToList();
        }

        public async void SearchDiaries()
        {
            List<UnitDiary> tempDiaires = DiaryList.ToList();
            UnitDiaries = tempDiaires.Where(x => x.ToString().Contains(marineSearch.ToUpper())).ToList();
        }
    }
}
