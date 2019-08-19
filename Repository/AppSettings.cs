using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AppSettings
    {
        public string DatabaseLocation { get; private set; }
        public string FileRepository { get; private set; }
        public string ApplicationLocation { get; private set; }
        public string AdministrationLocation { get; private set; }
        public static readonly string User = Environment.UserName;
        public static int Year { get; set; } = DateTime.Now.Year;
        public static int Aruc { get; set; }

        public AppSettings()
        {
            ReadLocations();
        }

        private string ResourceLocations = @".\Resources\ResourceLocations.txt";

        private void ReadLocations()
        {
            using (StreamReader reader = new StreamReader(ResourceLocations))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(';');

                    if(values[0] == "Database")
                    {
                        DatabaseLocation = values[1];
                    }
                    if(values[0] == "FileRepository")
                    {
                        FileRepository = values[1];
                    }
                    if(values[0] == "Application")
                    {
                        ApplicationLocation = values[1];
                    }
                    if(values[0] == "Administration")
                    {
                        AdministrationLocation = values[1];
                    }
                }
            }
        }
    }
}
