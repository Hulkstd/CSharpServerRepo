using System;
using FreeNet;

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

        void IPeer.on_message(Const<byte[]> buffer)
        {
            byte[] clone = new byte[buffer.Value.Length];
            Array.Copy(buffer.Value, clone, buffer.Value.Length);
            CPacket msg = new CPacket(clone, this);
            Program.gameServer.enqueue_packet(msg, this);
        }

        void IPeer.on_removed()
        {
            Console.WriteLine("The client disconnected.");

            Program.remove_user(this);
        }

        public void send(CPacket msg)
        {
            this.token.send(msg);   
        }

        void IPeer.disconnect()
        {
            this.token.socket.Disconnect(false);
        }

        void IPeer.process_user_operation(CPacket msg)
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

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_MAKINGROOM:
                    {
                        CPacket packet = CPacket.create((short)PROTOCOL.DEC_MAKINGROOM);

                        string ID = msg.pop_string();
                        string title = msg.pop_string();
                        string IP = msg.pop_string();

                        if(Utility.Utility.GameRoomManager.MakingRoom(ID, title, IP))
                        {
                            packet.push(1);
                        }
                        else
                        {
                            packet.push(0);
                        }

                        send(packet);

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_ENTERROOM:
                    {
                        CPacket packet = CPacket.create((short)PROTOCOL.DEC_ENTERROOM);

                        string MasterID = msg.pop_string();
                        string ID = msg.pop_string();
                        string IP = msg.pop_string();
                        var Room = Utility.Utility.GameRoomManager.FindRoom(MasterID);

                        if (Room != null)
                        {
                            packet.push(1);
                            Room.JoinRoom(NoTitleGameServer.Database.UserManager.FindUser(ID), IP);
                        }
                        else
                        {
                            packet.push(0);
                        }

                        send(packet);

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_STARTGAME:
                    {
                        CPacket packet = CPacket.create((short)PROTOCOL.DEC_STARTGAME);

                        string ID = msg.pop_string();
                        var room = Utility.Utility.GameRoomManager.FindRoom(ID);

                        if (room != null && room.CanStartGame())
                        {
                            packet.push(1);
                            room.BroadCast(packet);
                        }
                        else
                        {
                            packet.push(0);
                            send(packet);
                        }

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_REGISTER:
                    {
                        var ID = msg.pop_string();
                        var PW = msg.pop_string();
                        var Name = msg.pop_string();

                        var Error = new RegisterFailure();
                        var flag = Database.UserManager.AddUser(ID, PW, Name, ref Error);

                        CPacket packet = CPacket.create((short)PROTOCOL.DEC_REGISTER);
                        packet.push((byte)(flag ? 1 : 0));
                        packet.push_int16((short)Error);

                        send(packet);

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_LOGIN:
                    {
                        var ID = msg.pop_string();
                        var PW = msg.pop_string();

                        var Error = new LoginFailure();
                        var user = Database.UserManager.FindUser(ID);

                        CPacket packet = CPacket.create((short)PROTOCOL.DEC_LOGIN);

                        if(user.PW == PW)
                        {
                            packet.push(1);

                            user.token = token;
                        }
                        else
                        {
                            packet.push(0);
                            Error = Error | LoginFailure.PW_INCORRECT;
                            if(user.ID == null)
                            {
                                Error = Error | LoginFailure.ID_INCORRECT;
                            }
                            packet.push_int16((short)Error);
                        }

                        send(packet);

                        CPacket.destroy(packet);
                    }
                    break;

                case PROTOCOL.REQ_NICKNAME:
                    {
                        CPacket packet = CPacket.create((short)PROTOCOL.SEND_NICKNAME);
                        packet.push(Database.UserManager.FindUser(msg.pop_string()).Name);

                        send(packet);

                        CPacket.destroy(packet);
                    }
                    break;
            }
            CPacket.destroy(msg);
        }
    }
}
