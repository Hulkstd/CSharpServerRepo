using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using NoTitleGameServer.Utility;

namespace NoTitleGameServer.Database
{
    public partial class User
    {
        public string ID   { get; private set; }
        public string PW   { get; private set; }
        public string Name { get; private set; }

        public User(string ID, string PW, string Name)
        {
            this.ID = ID;
            this.PW = PW;
            this.Name = Name;
        }
        public User()
        {
            this.ID = null;
            this.PW = null;
            this.Name = null;
        }
    }

    public partial class User
    {
        public static void AddUser(string ID, string PW, string Name)
        {
            string insertQuery = $"INSERT INTO usertable(ID,PW,Name) VALUES({ID},{PW},{Name})";

            Utility.Utility.connection.Open();
            MySqlCommand command = new MySqlCommand(insertQuery, Utility.Utility.connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {

            }

            Utility.Utility.connection.Close();
        }

        public static User FindUser(string ID)
        {
            User user = new User();
            string selectQuery = $"SELECT * FROM usertable WHERE ID=={ID}";

            MySqlCommand command = new MySqlCommand(selectQuery, Utility.Utility.connection);
            MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                user = new User((string)reader["ID"], (string)reader["PW"], (string)reader["Name"]);
            }
            reader.Close();

            return user;
        }
    }
}
