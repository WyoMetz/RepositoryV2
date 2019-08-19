using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BaseReportable : IWritable
    {
        public string DefaultExtension()
        {
            return ".csv";
        }

        public string DefaultFileName()
        {
            return $"RepositoryReport{DateTime.Now.Month}{DateTime.Now.Day}";
        }

        public string FilterType()
        {
            return "CSV Files (*.csv)|*.csv";
        }
    }
}
