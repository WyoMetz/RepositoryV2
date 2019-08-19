using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class FileWriter
    {
        public void WriteReport(IList<IReportable> reportables)
        {
            using (StreamWriter writer = new StreamWriter(new FileOperation().SaveFile(reportables.FirstOrDefault().GetWritable()), false))
            {
                writer.WriteLine(reportables.FirstOrDefault().CsvHeader());
                foreach (var item in reportables)
                {
                    writer.WriteLine(item.CsvOutput());
                }
            }
        }

        public void WriteResourceLocations(IDeployable deployable)
        {
            string Database = $@"Database;{deployable.RepositoryDeploymentLocations()}";
            string FileRepository = $@"FileRepository;{deployable.FileRepositoryDeploymentLocations()}";
            string Application = $@"Application;{deployable.DocumentRepositoryDeploymentLocation()}";
            string Administration = $@"Administration;{deployable.AdministrationDeploymentLocations()}";

            foreach (var folder in deployable.ResourceFolders())
            {
                using(StreamWriter writer = new StreamWriter(Path.GetFullPath($@"{folder}\ResourceLocations.txt"), false))
                {
                    writer.WriteLine(Database);
                    writer.WriteLine(FileRepository);
                    writer.WriteLine(Application);
                    writer.WriteLine(Administration);
                }
            }
        }
    }
}
