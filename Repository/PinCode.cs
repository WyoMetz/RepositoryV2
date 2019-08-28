using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PinCode : ITransactable
    {
        public int ID { get; set; }
        public string ProgramName { get; set; }
        public int Pin { get; set; }

        public string Create()
        {
            throw new NotImplementedException();
        }

        public string DatabaseConnection()
        {
            throw new NotImplementedException();
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<PinCode>> GetPinCodes()
        {
            IList<PinCode> codes = new List<PinCode>();
            SQLiteConnection connection = await new Database().Connect();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = $@"SELECT ID, ProgramName, Pin FROM PinCodes;";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codes.Add(new PinCode
                        {
                            ID = reader.GetInt32(0),
                            ProgramName = reader.GetString(1),
                            Pin = reader.GetInt32(2)
                        });
                    }
                }
            }
            return codes;
        }

        public string Update()
        {
            return $"UPDATE PinCodes SET ProgramName = '{ProgramName}', Pin = '{Pin}' WHERE ID = '{ID}';";
        }

        public void UpdatePin()
        {
            new Database().UpdateEntry(this);
        }
    }
}
