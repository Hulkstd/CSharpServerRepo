using System;
using System.Collections.Generic;
using FreeNet;

namespace NoTitleGameServer
{
    class Program
    {
        static List<Player> userList;
        public static GameServer gameServer;

        static void Main(string[] args)
        {
            {
                Console.ForegroundColor = ConsoleColor.Black;
                string Pwd = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Utility.Utility.connection = new MySql.Data.MySqlClient.MySqlConnection($"Server=localhost;Database=user;Uid=root;Pwd={Pwd};Charset=utf8");
                Utility.Utility.GameRoomManager = new GameRoomManager();
                Console.Clear();
            }
            CPacketBufferManager.initialize(2000);
            userList = new List<Player>();

            CNetworkService service = new CNetworkService();
            service.session_created_callback += on_session_created;
            service.initialize();
            service.listen("0.0.0.0", 7979, 100);

            gameServer = new GameServer();

            Console.WriteLine("Started!! GameServer");
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }

            Console.ReadKey();
        }

        static void on_session_created(CUserToken token)
        {
            Player user = new Player(token);
            lock (userList)
            {
                userList.Add(user);
            }
        }

        public static void remove_user(Player player)
        {
            lock (userList)
            {
                userList.Remove(player);
                gameServer.user_disconnected(player);
            }
        }
    }
}
