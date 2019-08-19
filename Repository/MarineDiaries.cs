using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Repository
{
    public class MarineDiaries
    {
        public int EDIPI { get; set; }
        public int DiaryNumber { get; set; }
        public int Aruc { get; set; }
        public int Year { get; set; }

        private async Task<IList<MarineDiaries>> GetLinkedDiaries(string cmdText, UnitDiary diary)
        {
            IList<MarineDiaries> marineDiaries = new List<MarineDiaries>();
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
                            marineDiaries.Add(new MarineDiaries
                            {
                                EDIPI = reader.GetInt32(0),
                                DiaryNumber = reader.GetInt32(1),
                                Aruc = diary.Aruc,
                                Year = diary.Year
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An Error ocurred reading the Database: " + ex.Message.ToString(), "Database Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return marineDiaries;
        }

        public async Task<IList<MarineDiaries>> GetMemberDiaires(Marine marine)
        {
            string CommandText = $@"SELECT EDIPI, DiaryNumber FROM MemberDiaries WHERE EDIPI = {marine.EDIPI};";
            UnitDiary diary = new UnitDiary { Year = AppSettings.Year, Aruc = AppSettings.Aruc };
            return await GetLinkedDiaries(CommandText, diary);
        }
    }
}
