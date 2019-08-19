using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Comment : ITransactable
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CommentText { get; set; }
        public int TransactionID { get; set; }
        public int BatchID { get; set; }

        public string Create()
        {
            return $@"INSERT INTO Comments (CreatedBy, CreatedOn, CommentText, TransactionID, BatchID) VALUES ('{CreatedBy}', '{CreatedOn}', '{CommentText}', '{TransactionID}', '{BatchID}');";
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
            throw new NotImplementedException();
        }

        public async Task<IList<Comment>> GetTransactionComments(ESRTransaction transaction)
        {
            string cmd = $@"SELECT CreatedBy, CreatedOn, CommentText, TransactionID, BatchID FROM Comments WHERE TransactionID = '{transaction.ID}' ORDER BY CreatedOn ASC;";
            return await GetComments(cmd);
        }

        public async Task<IList<Comment>> GetBatchComments(Batch batch)
        {
            string cmd = $@"SELECT CreatedBy, CreatedOn, CommentText, TransactionID, BatchID FROM Comments WHERE BatchID = '{batch.ID}' ORDER BY CreatedOn ASC;";
            return await GetComments(cmd);
        }

        private async Task<IList<Comment>> GetComments(string cmdText)
        {
            Comment comment = new Comment();
            IList<Comment> comments = new List<Comment>();
            SQLiteConnection connection = await new Database(comment).Connect();
            using(SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = cmdText;
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new Comment
                        {
                            CreatedBy = reader.GetString(0),
                            CreatedOn = reader.GetDateTime(1),
                            CommentText = reader.GetString(2),
                            TransactionID = reader.GetInt32(3),
                            BatchID = reader.GetInt32(4)
                        });
                    }
                }
            }
            return comments;
        }
    }
}
