using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace OutputTest
{
    class Filewritertest : IDeployable
    {
        public string DatabaseFolder { get; set; }
        public string FileRepository { get; set; }
        public string ApplicationFolder { get; set; }
        public bool Update { get; set; }
        public bool ExistingDatabase { get; set; }
        private string CurrentRepositoryLocation { get; set; }
        private string CurrentApplicationsLocation { get; set; }
        private string CurrentLocation { get; set; }

        public Filewritertest()
        {
            CurrentLocation = Directory.GetCurrentDirectory();
            CurrentApplicationsLocation = CurrentLocation + @"\Applications";
            CurrentRepositoryLocation = CurrentLocation + @"\Repository";
            DatabaseFolder = $@"TEST REWRITE";
            FileRepository = $@"TEST REWRITE";
            ApplicationFolder = $@"TEST REWRITE";
            Update = false;
        }

        public void TestWriter()
        {
            new FileWriter().WriteResourceLocations(this);
        }

        public string ApplicationDeploymentLocations()
        {
            return ApplicationFolder;
        }

        public string ApplicationsCurrentLocations()
        {
            return CurrentApplicationsLocation;
        }

        public bool IsUpdate()
        {
            return Update;
        }

        public string RepositoryCurrentLocations()
        {
            return CurrentRepositoryLocation;
        }

        public string RepositoryDeploymentLocations()
        {
            return DatabaseFolder;
        }

        public string FileRepositoryDeploymentLocations()
        {
            return FileRepository;
        }

        public string AdministrationDeploymentLocations()
        {
            return $@"{ApplicationFolder}\IDRAdministration";
        }

        public IList<string> ResourceFolders()
        {
            IList<string> folders = new List<string>();
            folders.Add($@"{CurrentApplicationsLocation}\DocumentRepository\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\RepositoryAdministration\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\RepositoryDownloader\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\AdministrationDownloader\Resources");
            return folders;
        }

        public string DocumentRepositoryDeploymentLocation()
        {
            return $@"{ApplicationFolder}\DocumentRepository";
        }


    }
}
