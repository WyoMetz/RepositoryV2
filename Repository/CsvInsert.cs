using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Repository
{
    public class CsvInsert
    {
        CSVCollection collection { get; set; }
        public CsvInsert(CSVCollection CsvParse)
        {
            collection = CsvParse;
        }

        public async Task InsertRepositoryInfo()
        {
            if (collection != null)
            {
                SQLiteConnection connection = await new Database().Connect();
                try
                {
                    SQLiteCommand cmd = connection.CreateCommand();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    cmd.CommandText = @"INSERT INTO Marines (EDIPI, Rank, FirstName, LastName, MI, PRUC) VALUES (@EDIPI, @Rank, @FirstName, @LastName, @MI, @PRUC);";
                    cmd.Parameters.AddWithValue("@EDIPI", "");
                    cmd.Parameters.AddWithValue("@Rank", "");
                    cmd.Parameters.AddWithValue("@FirstName", "");
                    cmd.Parameters.AddWithValue("@LastName", "");
                    cmd.Parameters.AddWithValue("@MI", "");
                    cmd.Parameters.AddWithValue("@PRUC", "");
                    foreach (var marine in collection.Marines)
                    {
                        //Console.Clear();
                        //Console.WriteLine($"Inserting Marine {marine.EDIPI}");
                        cmd.Parameters["@EDIPI"].Value = marine.EDIPI;
                        cmd.Parameters["@Rank"].Value = marine.Rank;
                        cmd.Parameters["@FirstName"].Value = marine.FirstName;
                        cmd.Parameters["@LastName"].Value = marine.LastName;
                        cmd.Parameters["@MI"].Value = marine.MI;
                        cmd.Parameters["@PRUC"].Value = marine.PRUC;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    cmd.Dispose();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "Insert Marine Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                IList<int> arucs = await new Database().GetArucs();
                IList<int> years = await new Database().GetYears();
                foreach (var aruc in collection.Arucs)
                {
                    if (!arucs.Contains(aruc))
                    {
                        await UpdateDatabase($@"INSERT INTO ARUCS (ARUC, DateAdded) VALUES ('{aruc}', '{DateTime.Now}');");
                    }
                }
                foreach (var year in collection.Years)
                {
                    if (!years.Contains(year))
                    {
                        await UpdateDatabase($@"INSERT INTO Years (Year, DateAdded) VALUES ('{year}', '{DateTime.Now}');");
                    }
                }
            }
        }

        public async Task InsertDiaryInfo()
        {
            if(collection != null)
            {
                await CreateFolders();
                foreach (var aruc in collection.Arucs)
                {
                    foreach (var year in collection.Years)
                    {
                        await DiaryInsert(aruc, year);
                        await TransactionInsert(aruc, year);
                    }
                }
            }
        }

        private async Task UpdateDatabase(string statement)
        {
            SQLiteConnection connection = await new Database().Connect();
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
                MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "SQL Update Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CreateFolders()
        {
            string DefaultLocation = new AppSettings().DatabaseLocation;
            if(collection.Arucs != null)
            {
                foreach (var aruc in collection.Arucs)
                {
                    if(!Directory.Exists($@"{DefaultLocation}\{aruc}"))
                    {
                        Directory.CreateDirectory($@"{DefaultLocation}\{aruc}");
                    }
                    if (collection.Years != null)
                    {
                        foreach (var year in collection.Years)
                        {
                            if (!Directory.Exists($@"{DefaultLocation}\{aruc}\{year}"))
                            {
                                Directory.CreateDirectory($@"{DefaultLocation}\{aruc}\{year}");
                                FileSystem.CopyFile($@"{DefaultLocation}\Seed\Diaries.db", $@"{DefaultLocation}\{aruc}\{year}\Diaries.db");
                                FileSystem.CopyFile($@"{DefaultLocation}\Seed\ESR.db", $@"{DefaultLocation}\{aruc}\{year}\ESR.db");
                            }
                            foreach (var diary in collection.Diaries.Where(o => o.Aruc == aruc && o.Year == year).ToList())
                            {
                                if (!Directory.Exists($@"{DefaultLocation}\{aruc}\{year}\{diary.Number}"))
                                {
                                    Directory.CreateDirectory($@"{DefaultLocation}\{aruc}\{year}\{diary.Number}");
                                    FileSystem.CopyFile($@"{DefaultLocation}\Seed\Transactions.db", $@"{DefaultLocation}\{aruc}\{year}\{diary.Number}\Transactions.db");
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task DiaryInsert(int aruc, int year)
        {
            IList<UnitDiary> diaries = collection.Diaries.Where(o => o.Aruc == aruc && o.Year == year).ToList();
            IList<MarineDiaries> memberDiaries = collection.MemberDiaries.Where(o => o.Aruc == aruc && o.Year == year).ToList();
            IList<MarineDiaries> certifierDiaries = collection.CertifierDiaries.Where(o => o.Aruc == aruc && o.Year == year).ToList();
            IList<MarineDiaries> preparerDiaries = collection.PreparerDiaries.Where(o => o.Aruc == aruc && o.Year == year).ToList();
            SQLiteConnection connection = await new Database(diaries.First()).Connect();
            try
            {
                SQLiteCommand cmd = connection.CreateCommand();
                SQLiteTransaction transaction = connection.BeginTransaction();
                cmd.CommandText = @"INSERT INTO Diaries (DiaryNumber, Aruc, DiaryYear, DiaryDate, CycleDate, CycleNumber, Branch, Certifier, Uploaded, ReportDate, Confirmed) VALUES (@DiaryNumber, @Aruc, @DiaryYear, @DiaryDate, @CycleDate, @CycleNumber, @Branch, @Certifier, @Uploaded, @ReportDate, @Confirmed);";
                cmd.Parameters.AddWithValue("@DiaryNumber", "");
                cmd.Parameters.AddWithValue("@Aruc", "");
                cmd.Parameters.AddWithValue("@DiaryYear", "");
                cmd.Parameters.AddWithValue("@DiaryDate", "");
                cmd.Parameters.AddWithValue("@CycleDate", "");
                cmd.Parameters.AddWithValue("@CycleNumber", "");
                cmd.Parameters.AddWithValue("@Branch", "");
                cmd.Parameters.AddWithValue("@Uploaded", "");
                cmd.Parameters.AddWithValue("@ReportDate", "");
                cmd.Parameters.AddWithValue("@Certifier", "");
                cmd.Parameters.AddWithValue("@Confirmed", false);
                foreach (var diary in diaries)
                {
                    //Console.Clear();
                    Console.WriteLine($"Inserting {diary.Number}");
                    cmd.Parameters["@DiaryNumber"].Value = diary.Number;
                    cmd.Parameters["@Aruc"].Value = diary.Aruc;
                    cmd.Parameters["@DiaryYear"].Value = diary.Year;
                    cmd.Parameters["@DiaryDate"].Value = diary.Date;
                    cmd.Parameters["@CycleDate"].Value = diary.CycleDate;
                    cmd.Parameters["@CycleNumber"].Value = diary.CycleNumber;
                    cmd.Parameters["@Branch"].Value = diary.Branch;
                    cmd.Parameters["@Uploaded"].Value = "False";
                    cmd.Parameters["@ReportDate"].Value = DateTime.Now;
                    cmd.Parameters["@Certifier"].Value = diary.Certifier;
                    cmd.ExecuteNonQuery();
                }
                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO MemberDiaries (EDIPI, DiaryNumber) VALUES (@EDIPI, @DiaryNumber);";
                cmd.Parameters.AddWithValue("@EDIPI", "");
                cmd.Parameters.AddWithValue("@DiaryNumber", "");
                foreach (var marine in memberDiaries)
                {
                    cmd.Parameters["@EDIPI"].Value = marine.EDIPI;
                    cmd.Parameters["@DiaryNumber"].Value = marine.DiaryNumber;
                    cmd.ExecuteNonQuery();
                }
                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO PreparerDiaries (EDIPI, DiaryNumber) VALUES (@EDIPI, @DiaryNumber);";
                cmd.Parameters.AddWithValue("@EDIPI", "");
                cmd.Parameters.AddWithValue("@DiaryNumber", "");
                foreach (var marine in preparerDiaries)
                {
                    cmd.Parameters["@EDIPI"].Value = marine.EDIPI;
                    cmd.Parameters["@DiaryNumber"].Value = marine.DiaryNumber;
                    cmd.ExecuteNonQuery();
                }
                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO CertifierDiaries (EDIPI, DiaryNumber) VALUES (@EDIPI, @DiaryNumber);";
                cmd.Parameters.AddWithValue("@EDIPI", "");
                cmd.Parameters.AddWithValue("@DiaryNumber", "");
                foreach (var marine in certifierDiaries)
                {
                    cmd.Parameters["@EDIPI"].Value = marine.EDIPI;
                    cmd.Parameters["@DiaryNumber"].Value = marine.DiaryNumber;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
                cmd.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "Insert Diaries Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TransactionInsert(int aruc, int year)
        {
            IList<Transaction> transactions = collection.Transactions.Where(o => o.ARUC == aruc && o.DiaryYear == year).OrderBy(o => o.DiaryNumber).ToList();
            foreach (var diary in collection.Diaries.Where(o => o.Aruc == aruc && o.Year == year).OrderBy(o => o.Number).ToList())
            {
                SQLiteConnection connection = await new Database(transactions.Where(o => o.DiaryNumber == diary.Number).ToList().First()).Connect();
                try
                {
                    SQLiteCommand cmd = connection.CreateCommand();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    cmd.CommandText = @"INSERT INTO Transactions 
                                        (DiaryNumber, DiaryYear, ARUC, Branch, 
                                         Certifier, CertifierRank, CertifierFirstName, CertifierLastName, CertifierMI, 
                                         Preparer, PreparerRank, PreparerFirstName, PreparerLastName, PreparerMI, 
                                         Member, MemberRank, MemberFirstName, MemberLastName, MemberMI, 
                                         TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, DocumentRequired)
                                         VALUES
                                        (@DiaryNumber, @DiaryYear, @ARUC, @Branch, @Certifier, 
                                         @CertifierRank, @CertifierFirstName, @CertifierLastName, @CertifierMI,
                                         @Preparer, @PreparerRank, @PreparerFirstName, @PreparerLastName, @PreparerMI,
                                         @Member, @MemberRank, @MemberFirstName, @MemberLastName, @MemberMI, 
                                         @TTC, @TTS, @EnglishStatement, @HistoryStatement, @TransactionErrorCode, @DocumentRequired);";
                    cmd.Parameters.AddWithValue("@DiaryNumber", "");
                    cmd.Parameters.AddWithValue("@DiaryYear", "");
                    cmd.Parameters.AddWithValue("@ARUC", "");
                    cmd.Parameters.AddWithValue("@Branch", "");
                    cmd.Parameters.AddWithValue("@Certifier", "");
                    cmd.Parameters.AddWithValue("@CertifierRank", "");
                    cmd.Parameters.AddWithValue("@CertifierFirstName", "");
                    cmd.Parameters.AddWithValue("@CertifierLastName", "");
                    cmd.Parameters.AddWithValue("@CertifierMI", "");
                    cmd.Parameters.AddWithValue("@Preparer", "");
                    cmd.Parameters.AddWithValue("@PreparerRank", "");
                    cmd.Parameters.AddWithValue("@PreparerFirstName", "");
                    cmd.Parameters.AddWithValue("@PreparerLastName", "");
                    cmd.Parameters.AddWithValue("@PreparerMI", "");
                    cmd.Parameters.AddWithValue("@Member", "");
                    cmd.Parameters.AddWithValue("@MemberRank", "");
                    cmd.Parameters.AddWithValue("@MemberFirstName", "");
                    cmd.Parameters.AddWithValue("@MemberLastName", "");
                    cmd.Parameters.AddWithValue("@MemberMI", "");
                    cmd.Parameters.AddWithValue("@TTC", "");
                    cmd.Parameters.AddWithValue("@TTS", "");
                    cmd.Parameters.AddWithValue("@EnglishStatement", "");
                    cmd.Parameters.AddWithValue("@HistoryStatement", "");
                    cmd.Parameters.AddWithValue("@TransactionErrorCode", "");
                    cmd.Parameters.AddWithValue("@DocumentRequired", "");
                    foreach (var trans in transactions.Where(o => o.DiaryNumber == diary.Number).ToList())
                    {
                        Console.WriteLine($"Inserting Transaction for {trans.DiaryNumber}, {trans.TTC}, {trans.TTS}");
                        cmd.Parameters["@DiaryNumber"].Value = diary.Number;
                        cmd.Parameters["@DiaryYear"].Value = diary.Year;
                        cmd.Parameters["@ARUC"].Value = trans.ARUC;
                        cmd.Parameters["@Branch"].Value = diary.Branch;
                        cmd.Parameters["@Certifier"].Value = trans.Certifier.EDIPI;
                        cmd.Parameters["@CertifierRank"].Value = trans.Certifier.Rank;
                        cmd.Parameters["@CertifierFirstName"].Value = trans.Certifier.FirstName;
                        cmd.Parameters["@CertifierLastName"].Value = trans.Certifier.LastName;
                        cmd.Parameters["@CertifierMI"].Value = trans.Certifier.MI;
                        cmd.Parameters["@Preparer"].Value = trans.Preparer.EDIPI;
                        cmd.Parameters["@PreparerRank"].Value = trans.Preparer.Rank;
                        cmd.Parameters["@PreparerFirstName"].Value = trans.Preparer.FirstName;
                        cmd.Parameters["@PreparerLastName"].Value = trans.Preparer.LastName;
                        cmd.Parameters["@PreparerMI"].Value = trans.Preparer.MI;
                        cmd.Parameters["@Member"].Value = trans.Member.EDIPI;
                        cmd.Parameters["@MemberRank"].Value = trans.Member.Rank;
                        cmd.Parameters["@MemberFirstName"].Value = trans.Member.FirstName;
                        cmd.Parameters["@MemberLastName"].Value = trans.Member.LastName;
                        cmd.Parameters["@MemberMI"].Value = trans.Member.MI;
                        cmd.Parameters["@TTC"].Value = trans.TTC;
                        cmd.Parameters["@TTS"].Value = trans.TTS;
                        cmd.Parameters["@EnglishStatement"].Value = trans.EnglishStatement;
                        cmd.Parameters["@HistoryStatement"].Value = trans.HistoryStatement;
                        cmd.Parameters["@TransactionErrorCode"].Value = trans.TransactionErrorCode;
                        cmd.Parameters["@DocumentRequired"].Value = trans.DocumentRequired;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    cmd.Dispose();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An Error ocurred during SQL Execution: " + ex.Message.ToString(), "Insert Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
