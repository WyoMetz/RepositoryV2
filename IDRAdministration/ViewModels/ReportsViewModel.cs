using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDRAdministration.ViewModels
{
    public class ReportsViewModel : ObservableBase
    {
        public ICommand GetMissingReport
        {
            get { return new RelayCommand(execute => GetMissing()); }
        }

        public async void GetMissing()
        {
            IList<UnitDiary> diaries = await new UnitDiary().GetMissing();
            IList<IReportable> report = new List<IReportable>(diaries);
            new FileWriter().WriteReport(report);
        }
    }
}
