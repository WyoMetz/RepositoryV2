using Microsoft.VisualBasic.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class FileOperation
    {
        public string ChooseFile()
        {
            string filePath = null;

            OpenFileDialog openFile = new OpenFileDialog
            {
                RestoreDirectory = false
            };

            if (openFile.ShowDialog() != DialogResult.Cancel)
            {
                filePath = openFile.FileName;
            }
            return filePath;
        }

        public async Task<string> CopyFile(IStorable storable)
        {
            string Year = DateTime.Now.Year.ToString();
            string Repository = Path.GetFullPath(new AppSettings().DatabaseLocation);
            string finalPath = null;

            if (!Directory.Exists(Repository))
            {
                Directory.CreateDirectory(Repository);
            }
            if (storable.CurrentFilePath != null)
            {
                finalPath = Repository + @"\" + storable.FileStoragePath();
                FileSystem.CopyFile(storable.CurrentFilePath, finalPath, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing);
            }
            return finalPath;
        }

        public void OpenDocument(IViewable viewable)
        {
            try
            {
                Process.Start(Path.GetFullPath(viewable.DocumentPath()));
            }
            catch(Exception ex)
            {
                MessageBox.Show("An Error ocurred attempting to open document: " + ex.Message.ToString(), "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Task DownloadRepository()
        {
            string AppDataLocal = Path.GetFullPath(@"C:\");
            string RepositoryDownLoad = Path.GetFullPath(AppDataLocal + @"\IDR");
            if (!Directory.Exists(RepositoryDownLoad))
            {
                Directory.CreateDirectory(RepositoryDownLoad);
            }

            string RepositoryLocation = Path.GetFullPath(new AppSettings().ApplicationLocation);
            FileSystem.CopyDirectory(RepositoryLocation, RepositoryDownLoad, true);
            return Task.CompletedTask;
        }

        public Task DownloadAdmin()
        {
            string AppDataLocal = Path.GetFullPath(@"C:\");
            string RepositoryDownLoad = Path.GetFullPath(AppDataLocal + @"\IDRAdmin");
            if (!Directory.Exists(RepositoryDownLoad))
            {
                Directory.CreateDirectory(RepositoryDownLoad);
            }

            string RepositoryLocation = Path.GetFullPath(new AppSettings().AdministrationLocation);
            FileSystem.CopyDirectory(RepositoryLocation, RepositoryDownLoad, true);
            return Task.CompletedTask;
        }

        public Task DeployRepository(IDeployable deployable)
        {
            string ApplicationsFolder = deployable.ApplicationDeploymentLocations();
            string RepositoryFolder = deployable.RepositoryDeploymentLocations();
            string ApplicationsLocation = deployable.ApplicationsCurrentLocations();
            string RepositoryLocations = deployable.RepositoryCurrentLocations();
            bool IsUpdate = deployable.IsUpdate();

            if (IsUpdate == false)
            {
                if (!Directory.Exists(RepositoryFolder))
                {
                    Directory.CreateDirectory(RepositoryFolder);
                }
                if (!Directory.Exists(ApplicationsFolder))
                {
                    Directory.CreateDirectory(ApplicationsFolder);
                }
                FileSystem.CopyDirectory(ApplicationsLocation, ApplicationsFolder, true);
                FileSystem.CopyDirectory(RepositoryLocations, RepositoryFolder, true);
                return Task.CompletedTask;
            }
            else
            {
                FileSystem.CopyDirectory(ApplicationsLocation, ApplicationsFolder, true);
                return Task.CompletedTask;
            }
        }

        public string SaveFile(IWritable writable)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.AddExtension = true;
            saveFile.DefaultExt = writable.DefaultExtension();
            saveFile.FileName = writable.DefaultFileName();
            saveFile.Filter = writable.FilterType();
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFile.OverwritePrompt = true;
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                return saveFile.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
