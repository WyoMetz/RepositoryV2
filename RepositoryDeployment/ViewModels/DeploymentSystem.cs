using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using Repository;

namespace RepositoryDeployment.ViewModels
{
    public class DeploymentSystem : IDeployable
    {
        public string DatabaseFolder { get; set; }
        public string FileRepository { get; set; }
        public string ApplicationFolder { get; set; }
        public bool Update { get; set; }
        public bool ExistingDatabase { get; set; }
        private string CurrentRepositoryLocation { get; set; }
        private string CurrentApplicationsLocation { get; set; }
        private string CurrentLocation { get; set; }

        public DeploymentSystem()
        {
            CurrentLocation = Directory.GetCurrentDirectory();
            CurrentApplicationsLocation = CurrentLocation + @"\Applications";
            CurrentRepositoryLocation = CurrentLocation + @"\Repository";
        }

        public async Task Deploy()
        {
            new FileWriter().WriteResourceLocations(this);
            await new FileOperation().DeployRepository(this);
            await CreateDownloaderShortCut();
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

        public Task CreateDownloaderShortCut()
        {
            string AppDataLocal = Path.GetFullPath(ApplicationFolder);
            string ShortcutTarget = Path.GetFullPath(AppDataLocal + @"\RepositoryDownloader\RepositoryDownloader.exe");
            string ShortcutLink = Path.GetFullPath(AppDataLocal + @"\RepositoryDownloader\RepositoryDownloader.lnk");
            string ShortcutLocation = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Repository Downloader.lnk");
            string AppDirectory = Path.GetFullPath(AppDataLocal);
            WshShell ScriptingShell = new WshShell();
            WshShortcut ShortcutShell = (WshShortcut)ScriptingShell.CreateShortcut(ShortcutLocation);
            ShortcutShell.TargetPath = ShortcutTarget;
            ShortcutShell.IconLocation = ShortcutTarget + ",0";
            ShortcutShell.Description = "Repository Downloader";
            ShortcutShell.WorkingDirectory = AppDirectory;
            ShortcutShell.Arguments = "";
            ShortcutShell.Save();
            return Task.CompletedTask;
        }
    }
}
