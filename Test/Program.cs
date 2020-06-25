using System;
using System.Net;
using System.Collections.Generic;
using FreeNet;
using NoTitleGameServer.Utility;
using NoTitleGameServer;

namespace Test
{
    partial class Program
    {
        static IPeer server;
        static CFreeNetEventManager event_manager;
        static CFreeNetUnityService gameserver;
        delegate void PRINT();
        delegate void ACTION(string n);
        static PRINT print;
        static ACTION action;

        public static CPacket RoomList;
        public static CPacket MakingRoom;
        public static CPacket EnterRoom;
        public static CPacket StartGame;
        public static CPacket Register;
        public static CPacket Login;
        public static bool wait = false;

        /* 페킷 테스트 코드
        static void Main(string[] args)
        {
            {
                CPacketBufferManager.initialize(2000);
                RoomList = CPacket.create((short)PROTOCOL.REQ_ROOMLIST);
                MakingRoom = CPacket.create((short)PROTOCOL.REQ_MAKINGROOM);
                MakingRoom.push("ID");
                MakingRoom.push("Title");
                MakingRoom.push("192.168.10.1");
                EnterRoom = CPacket.create((short)PROTOCOL.REQ_ENTERROOM);
                EnterRoom.push("ID");
                StartGame = CPacket.create((short)PROTOCOL.REQ_STARTGAME);
                StartGame.push("ID");
                Register = CPacket.create((short)PROTOCOL.REQ_REGISTER);
                Register.push("test2");
                Register.push("test");
                Register.push("TEST");
                Login = CPacket.create((short)PROTOCOL.REQ_LOGIN);
                Login.push("test2");
                Login.push("test");
            }

            gameserver = new CFreeNetUnityService();
            gameserver.appcallback_on_message += Process_Packet;

            gameserver.connect("127.0.0.1", 7979);

            while(true)
            {
                Console.ReadKey();

                //send(RoomList);
                //send(MakingRoom);
                //send(EnterRoom);
                //send(StartGame);
                //send(Register);
                //send(Login);

                System.Threading.Thread.Sleep(10000);
            }

            Console.ReadKey();
        }
        */

        static void Main(string[] args)
        {
            gameserver = new CFreeNetUnityService();
            gameserver.appcallback_on_message += Process_Packet;

            gameserver.connect("127.0.0.1", 7979);


            print = Menu;
            action = MenuAction;

            while (true)
            {
                Console.Clear();
                print();
                var line = Console.ReadLine();

                action(line);
            }
        }

        static void on_connected_gameserver(CUserToken server_token)
        {
            Console.WriteLine("Connected");

            server = new CRemoteServerPeer(server_token);
            (server as CRemoteServerPeer).onmessage += Process_Packet;
            (server as CRemoteServerPeer).set_eventmanager(event_manager);

            event_manager.enqueue_network_event(NETWORK_EVENT.connected);
        }

        /* 패킷 태스트 코드
        public static void Process_Packet(CPacket packet)
        {
            var protocol = (PROTOCOL)packet.pop_protocol_id();

            Console.WriteLine("protocol :" + protocol.ToString());
            switch (protocol)
            {
                case PROTOCOL.SEND_ROOMLIST:
                    {
                        var List = Utility.RoomListToDictionary(packet);

                        foreach (KeyValuePair<string, GameRoom> keyValue in List)
                        {
                            Console.WriteLine(keyValue.Value.RoomTitle);
                        }
                    }
                    break;

                case PROTOCOL.DEC_MAKINGROOM:
                    {
                        var result = Convert.ToBoolean(packet.pop_int16());

                        Console.WriteLine(result);
                    }
                    break;

                case PROTOCOL.DEC_ENTERROOM:
                    {
                        var result = Convert.ToBoolean(packet.pop_int16());

                        Console.WriteLine(result);
                    }
                    break;

                case PROTOCOL.DEC_STARTGAME:
                    {
                        var result = Convert.ToBoolean(packet.pop_int16());

                        Console.WriteLine(result);
                    }
                    break;

                case PROTOCOL.DEC_REGISTER:
                    {
                        var flag = Convert.ToBoolean(packet.pop_byte());

                        if(!flag)
                        {
                            var error = (RegisterFailure)packet.pop_int16();

                            Console.WriteLine($"REGISTER FAIL CAUSE {error}");
                        }
                        else
                        {
                            Console.WriteLine($"REGISTER SUCCESS");
                        }
                    }
                    break;

                case PROTOCOL.DEC_LOGIN:
                    {
                        var flag = Convert.ToBoolean(packet.pop_byte());

                        if (!flag)
                        {
                            var error = (LoginFailure)packet.pop_int16();

                            Console.WriteLine($"LOGIN FAIL CAUSE {error}");
                        }
                        else
                        {
                            Console.WriteLine($"LOGIN SUCCESS");
                        }
                    }
                    break;
            }
        }
        */

