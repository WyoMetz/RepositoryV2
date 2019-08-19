using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace OutputTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning");
            Console.Write("File...>");
            string FileLocation = Console.ReadLine();
            new BuilderTest().RunCollectionBuild(FileLocation);
            Console.WriteLine("Main");
            Console.ReadLine();
        }

        private static void CheckOutput()
        {
            string path = Path.GetFullPath(@".\");
            string trim = path.Remove(path.Length - 10, 10);
            Console.WriteLine(path);
            Console.WriteLine(trim);
        }
    }
}
