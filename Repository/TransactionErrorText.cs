using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Repository
{
    public class TransactionErrorText
    {
        public string TTC { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get; set; }

        public IList<TransactionErrorText> GetTransactionErrorTexts(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                IList<TransactionErrorText> errorText = new List<TransactionErrorText>();
                string CSVheader = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(',', '\t');

                    errorText.Add(new TransactionErrorText
                    {
                        TTC = values[0],
                        ErrorCode = values[1],
                        Description = values[2]
                    });
                }
                return errorText;
            }
        }

        public async void InsertValues(IList<TransactionErrorText> errorTexts)
        {
            SQLiteConnection connection = await new Database().Connect();
            try
            {
                SQLiteCommand cmd = connection.CreateCommand();
                SQLiteTransaction transaction = connection.BeginTransaction();
                cmd.CommandText = @"INSERT INTO TransactionErrorText (TTC, ErrorCode, Description) VALUES (@TTC, @ErrorCode, @Description);";
                cmd.Parameters.AddWithValue("@TTC", "");
                cmd.Parameters.AddWithValue("@ErrorCode", "");
                cmd.Parameters.AddWithValue("@Description", "");
                foreach (var errorCode in errorTexts)
                {
                    Console.WriteLine($"Inserting {errorCode.TTC}, {errorCode.ErrorCode}");
                    cmd.Parameters["@TTC"].Value = errorCode.TTC;
                    cmd.Parameters["@ErrorCode"].Value = errorCode.ErrorCode;
                    cmd.Parameters["@Description"].Value = errorCode.Description;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
                cmd.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
