using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Repository
{
    public class Transaction : BaseReportable, IReportable, IViewable, ITransactable
    {
        public Marine Certifier { get; set; }
        public Marine Member { get; set; }
        public Marine Preparer { get; set; }
        public int DiaryNumber { get; set; }
        public int DiaryYear { get; set; }
        public int ARUC { get; set; }
        public int TTC { get; set; }
        public int TTS { get; set; }
        public string Branch { get; set; }
        public string EnglishStatement { get; set; }
        public string TransactionErrorCode { get; set; }
        public string HistoryStatement { get; set; }
        public string ErrorDescription { get; set; }
        public string DiaryUploadLocation { get; set; }
        public string DocumentRequired { get; set; }
        public bool DocumentMissing { get; set; }
        public bool DocumentAttached { get; set; }
        public int BatchNumber { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UploadLocation { get; set; }

        public override string ToString()
        {
            return $"{Certifier.EDIPI}, {Certifier.LastName}, {Certifier.FirstName}, {Member.EDIPI}, {Member.LastName}, {Member.FirstName}, {Member.PRUC}, {Preparer.EDIPI}, {Preparer.LastName}, {Preparer.FirstName}, {DiaryNumber}, {TTC}, {TTS}, {EnglishStatement}, {TransactionErrorCode}, {HistoryStatement}".ToUpper();
        }

        public async Task<IList<Transaction>> GetTransactions(UnitDiary diary)
        {
            Transaction transaction = new Transaction { ARUC = AppSettings.Aruc, DiaryYear = diary.Year, DiaryNumber = diary.Number };
            string commandText = $@"SELECT TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, Certifier, CertifierRank, CertifierLastName, CertifierFirstName, Preparer, PreparerRank, PreparerLastName, PreparerFirstName, Member, MemberRank, MemberLastName, MemberFirstName, DiaryNumber, UploadLocation, ARUC, DiaryYear, Branch, DocumentRequired, DocumentMissing, DocumentAttached, UpdateDate, BatchNumber
                                    FROM Transactions 
                                    WHERE DiaryNumber = '{diary.Number}' ORDER BY Member ASC;";
            return await GetTransactionsAsync(commandText, transaction);
        }

        public async Task<IList<Transaction>> GetTransactionInfomration(UnitDiary diary)
        {
            Transaction transaction = new Transaction { ARUC = diary.Aruc, DiaryYear = diary.Year, DiaryNumber = diary.Number };
            string commandText = $@"SELECT TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, Certifier, CertifierRank, CertifierLastName, CertifierFirstName, Preparer, PreparerRank, PreparerLastName, PreparerFirstName, Member, MemberRank, MemberLastName, MemberFirstName, DiaryNumber, UploadLocation, ARUC, DiaryYear, Branch, DocumentRequired, DocumentMissing, DocumentAttached, UpdateDate, BatchNumber
                                    FROM Transactions 
                                    WHERE DiaryNumber = '{diary.Number}' ORDER BY Member ASC;";
            return await GetTransactionsAsync(commandText, transaction);
        }

        public async Task<IList<Transaction>> GetTransactions(Marine marine)
        {
            string commandText = $@"SELECT TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, Certifier, CertifierRank, CertifierLastName, CertifierFirstName, Preparer, PreparerRank, PreparerLastName, PreparerFirstName, Member, MemberRank, MemberLastName, MemberFirstName, DiaryNumber, UploadLocation, ARUC, DiaryYear, Branch, DocumentRequired, DocumentMissing, DocumentAttached, UpdateDate, BatchNumber
                                    FROM Transactions 
                                    WHERE Member = '{marine.EDIPI}' ORDER BY Member ASC;";
            IList<MarineDiaries> marineDiaries = await new MarineDiaries().GetMemberDiaires(marine);
            IList<Transaction> transactions = new List<Transaction>();
            foreach (var diary in marineDiaries)
            {
                Transaction transaction = new Transaction { ARUC = diary.Aruc, DiaryNumber = diary.DiaryNumber, DiaryYear = diary.Year };
                IList<Transaction> temp = await GetTransactionsAsync(commandText, transaction);
                foreach (var item in temp)
                {
                    transactions.Add(item);
                }
            }
            return transactions;
        }

        public async Task<IList<Transaction>> GetAll()
        {
            string commandText = $@"SELECT TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, Certifier, CertifierRank, CertifierLastName, CertifierFirstName, Preparer, PreparerRank, PreparerLastName, PreparerFirstName, Member, MemberRank, MemberLastName, MemberFirstName, DiaryNumber, UploadLocation, ARUC, DiaryYear, Branch, DocumentRequired, DocumentMissing, DocumentAttached, UpdateDate, BatchNumber
                                    FROM Transactions 
                                    ORDER BY Member ASC;";
            IList<Transaction> transactions = new List<Transaction>();
            IList<int> diaries = await new UnitDiary().GetDiaryNumbers(AppSettings.Year, AppSettings.Aruc);
            foreach (var diary in diaries)
            {
               Transaction transaction = new Transaction { ARUC = AppSettings.Aruc, DiaryYear = AppSettings.Year, DiaryNumber = diary };
               IList<Transaction> temp = await GetTransactionsAsync(commandText, transaction);
               foreach (var item in temp)
               {
                    transactions.Add(item);
               }
            }
            return transactions;
        }

        public async Task<IList<Transaction>> GetTransactions(Transaction transaction)
        {
            transaction.ARUC = AppSettings.Aruc;
            transaction.DiaryYear = AppSettings.Year;
            StringBuilder builder = new StringBuilder();
            builder.Append($@"SELECT TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, Certifier, CertifierRank, CertifierLastName, CertifierFirstName, Preparer, PreparerRank, PreparerLastName, PreparerFirstName, Member, MemberRank, MemberLastName, MemberFirstName, DiaryNumber, UploadLocation, ARUC, DiaryYear, Branch, DocumentRequired, DocumentMissing, DocumentAttached, UpdateDate, BatchNumber
                                    FROM Transactions 
                                    WHERE ");
            if (transaction.TTC != 0)
            {
                builder.Append($"TTC = '{transaction.TTC}'");
            }   
            if (transaction.TTS != 0)
            {
                if(transaction.TTC != 0)
                {
                    builder.Append(" AND ");
                }
                builder.Append($"TTS = '{transaction.TTS}'");
            }    
            if (!string.IsNullOrEmpty(transaction.TransactionErrorCode))
            {
                if(transaction.TTC != 0 || transaction.TTS != 0)
                {
                    builder.Append(" AND ");
                }
                builder.Append($"TransactionErrorCode = '{transaction.TransactionErrorCode.ToUpper()}'");
            }
            builder.Append(" ORDER BY DiaryNumber;");
            string commandText = builder.ToString();
            IList<Transaction> transactions = new List<Transaction>();
            IList<int> diaries = await new UnitDiary().GetDiaryNumbers(AppSettings.Year, AppSettings.Aruc);
            foreach (var diary in diaries)
            {
                transaction.DiaryNumber = diary;
                IList<Transaction> tempTransactions = await GetTransactionsAsync(commandText, transaction);
                foreach (var item in tempTransactions)
                {
                    transactions.Add(item);
                }
            }
            return transactions;
        }

        private async Task<IList<Transaction>> GetTransactionsAsync(string cmdText, Transaction transaction)
        {
            IList<Transaction> transactions = new List<Transaction>();
            SQLiteConnection connection = await new Database(transaction).Connect();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = cmdText;
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = cleanInput(reader, 27);
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
                            transactions.Add(new Transaction
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
                            });
                        }
                        reader.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An Error ocurred reading the Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return transactions;
        }

        public async Task<string> GetErrorDescription(Transaction transaction)
        {
            if (!string.IsNullOrEmpty(transaction.TransactionErrorCode))
            {
                SQLiteConnection connection = await new Database().Connect();
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(connection))
                    {
                        cmd.CommandText = $"SELECT Description FROM TransactionErrorText WHERE TTC = '{transaction.TTC}' AND ErrorCode = '{transaction.TransactionErrorCode}' OR TTC = 'ANY' AND ErrorCode = '{transaction.TransactionErrorCode}';";
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            if (reader.HasRows)
                            {
                                transaction.ErrorDescription = reader.GetString(0);
                            }
                            reader.Close();
                        }
                    }
                    if (string.IsNullOrEmpty(transaction.ErrorDescription))
                    {
                        transaction.ErrorDescription = "Resulted in ICR or Description not on file. Research and Resubmit if necessary.".ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error ocurred reading the Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return transaction.ErrorDescription;
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
            return $"{DiaryNumber},{TTC},{TTS},{TransactionErrorCode},{Member.EDIPI},{Member.LastName} {Member.Rank} {Member.FirstName},{Preparer.EDIPI},{Preparer.LastName} {Preparer.Rank} {Preparer.FirstName},{Certifier.EDIPI},{Certifier.LastName} {Certifier.Rank} {Certifier.FirstName},{EnglishStatement},{HistoryStatement},{ErrorDescription},{DocumentRequired}";
        }

        public string CsvHeader()
        {
            return "Diary Number,TTC,TTS,Error Code,Marine EDIPI,Marine,Preparer EDIPI,Preparer,Certifier EDIPI,Certifier,English Text,History Statement,Error Code Description,Document Required";
        }

        public IWritable GetWritable()
        {
            return this;
        }

        public string DocumentPath()
        {
            return DiaryUploadLocation;
        }

        public async Task<Transaction> ParseObject(object[] inputValues)
        {
            object[] values = cleanInput(inputValues);
            Marine Certifier = new Marine { EDIPI = int.Parse(values[5].ToString()), Rank = values[6].ToString(), LastName = values[7].ToString(), FirstName = values[8].ToString() };
            Marine Preparer = new Marine { EDIPI = int.Parse(values[9].ToString()), Rank = values[10].ToString(), LastName = values[11].ToString(), FirstName = values[12].ToString() };
            Marine Member = new Marine { EDIPI = int.Parse(values[13].ToString()), Rank = values[14].ToString(), LastName = values[15].ToString(), FirstName = values[16].ToString() };
            Transaction transaction = new Transaction
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
                Member = Member
            };
            transaction.ErrorDescription = await GetErrorDescription(transaction);
            return transaction;
        }

        private object[] cleanInput(object[] values)
        {
            object[] temp = values;
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
            return $@"{ARUC}\{DiaryYear}\{DiaryNumber}\Transactions.db";
        }

        public string Create()
        {
            throw new NotImplementedException();
        }

        public string Update()
        {
            throw new NotImplementedException();
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }
    }
}
