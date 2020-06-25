using System;
using System.Net;
using FreeNet;

namespace CSampleClient
{
    using GameServer;
    using System.Collections.Generic;

    class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        void IPeer.on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);
            PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

            switch(protocol_id)
            {
                case PROTOCOL.CHAT_MSG_ACK:
                    {
                        string text = msg.pop_string();
                        Console.WriteLine(string.Format("text {0}", text));
                    }
                    break;
            }
        }

        void IPeer.on_removed()
        {
            Console.WriteLine("Server removed");
        }

        void IPeer.send(CPacket msg)
        {
            this.token.send(msg);
        }

        void IPeer.disconnect()
        {
            this.token.socket.Disconnect(false);
        }

        void IPeer.process_user_operation(CPacket msg)
        {

        }
    }

    class Program
    {
        static List<IPeer> game_servers = new List<IPeer>();

        static void Main(string[] args)
        {
            CPacketBufferManager.initialize(2000);
            CNetworkService service = new CNetworkService();

            CConnector connector = new CConnector(service);

            connector.connected_callback += on_connected_gameserver;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            connector.connect(endpoint);

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if(line == "q")
                {
                    break;
                }

                CPacket msg = CPacket.create((short)PROTOCOL.CHAT_MSG_REQ);
                msg.push(line);
                game_servers[0].send(msg);
            }

            ((CRemoteServerPeer)game_servers[0]).token.disconnect();

            Console.ReadKey();
        }

        static void on_connected_gameserver(CUserToken server_token)
        {
            lock(game_servers)
            {
                IPeer server = new CRemoteServerPeer(server_token);
                game_servers.Add(server);
                Console.WriteLine("Connected!");
            }
        }
    }
}
