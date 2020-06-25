using System;
using FreeNet;
using NoTitleGameServer.Database;

namespace NoTitleGameServer
{
    public class Player : IPeer
    {
        CUserToken token;

        public Player(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void on_message(Const<byte[]> buffer)
        {
            byte[] clone = new byte[1024];
            Array.Copy(buffer.Value, clone, buffer.Value.Length);
            CPacket msg = new CPacket(clone, this);
            Program.gameServer.enqueue_packet(msg, this);
        }

        public void on_removed()
        {
            Program.remove_user(this);
        }

        public void send(CPacket msg)
        {
            this.token.send(msg);
        }

        public void disconnect()
        {
            this.token.socket.Disconnect(false);
        }

        public void process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("protocol :" + protocol.ToString());
            switch(protocol)
            {
                case PROTOCOL.REQ_ROOMLIST:
                    {
                        var RoomList = Utility.Utility.GameRoomManager.GetGameRooms();

                        var packet = Utility.Utility.DictionaryToRoomList(RoomList);

                        send(packet);
                    }
                    break;

                case PROTOCOL.REQ_MAKINGROOM:
                    {

                    }
                    break;

                case PROTOCOL.REQ_ENTERROOM:
                    {

                    }
                    break;

                case PROTOCOL.REQ_STARTGAME:
                    {

                    }
                    break;
            }
        }
    }
}
