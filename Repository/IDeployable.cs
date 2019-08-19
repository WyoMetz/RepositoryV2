using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IDeployable
    {
        string ApplicationDeploymentLocations();
        string RepositoryDeploymentLocations();
        string FileRepositoryDeploymentLocations();
        string AdministrationDeploymentLocations();
        string ApplicationsCurrentLocations();
        string RepositoryCurrentLocations();
        string DocumentRepositoryDeploymentLocation();
        bool IsUpdate();
        IList<string> ResourceFolders();
    }
}
