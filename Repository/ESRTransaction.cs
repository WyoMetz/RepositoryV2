using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class ESRTransaction : BaseReportable, ITransactable, IViewable, IReportable
    {
        public Transaction Transaction { get; set; }
        public int BatchID { get; set; }
        public int ID { get; set; }
        public DateTime ConfirmDate { get; set; }
        public DateTime RejectDate { get; set; }
        public DateTime ApproverDate { get; set; }
        public bool IsRejected { get; set; }
        public bool NeedsConfirmed { get; set; }
        public bool Complete { get; set; }
        public double ConfirmWait { get { return Math.Round((DateTime.Now - ConfirmDate).TotalDays); } }
        public double RejectWait { get { return Math.Round((DateTime.Now - RejectDate).TotalDays); } }
        public double ApproverWait { get { return Math.Round((DateTime.Now - ApproverDate).TotalDays); } }
        private string UpdateText { get; set; }

        public string Create()
        {
            throw new NotImplementedException();
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
            return UpdateText; //TODO: Define Update
        }

        public IWritable GetWritable()
        {
            return this;
        }

        public void RejectToApprover()
        {
            UpdateText = $@"UPDATE ESRTransactions SET NeedsConfirmed = 'True', ApproverDate = '{DateTime.Now}' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public void RejectToBranches()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($@"UPDATE ESRTransactions SET NeedsConfirmed = 'False', IsRejected = 'True'");
            if(DateTime.Now <= RejectDate || RejectDate == null)
            {
                builder.Append($@", RejectDate = '{DateTime.Now}");
            }
            builder.Append($@" WHERE ID = '{ID}';");
            UpdateText = builder.ToString();
            new Database(this).UpdateEntry(this);
        }

        public void CompleteTransaction()
        {
            UpdateText = $@"UPDATE ESRTransactions SET Complete = 'True' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public void UpdateDocument()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($@"UPDATE ESRTransactions SET IsRejected = 'False'");
            if(Transaction.CurrentFilePath != null)
            {
                builder.Append($@", UploadLocation = '{Transaction.UploadLocation}'");
            }
            builder.Append($@"WHERE ID = '{ID}';");
            UpdateText = builder.ToString();
            new Database(this).UpdateEntry(this);
        }

        public void AddToBatch(Batch batch)
        {
            UpdateText = $@"UPDATE ESRTransactions SET BatchID = '{batch.ID}' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public void RemoveFromBatch()
        {
            UpdateText = $@"UPDATE ESRTransactions SET BatchID = '0' WHERE ID = '{ID}';";
            new Database(this).UpdateEntry(this);
        }

        public string DocumentPath()
        {
            return Transaction.UploadLocation;
        }

        public string CsvOutput()
        {
            throw new NotImplementedException(); //TODO: Define CSV Output and Header
        }

        public string CsvHeader()
        {
            throw new NotImplementedException();
        }

        public async Task<IList<ESRTransaction>> GetNotBatched()
        {
            string Command = SelectBase().Append($@"WHERE BatchID = '0' ORDER BY DiaryNumber ASC;").ToString();
            return await GetTransactionsAsync(Command);
        }

        public async Task<IList<ESRTransaction>> GetBatchTransactions(Batch batch)
        {
            string Command = SelectBase().Append($@"WHERE BatchID = '{batch.ID}' ORDER BY Member ASC;").ToString();
            return await GetTransactionsAsync(Command);
        }

        public async  Task<IList<ESRTransaction>> GetRejectedTransactions()
        {
            string Command = SelectBase().Append($@"WHERE IsRejected = 'True' ORDER BY DiaryNumber ASC;").ToString();
            return await GetTransactionsAsync(Command);
        }

        public async Task<IList<ESRTransaction>> GetNeedsConfirmedTransactions()
        {
            string Command = SelectBase().Append($@"WHERE NeedsConfirmed = 'True' ORDER BY DiaryNumber ASC;").ToString();
            return await GetTransactionsAsync(Command);
        }

        public async Task<IList<ESRTransaction>> GetCompleteTransactions()
        {
            string Command = SelectBase().Append($@"WHERE Complete = 'True' ORDER BY DiaryNumber ASC;").ToString();
            return await GetTransactionsAsync(Command);
        }

        private StringBuilder SelectBase()
        {
            string BaseSelect = $@"SELECT TTC, 
                                    TTS, 
                                    EnglishStatement, 
                                    HistoryStatement, 
                                    TransactionErrorCode, 
                                    Certifier, 
                                    CertifierRank, 
                                    CertifierLastName, 
                                    CertifierFirstName, 
                                    Preparer, 
                                    PreparerRank, 
                                    PreparerLastName, 
                                    PreparerFirstName, 
                                    Member, 
                                    MemberRank, 
                                    MemberLastName, 
                                    MemberFirstName, 
                                    DiaryNumber, 
                                    UploadLocation, 
                                    ARUC, 
                                    DiaryYear, 
                                    Branch, 
                                    DocumentRequired, 
                                    DocumentMissing, 
                                    DocumentAttached, 
                                    UpdateDate, 
                                    BatchNumber,
                                    BatchID,
                                    IsRejected,
                                    ConfirmDate,
                                    RejectDate,
                                    NeedsConfirmed,
                                    Complete,
                                    ID,
                                    ApproverDate
                                    FROM ESRTransactions ";
            StringBuilder builder = new StringBuilder();
            builder.Append(BaseSelect);
            return builder;
        }

        private async Task<IList<ESRTransaction>> GetTransactionsAsync(string cmdText)
        {
            IList<ESRTransaction> transactions = new List<ESRTransaction>();
            SQLiteConnection connection = await new Database(new ESRTransaction()).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = cmdText;
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = cleanInput(reader, 35);
                            bool missing = false;
                            bool attached = false;
                            int batchNumber = 0;
                            DateTime now = DateTime.Now;
                            bool.TryParse(values[23].ToString(), out missing);
                            bool.TryParse(values[24].ToString(), out attached);
                            DateTime.TryParse(values[25].ToString(), out now);
                            int.TryParse(values[26].ToString(), out batchNumber);
                            Marine Certifier = new Marine { EDIPI = int.Parse(values[5].ToString()), Rank = values[6].ToString(), LastName = values[7].ToString(), FirstName = values[8].ToString() };
                            Marine Preparer = new Marine { EDIPI = int.Parse(values[9].ToString()), Rank = values[10].ToString(), LastName = values[11].ToString(), FirstName = values[12].ToString() };
                            Marine Member = new Marine { EDIPI = int.Parse(values[13].ToString()), Rank = values[14].ToString(), LastName = values[15].ToString(), FirstName = values[16].ToString() };
                            transactions.Add(new ESRTransaction
                            {
                                Transaction = new Transaction
                                {
                                    TTC = int.Parse(values[0].ToString()),
                                    TTS = int.Parse(values[1].ToString()),
                                    DiaryNumber = int.Parse(values[17].ToString()),
                                    EnglishStatement = values[2].ToString(),
                                    HistoryStatement = values[3].ToString(),
                                    TransactionErrorCode = values[4].ToString(),
                                    DiaryUploadLocation = values[18].ToString(),
                                    Preparer = Preparer,
                                    Certifier = Certifier,
                                    Member = Member,
                                    ARUC = int.Parse(values[19].ToString()),
                                    DiaryYear = int.Parse(values[20].ToString()),
                                    Branch = values[21].ToString(),
                                    DocumentRequired = values[22].ToString(),
                                    DocumentMissing = missing,
                                    DocumentAttached = attached,
                                    UpdateDate = now,
                                    BatchNumber = batchNumber
                                },
                                BatchID = int.Parse(values[27].ToString()),
                                IsRejected = bool.Parse(values[28].ToString()),
                                ConfirmDate = DateTime.Parse(values[29].ToString()),
                                RejectDate = DateTime.Parse(values[30].ToString()),
                                NeedsConfirmed = bool.Parse(values[31].ToString()),
                                Complete = bool.Parse(values[32].ToString()),
                                ID = int.Parse(values[33].ToString()),
                                ApproverDate = DateTime.Parse(values[34].ToString())
                            });
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred reading the ESR Transaction Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return transactions;
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
