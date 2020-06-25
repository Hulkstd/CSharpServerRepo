using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using NoTitleGameServer.Utility;
using FreeNet;

namespace NoTitleGameServer.Database
{
    [Serializable]
    public partial class User
    {
        public string ID   { get; private set; }
        public string PW   { get; private set; }
        public string Name { get; private set; }

        public CUserToken token; 

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

    public static class UserManager
    {
        private static Dictionary<string, User> UserList = new Dictionary<string, User>();

        public static bool AddUser(string ID, string PW, string Name, ref RegisterFailure Error)
        {
            string insertQuery = $"INSERT INTO usertable(ID,PW,Name) VALUES('{ID}','{PW}','{Name}')";

            Utility.Utility.connection.Open();
            MySqlCommand command = new MySqlCommand(insertQuery, Utility.Utility.connection);

            try
            {
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                _ = e.Message.Contains("PRIMARY") ? Error |= RegisterFailure.ID_ALREADY_EXISTS : Error = Error;
                _ = e.Message.Contains("Name_UNIQUE") ? Error |= RegisterFailure.NICKNAME_ALREADY_EXISTS : Error = Error;
                Utility.Utility.connection.Close();
                return false;
            }

            Utility.Utility.connection.Close();
            return true;
        }

        public static User FindUser(string ID)
        {
            if (UserList.ContainsKey(ID)) return UserList[ID];

            User user = new User();
            string selectQuery = $"SELECT * FROM usertable WHERE ID='{ID}'";

            Utility.Utility.connection.Open();
            MySqlCommand command = new MySqlCommand(selectQuery, Utility.Utility.connection);
            MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                user = new User((string)reader["ID"], (string)reader["PW"], (string)reader["Name"]);
            }
            reader.Close();
            Utility.Utility.connection.Close();

            if (user.ID != null) UserList.Add(ID, user);

            return user;
        }
    }
}
