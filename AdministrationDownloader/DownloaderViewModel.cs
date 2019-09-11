using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdministrationDownloader
{
    public class DownloaderViewModel : ObservableBase
    {
        private int downloadProgress;
        public int DownloadProgress
        {
            get
            {
                return downloadProgress;
            }
            set
            {
                downloadProgress = value;
                OnPropertyChanged("DownloadProgress");
            }
        }

        public ICommand Download
        {
            get { return new RelayCommand(execute => RunDownload()); }
        }

        public async void RunDownload()
        {
            await Task.Run(() => Progress());
            Process.Start(Path.GetFullPath(@"C:\IDR\DocumentRepository.exe"));
            Application.Current.MainWindow.Close();
        }

        public async Task Progress()
        {
            Task.Run(() => new FileOperation().DownloadRepository());
            DownloadProgress = 0;
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(150);
                DownloadProgress++;
            }
            await Task.Run(() => CreateShortCut());
        }


        public Task CreateShortCut()
        {
            string AppDataLocal = Path.GetFullPath(@"C:\");
            string ShortcutTarget = Path.GetFullPath(AppDataLocal + @"\IDR\DocumentRepository.exe");
            string ShortcutLink = Path.GetFullPath(AppDataLocal + @"\IDR\DocumentRepository.lnk");
            string ShortcutLocation = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Document Repository.lnk");
            string AppDirectory = Path.GetFullPath(AppDataLocal + @"\IDR");
            WshShell ScriptingShell = new WshShell();
            WshShortcut ShortcutShell = (WshShortcut)ScriptingShell.CreateShortcut(ShortcutLocation);
            ShortcutShell.TargetPath = ShortcutTarget;
            ShortcutShell.IconLocation = ShortcutTarget + ",0";
            ShortcutShell.Description = "Document Repository";
            ShortcutShell.WorkingDirectory = AppDirectory;
            ShortcutShell.Arguments = "";
            ShortcutShell.Save();
            return Task.CompletedTask;
        }
    }
}
