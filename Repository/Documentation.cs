using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class Documentation : BaseReportable, IStorable, ITransactable, IViewable, IReportable
    {
        public int ID { get; set; }
        public Marine Marine { get; set; }
        public string DocType { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UploadLocation { get; set; }
        public string UploadedBy { get; set; }
        public string CurrentFilePath { get; set; }

        private async Task<IList<Documentation>> GetDocuments(string cmdText)
        {
            IList<Documentation> Documents = new List<Documentation>();
            SQLiteConnection connection = await new Database().Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = cmdText;
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = cleanInput(reader, 10);
                            Marine marine = new Marine { EDIPI = int.Parse(values[1].ToString()), LastName = values[2].ToString(), FirstName = values[3].ToString(), MI = values[4].ToString(), Rank = values[5].ToString() };
                            Documents.Add(new Documentation
                            {
                                ID = reader.GetInt32(0),
                                DocType = reader.GetString(6),
                                UploadedOn = reader.GetDateTime(7),
                                UploadLocation = reader.GetString(8),
                                UploadedBy = reader.GetString(9),
                                Marine = marine
                            });
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred reading the Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Documents;
        }

        public async Task<IList<Documentation>> GetMarineDocuments(Marine marine)
        {
            string commandText = $@"SELECT D.ID, M.EDIPI, M.LastName, M.FirstName, M.MI, M.Rank, D.DocType, D.UploadedOn, D.UploadLocation, D.UploadedBy
                                    FROM Documentation AS D
                                    JOIN Marines AS M ON D.Marine = M.EDIPI
                                    WHERE D.Marine = '{marine.EDIPI}' ORDER BY M.LastName ASC;";
            return await GetDocuments(commandText);
        }

        public async Task<IList<Documentation>> GetAllDocuments()
        {
            string commandText = $@"SELECT D.ID, M.EDIPI, M.LastName, M.FirstName, M.MI, M.Rank, D.DocType, D.UploadedOn, D.UploadLocation, D.UploadedBy
                                    FROM Documentation AS D
                                    JOIN Marines AS M ON D.Marine = M.EDIPI
                                    ORDER BY M.LastName ASC;";
            return await GetDocuments(commandText);
        }

        public string FileStoragePath()
        {
            return $@"{AppSettings.Aruc}\Documentation\{Marine.EDIPI}_{Marine.LastName}\{DocType}_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Millisecond}.{Path.GetExtension(CurrentFilePath)}";
        }

        public string Create()
        {
            return $"INSERT INTO Documentation (Marine, DocType, UploadedOn, UploadLocation, UploadedBy) VALUES ('{Marine.EDIPI}', '{DocType}', '{DateTime.Now}', '{UploadLocation}', '{AppSettings.User}');";
        }

        public string Update()
        {
            return $"UPDATE Documentation SET Marine = '{Marine.EDIPI}', DocType = '{DocType}' WHERE ID = '{ID}';";
        }

        public string Delete()
        {
            return $"DELETE FROM Documentation WHERE ID = '{ID}';";
        }

        public override string ToString()
        {
            return $"{Marine.EDIPI}, {Marine.LastName}, {Marine.FirstName}, {DocType}, {UploadedBy}";
        }

        public async Task<IList<string>> GetDocumentTypes()
        {
            IList<string> DocTypes = new List<string>();
            SQLiteConnection connection = await new Database().Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"SELECT Type FROM DocumentTypes;";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocTypes.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An Error ocurred reading the Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return DocTypes;
        }

        public string DocumentPath()
        {
            return UploadLocation;
        }

        public string CsvOutput()
        {
            return $"{DocType},{Marine.EDIPI},{Marine.LastName} {Marine.Rank} {Marine.FirstName} {Marine.MI},{UploadedOn},{UploadedBy}";
        }

        public string CsvHeader()
        {
            return "Document Type,EDIPI,Marine,Uploaded On,Uploaded By";
        }

        public IWritable GetWritable()
        {
            return this;
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

        public string DatabaseConnection()
        {
            return $@"\Repository.db";
        }
    }
}