        public static void Process_Packet(CPacket msg)
        {
            var protocol = (PROTOCOL)msg.pop_protocol_id();

            //Console.WriteLine("protocol :" + protocol.ToString());
            switch(protocol)
            {
                case PROTOCOL.DEC_REGISTER:
                    {
                        var flag = Convert.ToBoolean(msg.pop_byte());
                        wait = false;

                        if (!flag)
                        {
                            var exist = (RegisterFailure)msg.pop_int16();
                            
                            Console.WriteLine(exist);
                        }
                        else
                        {
                            Console.WriteLine("회원가입 성공");

                            print = Menu;
                            action = MenuAction;
                        }
                    }
                    break;

                case PROTOCOL.DEC_LOGIN:
                    {
                        var flag = Convert.ToBoolean(msg.pop_byte());
                        wait = false;

                        if (!flag)
                        {
                            var exist = (LoginFailure)msg.pop_int16();

                            Console.WriteLine(exist);
                        }
                        else
                        {
                            Console.WriteLine("로그인 성공");

                            print = RoomMenu;
                            action = RoomMenuAction;
                        }
                    }
                    break;

                case PROTOCOL.SEND_ROOMLIST:
                    {
                        roomList = Utility.RoomListToDictionary(msg);
                        wait = false;
                    }
                    break;

                case PROTOCOL.DEC_MAKINGROOM:
                    {
                        var flag = Convert.ToBoolean(msg.pop_byte());

                        wait = false;

                        if(flag)
                        {
                            IsMaster = true;
                            print = InRoomMenu;
                            action = InRoomMenuAction;
                            Console.WriteLine("방 제작 성공");
                        }
                        else
                        {
                            Console.WriteLine("방 제작 실패");
                        }
                    }
                    break;

                case PROTOCOL.DEC_ENTERROOM:
                    {
                        var flag = Convert.ToBoolean(msg.pop_byte());

                        if(flag)
                        {
                            print = InRoomMenu;
                            action = InRoomMenuAction;
                            wait = false;
                        }
                        else
                        {
                            Console.WriteLine("방 입장 실패");
                        }
                    }
                    break;

                case PROTOCOL.SEND_ENTERROOM:
                    {
                        Names.Add(msg.pop_string());
                        Master.Add(Convert.ToBoolean(msg.pop_byte()));
                    }
                    break;

                case PROTOCOL.DEC_STARTGAME:
                    {
                        var flag = Convert.ToBoolean(msg.pop_byte());

                        if(flag)
                        {
                            Console.WriteLine("게임 시작");
                            Console.WriteLine("{0}", masterIP);
                        }
                        else
                        {
                            Console.WriteLine("인원수 부족");
                        }
                    }
                    break;

                case PROTOCOL.SEND_NICKNAME:
                    {
                        string name = msg.pop_string();
                    }
                    break;
            }
        }

        public static void send(CPacket msg)
        {
            gameserver.send(msg);
        }
    }

    /// <summary>
    /// 회원가입 관련 함수 및 변수
    /// </summary>
    partial class Program
    {
        static void Menu()
        {
            Console.WriteLine($"┌──────────────────────────────────────────┐");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│               1. Register                │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│               2. Login                   │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"└──────────────────────────────────────────┘");
        }

        static void MenuAction(string n)
        {
            int num = int.Parse(n);
            if(num == 1)
            {
                print = RegisterMenu;
                action = RegisterMenuAction;
            }
            else if(num == 2)
            {
                print = LoginMenu;
                action = LoginMenuAction;
            }
        }

        static void RegisterMenu()
        {
            Console.WriteLine($"             Register             ");
            Console.WriteLine($" ID PW Name 순으로 한줄에 입력");
        }

