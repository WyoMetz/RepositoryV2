using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class ESRInsert
    {
        public async Task InsertTransactions(IList<Transaction> transactions)
        {
            IList<Transaction> esrTransactions = transactions.Where(o => o.DocumentRequired != string.Empty).ToList();
            SQLiteConnection connection = await new Database(new ESRTransaction()).Connect();
            try
            {
                SQLiteCommand cmd = connection.CreateCommand();
                SQLiteTransaction transaction = connection.BeginTransaction();
                cmd.CommandText = @"INSERT INTO ESRTransactions 
                                        (DiaryNumber, DiaryYear, ARUC, Branch, 
                                         Certifier, CertifierRank, CertifierFirstName, CertifierLastName, CertifierMI, 
                                         Preparer, PreparerRank, PreparerFirstName, PreparerLastName, PreparerMI, 
                                         Member, MemberRank, MemberFirstName, MemberLastName, MemberMI, 
                                         TTC, TTS, EnglishStatement, HistoryStatement, TransactionErrorCode, DocumentRequired, ConfirmDate, ApproverDate, RejectDate)
                                         VALUES
                                        (@DiaryNumber, @DiaryYear, @ARUC, @Branch, @Certifier, 
                                         @CertifierRank, @CertifierFirstName, @CertifierLastName, @CertifierMI,
                                         @Preparer, @PreparerRank, @PreparerFirstName, @PreparerLastName, @PreparerMI,
                                         @Member, @MemberRank, @MemberFirstName, @MemberLastName, @MemberMI, 
                                         @TTC, @TTS, @EnglishStatement, @HistoryStatement, @TransactionErrorCode, @DocumentRequired, @ConfirmDate, @ApproverDate, @RejectDate);";
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
                cmd.Parameters.AddWithValue("@ConfirmDate", "");
                cmd.Parameters.AddWithValue("@ApproverDate", "");
                cmd.Parameters.AddWithValue("@RejectDate", "");
                foreach (var trans in esrTransactions)
                {
                    Console.WriteLine($"Inserting Transaction for {trans.DiaryNumber}, {trans.TTC}, {trans.TTS}");
                    cmd.Parameters["@DiaryNumber"].Value = trans.DiaryNumber;
                    cmd.Parameters["@DiaryYear"].Value = trans.DiaryYear;
                    cmd.Parameters["@ARUC"].Value = trans.ARUC;
                    cmd.Parameters["@Branch"].Value = trans.Branch;
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
                    cmd.Parameters["@ConfirmDate"].Value = DateTime.Now;
                    cmd.Parameters["@ApproverDate"].Value = DateTime.Now;
                    cmd.Parameters["@RejectDate"].Value = DateTime.Now;
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
                cmd.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred during ESR SQL Execution: " + ex.Message.ToString(), "Insert Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
