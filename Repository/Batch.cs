using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class Batch : BaseReportable, ITransactable
    {
        public int ID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string BatchNumber { get; set; }
        public bool Complete { get; set; }
        public bool NeedsConfirmed { get; set; }
        public IList<ESRTransaction> EsrTransactions { get; set; }
        public IList<Comment> Comments { get; set; }
        private string UpdateText { get; set; }
        public double DaysSinceCreation { get { return Math.Round((DateTime.Now - CreatedOn).TotalDays); } }

        public string Create()
        {
            throw new NotImplementedException();
        }

        public async void CreateNewBatch()
        {
            string command = $@"INSERT INTO Batches (CreatedBy, CreatedOn, BatchNumber, Complete, NeedsConfirmed) VALUES ('{AppSettings.User}', '{DateTime.Now}', '0', 'False', 'False');";
            await UpdateDatabase(command);
        }

        public string DatabaseConnection()
        {
            return $@"{AppSettings.Aruc}\{AppSettings.Year}\ESR.db";
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }

        public string Update()
        {
            return UpdateText;
        }

        public void SendForConfirmation()
        {
            UpdateText = $"UPDATE Batches SET NeedsConfirmed = 'True', BatchNumber = '{BatchNumber}' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public void AddComment(Comment comment)
        {
            comment.BatchID = ID;
            new Database(this).CreateEntry(comment);
        }

        public void IsCompleted()
        {
            UpdateText = $"UPDATE Batches SET Complete = 'True' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public void RejectToPreparer()
        {
            UpdateText = $"UPDATE Batches SET NeedsConfirmed = 'False' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public async Task<IList<Batch>> GetWorkingBatches()
        {
            string command = $@"SELECT ID, CreatedBy, CreatedOn, BatchNumber, NeedsConfirmed, Complete FROM Batches WHERE NeedsConfirmed = 'False' AND Complete <> 'True' ORDER BY CreatedOn ASC;";
            return await GetBatches(command);
        }

        public async Task<IList<Batch>> GetNeedsCompleteBatches()
        {
            string command = $@"SELECT ID, CreatedBy, CreatedOn, BatchNumber, NeedsConfirmed, Complete FROM Batches WHERE NeedsConfirmed = 'True' AND Complete <> 'True' ORDER BY BatchNumber ASC;";
            return await GetBatches(command);
        }

        public async Task<IList<Batch>> GetCompletedBatches()
        {
            string command = $@"SELECT ID, CreatedBy, CreatedOn, BatchNumber, NeedsConfirmed, Complete FROM Batches WHERE Complete = 'True' ORDER BY BatchNumber ASC;";
            return await GetBatches(command);
        }
        
        private async Task<IList<Batch>> GetBatches(string cmdText)
        {
            bool needsUpdate = false;
            Type ColumnOld = typeof(long);
            Batch batch = new Batch();
            IList<Batch> batches = new List<Batch>();
            SQLiteConnection connection = await new Database(batch).Connect();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = cmdText;
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.GetFieldType(3) == ColumnOld)
                    {
                        needsUpdate = true;
                    }

                    while (reader.Read())
                    {
                        batches.Add(new Batch
                        {
                            ID = reader.GetInt32(0),
                            CreatedBy = reader.GetString(1),
                            CreatedOn = reader.GetDateTime(2),
                            BatchNumber = reader.GetValue(3).ToString(),
                            NeedsConfirmed = bool.Parse(reader.GetString(4)),
                            Complete = bool.Parse(reader.GetString(5))
                        });
                    }
                }
            }
            if(needsUpdate == true)
            {
                Task.Run(() => CorrectColumn());
            }
            return batches;
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

        private async Task UpdateDatabase(string statement)
        {
            SQLiteConnection connection = await new Database(this).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                    ID = (int)connection.LastInsertRowId;
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "SQL Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CorrectColumn()
        {
            SQLiteConnection connection = await new Database(this).Connect();
            SQLiteCommand cmd = connection.CreateCommand();
            SQLiteTransaction transaction = connection.BeginTransaction();
            cmd.CommandText = @"CREATE TABLE BatchNew 
                                (
                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                CreatedBy VARCHAR(100),
                                CreatedOn DATE,
                                BatchNumber VARCHAR(100),
                                NeedsConfirmed BOOL,
                                Complete BOOL
                                );";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"INSERT INTO BatchNew SELECT ID, CreatedBy, CreatedOn, BatchNumber, NeedsConfirmed, Complete FROM Batches WHERE true;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"DROP TABLE Batches";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"ALTER TABLE BatchNew RENAME TO Batches;";
            cmd.ExecuteNonQuery();
            transaction.Commit();
            cmd.Dispose();
            connection.Dispose();
        }
    }
}
