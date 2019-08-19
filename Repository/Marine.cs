using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Repository
{
    public class Marine : BaseReportable, ITransactable, IReportable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MI { get; set; }
        public string Rank { get; set; }
        public int EDIPI { get; set; }
        //public string Branch { get; set; }
        public int PRUC { get; set; }

        public string Create()
        {
            return $"INSERT INTO Marines (FirstName, LastName, MI, Rank, EDIPI, PRUC) Values ('{FirstName.ToUpper()}', '{LastName.ToUpper()}', '{MI.ToUpper()}', '{Rank.ToUpper()}', '{EDIPI}', '{PRUC}');";
        }

        public string CsvHeader()
        {
            return "EDIPI,Last Name,Rank,First Name,MI,PRUC";
        }

        public string CsvOutput()
        {
            return $"{EDIPI},{LastName},{Rank},{FirstName},{MI},{PRUC}";
        }

        public string DatabaseConnection()
        {
            throw new NotImplementedException();
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Marine>> GetMarines()
        {
            IList<Marine> Marines = new List<Marine>();
            SQLiteConnection connection = await new Database().Connect();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = @"SELECT EDIPI, Rank, LastName, FirstName, MI, PRUC FROM Marines ORDER BY LastName ASC;";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] values = cleanInput(reader, 6);
                        Marines.Add(new Marine
                        {
                            EDIPI = int.Parse(values[0].ToString()),
                            Rank = values[1].ToString(),
                            LastName = values[2].ToString(),
                            FirstName = values[3].ToString(),
                            MI = values[4].ToString(),
                            PRUC = int.Parse(values[5].ToString())
                        });
                    }
                }
            }
            return Marines;
        }

        public IWritable GetWritable()
        {
            return this;
        }

        public override string ToString()
        {
            return $"{EDIPI} {Rank} {FirstName} {LastName} {MI} {PRUC}".ToUpper();
        }

        public string Update()
        {
            return $"UPDATE Marines SET FirstName = '{FirstName}', LastName = '{LastName}', MI = '{MI}', Rank = '{Rank}', PRUC = '{PRUC}' WHERE EDIPI = '{EDIPI}';";
        }

        private object[] cleanInput(SQLiteDataReader reader, int size)
        {
            object[] temp = new object[size];
            reader.GetValues(temp);
            for (int i = 0; i < temp.Length; i++)
            {
                if (string.IsNullOrEmpty(temp[i].ToString()))
                {
                    temp[i] = "";
                }
            }
            return temp;
        }
    }
}
