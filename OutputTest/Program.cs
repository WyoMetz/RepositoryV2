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
            IniTest test = new IniTest();
            test.ReadData();
            Console.ReadLine();
        }
    }
}
