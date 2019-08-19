using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITransactable
    {
        string DatabaseConnection();
        string Create();
        string Update();
        string Delete();
    }
}
