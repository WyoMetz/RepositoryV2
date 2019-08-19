using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Repository;

namespace OutputTest
{
    public class Times
    {
        public int LargeQuery { get; set; }
        public int SmallQuery { get; set; }
        public int LargeCount { get; set; }
        public int SmallCount { get; set; }

        public Times()
        {
            Task.Run(() => RunLists());
        }

        public async void RunLists()
        {
            //GetSmall();
            GetLarge();
            GetTransaction();
            //GetValues();
            //GetTransactionsStream();
            //Console.WriteLine($"Large query took {LargeQuery} mili. Count {LargeCount}");
            //Console.WriteLine($"Small query took {SmallQuery} mili. Count {SmallCount}");
        }

        private async void GetLarge()
        {
            IList<Transaction> transactions = new List<Transaction>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            transactions = await new Transaction().GetAll();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            LargeQuery = ts.Milliseconds;
            LargeCount = transactions.Count;
            Console.WriteLine($"Large query took {ts.Seconds}s {ts.Milliseconds}ms. Count {transactions.Count}");
        }

        //private async void GetSmall()
        //{
        //    IList<Transaction> transactions = new List<Transaction>();
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    transactions = await new Transaction().GetAllShort();
        //    stopwatch.Stop();
        //    TimeSpan ts = stopwatch.Elapsed;
        //    SmallQuery = ts.Milliseconds;
        //    SmallCount = transactions.Count;
        //    Console.WriteLine($"Small query took {ts.Seconds}s {ts.Milliseconds}ms. Count {transactions.Count}");
        //}

        private async void GetTransaction()
        {
            IList<Transaction> transactions = new List<Transaction>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            transactions = await new Transaction().GetTransactions(new Transaction { TTC = 483 });
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            SmallQuery = ts.Milliseconds;
            SmallCount = transactions.Count;
            Console.WriteLine($"Transaction Specific query took {ts.Seconds}s {ts.Milliseconds}ms. Count {transactions.Count}");
        }
    }
}
