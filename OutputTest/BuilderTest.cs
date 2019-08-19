using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace OutputTest
{
    class BuilderTest
    {
        public async void RunCollectionBuild(string file)
        {
            CSVCollection collection = await new CSVCollection().read($@"{file}");
            Console.WriteLine("Running Insert");
            await Task.Run(() => new Database().InsertInformation(collection));
            Console.WriteLine("InsertComplete");
        }
    }
}
