using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class UnitDiary : BaseReportable, ITransactable, IStorable, IViewable, IReportable
    {
        public int Number { get; set; }
        public int Year { get; set; }
        public int Aruc { get; set; }
        public DateTime Date { get; set; }
        public DateTime CycleDate { get; set; }
        public string Certifier { get; set; }
        public int CycleNumber { get; set; }
        public string Branch { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public bool Uploaded { get; set; }
        public string UploadLocation { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedOn { get; set; }
        public DateTime ReportDate { get; set; }
        public bool Confirmed { get; set; }
        public double WaitTime { get { return Math.Round((DateTime.Now - ReportDate).TotalDays); } }
        public double ConfirmWait { get { return Math.Round((DateTime.Now - UploadedOn).TotalDays); } }

        public string CurrentFilePath { get; set; }

        private async Task<IList<UnitDiary>> GetDiaries(string cmdText, UnitDiary diary)
        {
            IList<UnitDiary> unitDiaries = new List<UnitDiary>();
            SQLiteConnection connection = await new Database(diary).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = cmdText;
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = cleanInput(reader, 14);
                            DateTime parseDate;
                            DateTime.TryParse(values[9].ToString(), out parseDate);
                            unitDiaries.Add(new UnitDiary
                            {
                                Number = int.Parse(values[0].ToString()),
                                Aruc = int.Parse(values[1].ToString()),
                                Year = int.Parse(values[2].ToString()),
                                Date = DateTime.Parse(values[3].ToString()),
                                CycleDate = DateTime.Parse(values[4].ToString()),
                                CycleNumber = int.Parse(values[5].ToString()),
                                Branch = values[6].ToString(),
                                UploadLocation = values[7].ToString(),
                                UploadedBy = values[8].ToString(),
                                UploadedOn = parseDate,
                                Uploaded = bool.Parse(values[10].ToString()),
                                ReportDate = DateTime.Parse(values[11].ToString()),
                                Certifier = values[12].ToString(),
                                Confirmed = bool.Parse(values[13].ToString())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading Diaries Database: " + ex.Message.ToString(), "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return unitDiaries;
        }

        public async Task<IList<UnitDiary>> GetMissing()
        {
            IList<int> Years = await new Database().GetYears();
            IList<UnitDiary> unitDiaries = new List<UnitDiary>();
            foreach (var year in Years)
            {
                UnitDiary diary = new UnitDiary { Aruc = AppSettings.Aruc, Year = year };
                IList<UnitDiary> Temp = await GetDiaries(@"SELECT DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Uploadlocation, UploadedBy, UploadedOn, Uploaded, ReportDate, Certifier, Confirmed FROM Diaries WHERE Uploaded = 'False' ORDER BY DiaryNumber ASC;", diary);
                if (Temp != null)
                {
                    foreach (var item in Temp)
                    {
                        unitDiaries.Add(item);
                    }
                }
            }
            return unitDiaries;
        }

        public async Task<IList<UnitDiary>> GetUploaded()
        {
            UnitDiary diary = new UnitDiary { Aruc = AppSettings.Aruc, Year = AppSettings.Year };
            IList<int> Years = await new Database().GetYears();
            IList<UnitDiary> Diaries = new List<UnitDiary>();
            Diaries = await GetDiaries(@"SELECT DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Uploadlocation, UploadedBy, UploadedOn, Uploaded, ReportDate, Certifier, Confirmed FROM Diaries WHERE Uploaded = 'True' ORDER BY DiaryNumber ASC;", diary);
            return Diaries;
        }

        public async Task<IList<UnitDiary>> GetAll()
        {
            UnitDiary diary = new UnitDiary { Aruc = AppSettings.Aruc, Year = AppSettings.Year };
            IList<UnitDiary> Temp = await GetDiaries(@"SELECT DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Uploadlocation, UploadedBy, UploadedOn, Uploaded, ReportDate, Certifier, Confirmed FROM Diaries ORDER BY DiaryNumber ASC;", diary);
            return Temp;
        }

        public async Task<UnitDiary> GetDiary(Transaction transaction)
        {
            UnitDiary diary = new UnitDiary { Aruc = transaction.ARUC, Year = transaction.DiaryYear };
            IList<UnitDiary> unitDiaries = await GetDiaries($@"SELECT DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Uploadlocation, UploadedBy, UploadedOn, Uploaded, ReportDate, Certifier, Confirmed FROM Diaries WHERE DiaryNumber = '{transaction.DiaryNumber}' ORDER BY DiaryNumber ASC;", diary);
            return unitDiaries.FirstOrDefault();
        }

        public async Task<IList<UnitDiary>> GetUnconfirmedDiaries()
        {
            UnitDiary diary = new UnitDiary { Aruc = AppSettings.Aruc, Year = AppSettings.Year };
            IList<UnitDiary> unitDiaries = await GetDiaries($@"SELECT DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Uploadlocation, UploadedBy, UploadedOn, Uploaded, ReportDate, Certifier, Confirmed FROM Diaries WHERE Uploaded = 'True' AND Confirmed = 'False' ORDER BY DiaryNumber ASC;", diary);
            return unitDiaries;
        }

        public override string ToString()
        {
            return $"{Number}, {Year}, {Date}, {CycleDate}, {CycleNumber}, {Branch}, {UploadedBy}".ToUpper();
        }

        public string Create()
        {
            throw new NotImplementedException();
        }

        public string Update()
        {
            return $"UPDATE Diaries SET Uploaded = 'True', UploadLocation = '{UploadLocation}', UploadedBy = '{UploadedBy}', UploadedOn = '{UploadedOn}', Confirmed = '{Confirmed}' WHERE DiaryNumber = '{Number}';";
        }

        public string Delete()
        {
            return $"UPDATE Diaries SET Uploaded = 'False' WHERE DiaryNumber = '{Number}';";
        }

        public string FileStoragePath()
        {
            return $@"{Aruc}\{Year}\{Number}\{Number}.pdf";
        }

        public void ConfirmUpload()
        {
            Confirmed = true;
            new Database(this).UpdateEntry(this);
        }

        public void RejectUpload()
        {
            new Database(this).DeleteEntry(this);
        }

        public async Task<IList<string>> GetBranches()
        {
            IList<string> branches = new List<string>();
            UnitDiary diary = new UnitDiary { Aruc = AppSettings.Aruc, Year = AppSettings.Year };
            SQLiteConnection connection = await new Database(diary).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"SELECT DISTINCT(Branch) FROM Diaries ORDER BY Branch ASC;";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = cleanInput(reader, 1);
                            branches.Add(values[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading Branches Database: " + ex.Message.ToString(), "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return branches;
        }

        public async Task<IList<int>> GetDiaryNumbers(int year, int aruc)
        {
            IList<int> diaries = new List<int>();
            UnitDiary diary = new UnitDiary { Aruc = aruc, Year = year };
            SQLiteConnection connection = await new Database(diary).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = @"SELECT DiaryNumber FROM Diaries ORDER BY DiaryNumber ASC;";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diaries.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading Diaries Database " + ex.Message.ToString(), "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return diaries;
        }

        public string DocumentPath()
        {
            return UploadLocation;
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

        public string CsvOutput()
        {
            if (Uploaded == false)
            {
                return $"{WaitTime},{Branch},{Number},{Certifier},{Date.ToShortDateString()},{CycleNumber},{CycleDate.ToShortDateString()}";
            }
            return $"{Year},{Number},{Date.ToShortDateString()},{CycleDate.ToShortDateString()},{CycleNumber},{Branch},{Certifier},{UploadedBy},{UploadedOn}";
        }

        public string CsvHeader()
        {
            if (Uploaded == false)
            {
                return "Days Awaiting Upload,Branch,Diary Number,Certifier,Diary Date,Cycle Number,Cycle Date";
            }
            return "Diary Year,Diary Number,Diary Date,Cycle Date,Cycle Number,Branch,Certifier,Uploaded,Uploaded By,Uploaded On";
        }

        public IWritable GetWritable()
        {
            return this;
        }

        public string DatabaseConnection()
        {
            return $@"{Aruc}\{Year}\Diaries.db";
        }
    }
}
