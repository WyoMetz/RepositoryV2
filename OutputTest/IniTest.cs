using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutputTest
{
    public class IniTest
    {
        public IniTest()
        {
        }

        IniFile ini = new IniFile("OutputTest.ini");

        private void WriteData()
        {
            ini.Write("TestValue1", "SomeOtherValue2", "FirstSection");
            ini.Write("TestValue2", "SomeOtherValue3", "FirstSection");
            ini.Write("NewValues1", "SomeNewValues", "SecondSection");
            ini.Write("NewValues2", "SomeNewValues2", "SecondSection");
        }

        public void ReadData()
        {
            Console.WriteLine("Reading out of order...");
            string test1 = ini.Read("TestValue2", "FirstSection");
            string test2 = ini.Read("TestValue2", "SecondSection");
            Console.WriteLine(test1);
            Console.WriteLine(test2);
        }
    }
}
