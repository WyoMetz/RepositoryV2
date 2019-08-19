using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartHere
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentLocation = Directory.GetCurrentDirectory();
            //Process.Start($@"{currentLocation}\RepositoryDeployment\RepositoryDeployment.exe");
            Console.WriteLine(currentLocation);
            IList<string> directories = Directory.GetDirectories(currentLocation.Remove(currentLocation.Length - 20, 20)).ToList();
            Console.WriteLine("Directories");
            foreach (var item in directories)
            {
                Console.WriteLine(item);
            }
            IList<string> AppDirectories = Directory.GetDirectories(currentLocation.Remove(currentLocation.Length - 20, 20) + @"Applications").ToList();
            Console.WriteLine("Applications");
            foreach (var item in AppDirectories)
            {
                Console.WriteLine(item);
            }
            IList<string> DocumentRepDirec = Directory.GetDirectories(currentLocation.Remove(currentLocation.Length - 20, 20) + @"Applications\DocumentRepository").ToList();
            Console.WriteLine("IDR");
            foreach (var item in DocumentRepDirec)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Configured Folders");
            string CurrentLocation = Directory.GetCurrentDirectory();
            string CurrentApplicationsLocation = CurrentLocation.Remove(CurrentLocation.Length - 20, 20) + @"Applications";
            IList<string> folders = new List<string>();
            folders.Add($@"{CurrentApplicationsLocation}\DocumentRepository\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\RepositoryAdministration\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\RepositoryDownloader\Resources");
            folders.Add($@"{CurrentApplicationsLocation}\AdministrationDownloader\Resources");
            string CurrentRepositoryLocation = CurrentLocation.Remove(CurrentLocation.Length - 20, 20) + @"Repository";
            foreach (var item in folders)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(CurrentRepositoryLocation);
            Console.ReadLine();
        }
    }
}
