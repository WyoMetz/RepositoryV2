using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Repository;
using System.Windows.Forms;

namespace Linker
{
    public class DiaryLinker
    {
        public IList<UnitDiary> Diaries = new List<UnitDiary>();

        public DiaryLinker()
        {
            GetAllDocs();
        }

        private void GetAllDocs()
        {
            try
            {
                Console.WriteLine("Please Enter Aruc");
                Console.Write("...>");
                int Aruc = int.Parse(Console.ReadLine());
                Console.WriteLine("Please Enter Year");
                Console.Write("...>");
                int Year = int.Parse(Console.ReadLine());
                Console.WriteLine("Paste Location of current Unit Diaries;");
                Console.Write("...>");
                string[] files = Directory.GetFiles(Path.GetFullPath(Console.ReadLine()));
                foreach (var item in files)
                {
                    int number = int.Parse(Path.GetFileNameWithoutExtension(item));
                    string uploadloc = Path.GetFullPath(item);
                    Diaries.Add(new UnitDiary
                    {
                        Aruc = Aruc,
                        Year = Year,
                        Number = number,
                        Uploaded = true,
                        UploadedOn = DateTime.Now,
                        UploadedBy = "System",
                        CurrentFilePath = uploadloc
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occered: Error Text: " + ex.Message.ToString());
            }
        }

        public async void Upload()
        {
            Console.WriteLine("Copying and Updating Repository");
            foreach (var item in Diaries)
            {
                item.UploadLocation = await new FileOperation().CopyFile(item);
                new Database().UpdateEntry(item);
                Console.WriteLine(item);
            }
            Console.WriteLine("Complete");
        }
    }
}
