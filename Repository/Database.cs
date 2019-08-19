using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class Database
    {
        private string DatabaseConnection { get; set; }

        public Database()
        {
            DatabaseConnection = $@"{new AppSettings().DatabaseLocation}\Repository.db";
        }

        public Database(ITransactable connection)
        {
            DatabaseConnection = $@"{new AppSettings().DatabaseLocation}\{connection.DatabaseConnection()}";
        }

        public async Task<SQLiteConnection> Connect()
        {
            string dbLocation = Path.GetFullPath(DatabaseConnection);
            if (dbLocation.StartsWith(@"\"))
            {
                dbLocation = $@"\{dbLocation}";
            }
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + dbLocation + ";Version=3;new=False;datetimeformat=CurrentCulture;");
            try
            {
                await dbConnection.OpenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The Connection to the Database failed: " + ex.Message.ToString(), "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new SQLiteConnection(dbConnection);
        }

        public async Task InsertInformation(CSVCollection collection)
        {
            if (collection != null)
            {
                CsvInsert insert = new CsvInsert(collection);
                await insert.InsertRepositoryInfo();
                await insert.InsertDiaryInfo();
            }
        }

        private async Task UpdateDatabase(string statement)
        {
            SQLiteConnection connection = await Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "SQL Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void CreateEntry(ITransactable transactable)
        {
            DatabaseConnection = $@"{new AppSettings().DatabaseLocation}\{transactable.DatabaseConnection()}";
            await UpdateDatabase(transactable.Create());
        }

        public async void UpdateEntry(ITransactable transactable)
        {
            DatabaseConnection = $@"{new AppSettings().DatabaseLocation}\{transactable.DatabaseConnection()}";
            await UpdateDatabase(transactable.Update());
        }

        public async void DeleteEntry(ITransactable transactable)
        {
            DatabaseConnection = $@"{new AppSettings().DatabaseLocation}\{transactable.DatabaseConnection()}";
            await UpdateDatabase(transactable.Delete());
        }

        public async Task<IList<int>> GetYears()
        {
            IList<int> YearList = new List<int>();
            SQLiteConnection connection = await Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"SELECT Year FROM Years;";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            YearList.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading Database for Years; " + ex.Message.ToString(), "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return YearList;
        }

        public async Task<IList<int>> GetArucs()
        {
            IList<int> ArucList = new List<int>();
            SQLiteConnection connection = await Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"SELECT ARUC FROM ARUCS;";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArucList.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading Database for ARUCS; " + ex.Message.ToString(), "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ArucList;
        }
    }
}