        static void RegisterMenuAction(string line)
        {
            string data = line;

            string ID = data.Split(' ')[0];
            string PW = data.Split(' ')[1];
            string Name = data.Split(' ')[2];

            CPacket packet = CPacket.create((short)PROTOCOL.REQ_REGISTER);
            packet.push(ID);
            packet.push(PW);
            packet.push(Name);

            send(packet);

            wait = true;

            while(wait)
            {
                System.Threading.Thread.Sleep(10);
            }

            Console.ReadKey();
        }
    }

    /// <summary>
    /// 로그인 관련 함수 및 변수
    /// </summary>
    partial class Program
    {
        static string ID;

        static void LoginMenu()
        {
            Console.WriteLine($"             Login             ");
            Console.WriteLine($" ID PW 순으로 한줄에 입력");
        }

        static void LoginMenuAction(string line)
        {
            string data = line;

            string ID = data.Split(' ')[0];
            string PW = data.Split(' ')[1];
            Program.ID = ID;

            CPacket packet = CPacket.create((short)PROTOCOL.REQ_LOGIN);
            packet.push(ID);
            packet.push(PW);

            send(packet);

            wait = true;

            while (wait)
            {
                System.Threading.Thread.Sleep(10);
            }

            Console.ReadKey();
        }
    }

    /// <summary>
    /// 로그인 이후 행동 표시.
    /// </summary>
    partial class Program
    {
        static void RoomMenu()
        {
            Console.WriteLine($"┌──────────────────────────────────────────┐");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│               1. Room List               │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│               2. Making Room             │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"│                                          │");
            Console.WriteLine($"└──────────────────────────────────────────┘");
        }
        
        static void RoomMenuAction(string line)
        {
            int n = int.Parse(line);

            switch(n)
            {
                case 1:
                    {
                        print = RoomListMenu;
                        action = RoomListMenuAction;
                    }
                    break;

                case 2:
                    {
                        print = MakingRoomMenu;
                        action = MakingRoomMenuAction;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 방 참가를 위한 방 리스트 출력 및 잠가.
    /// </summary>
    partial class Program
    {
        static Dictionary<string, GameRoom> roomList;
        static Dictionary<int, string> roomIndex = new Dictionary<int, string>();
        static string masterIP;

        static void RoomListMenu()
        {
            CPacket packet = CPacket.create((short)PROTOCOL.REQ_ROOMLIST);
            send(packet);
            
            wait = true;

            while(wait)
            {
                System.Threading.Thread.Sleep(10);
            }
            
            Console.WriteLine("{0,2}{1,6}{2,20}\n\n", "No.", "Title", "Info");
            int i = 1;
            roomIndex.Clear();
            foreach(var room in roomList)
            {
                Console.WriteLine("{0,2}{1,6}{2,20}\n", i, room.Value.RoomTitle, $"{room.Value.CntPlayer}/{GameRoom.MAX_PLAYER}");
                roomIndex.Add(i, room.Key);
                i++;
            }

            Console.WriteLine("1. 참가");
            Console.WriteLine("2. 새로고침");
        }

        static void RoomListMenuAction(string line)
        {
            switch(line)
            {
                case "1":
                    {
                        Console.Write("방 번호 입력 : ");
                        int num = int.Parse(Console.ReadLine());

                        CPacket packet = CPacket.create((short)PROTOCOL.REQ_ENTERROOM);
                        packet.push(roomIndex[num]);
                        packet.push(ID);
                        packet.push(GetLocalIP());
                        masterIP = roomList[roomIndex[num]].MasterIP;

                        send(packet);
                        wait = true;

                        while(wait)
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                    break;

                case "2":
                    {
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 방 제작을 위한 정보 입력창.
    /// </summary>
    partial class Program
    {
        static bool IsMaster;

        static void MakingRoomMenu()
        {
            Console.WriteLine("방 이름을 입력해 주세요.");
        }

        static void MakingRoomMenuAction(string line)
        {
            CPacket packet = CPacket.create((short)PROTOCOL.REQ_MAKINGROOM);

            packet.push(ID);
            packet.push(line);
            packet.push(GetLocalIP());

            send(packet);

            wait = true;

            while(wait)
            {
                System.Threading.Thread.Sleep(10);
            }

            Console.ReadKey();
        }

        static string GetLocalIP()
        {
            string myip = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(var ip in host.AddressList)
            {
                if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    myip = ip.ToString();
                }
            }

            return myip;
        }
    }

    /// <summary>
    /// 방 참가후 플레이어들 출력.
    /// </summary>
    partial class Program
    {
        static List<string> Names = new List<string>();
        static List<bool> Master = new List<bool>();

        static void InRoomMenu()
        {
            int i = 1;
            Console.WriteLine("{0,2}{1,6}{2,20}\n\n", "No.", "Name", "Info");

            for(int j = 0; j < Names.Count; j++)
            { 
                Console.WriteLine("{0,2}{1,6}{2,20}\n\n", i++, Names[j], Master[j] ? "Master" : "User");
            }
            for(; i <= 4; i++)
            {
                Console.WriteLine("{0,2}{1,6}\n\n", i, "Wait for connection");
            }

            Console.WriteLine(" 1. 새로고침");
            if (IsMaster) Console.WriteLine(" 2. 게임시작");
        }

        static void InRoomMenuAction(string line)
        {
            if(line == "2")
            {
                CPacket packet = CPacket.create((short)PROTOCOL.REQ_STARTGAME);
                packet.push(ID);

                send(packet);

                while(wait)
                {
                    System.Threading.Thread.Sleep(10);
                }

                Console.ReadKey();
            }
        }
    }
}
