using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Repository
{
    public class User : ITransactable, IStorable
    {
        public string UserName { get; set; }
        public string PrimaryHue { get; set; }
        public string AccentColor { get; set; }
        public string BackgroundPath { get; set; }
        public bool UsesDarkTheme { get; set; }
        public int Aruc { get; set; }

        public string CurrentFilePath { get; set; }

        public User()
        {
            UserName = AppSettings.User;
        }

        public string Create()
        {
            return $@"INSERT INTO UserTable (UserName, Background, PrimaryColor, SecondaryColor, UsesDarkTheme, Aruc) VALUES ('{UserName}', '{BackgroundPath}', '{PrimaryHue}', '{AccentColor}', '{UsesDarkTheme}', '{Aruc}');";
        }

        public string Delete()
        {
            throw new NotImplementedException();
        }

        public string FileStoragePath()
        {
            return $@"{Aruc}\UserFiles\{UserName}\{Path.GetFileName(CurrentFilePath)}";
        }

        public string Update()
        {
            return $@"UPDATE UserTable SET Background='{BackgroundPath}', PrimaryColor='{PrimaryHue}', SecondaryColor='{AccentColor}', UsesDarkTheme='{UsesDarkTheme}', Aruc = '{Aruc}' WHERE UserName = '{UserName}';";
        }

        public async Task<User> GetUserSettings()
        {
            User user = new User();
            int userAruc = 0;
            SQLiteConnection connection = await new Database().Connect();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = $@"SELECT PrimaryColor, SecondaryColor, UsesDarkTheme, Background, Aruc FROM UserTable WHERE UserName = '{UserName}';";
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int.TryParse(reader.GetValue(4).ToString(), out userAruc);
                        user.PrimaryHue = reader.GetString(0);
                        user.AccentColor = reader.GetString(1);
                        user.UsesDarkTheme = bool.Parse(reader.GetString(2));
                        user.BackgroundPath = reader.GetString(3);
                        user.Aruc = userAruc;
                    }
                }
            }
            return user;
        }

        public async Task<IList<User>> GetUsers()
        {
            IList<User> users = new List<User>();
            SQLiteConnection connection = await new Database().Connect();
            using (SQLiteCommand cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = $@"SELECT UserName, PrimaryColor, SecondaryColor, UsesDarkTheme, Background, Aruc FROM UserTable;";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserName = reader.GetString(0),
                            PrimaryHue = reader.GetString(1),
                            AccentColor = reader.GetString(2),
                            UsesDarkTheme = bool.Parse(reader.GetString(3)),
                            BackgroundPath = reader.GetString(4),
                            Aruc = reader.GetInt32(5)
                        });
                    }
                }
            }
            return users;
        }

        public string DatabaseConnection()
        {
            return $@"\Repository.db";
        }
    }
}
