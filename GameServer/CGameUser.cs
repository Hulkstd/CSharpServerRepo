using FreeNet;
using System;

namespace GameServer
{
    class CGameUser : IPeer
    {
        CUserToken token;

        public CGameRoom battle_room { get; private set; }

        public CPlayer player { get; private set; }

        public CGameUser(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        void IPeer.on_message(Const<byte[]> buffer)
        {
            byte[] clone = new byte[1024];
            Array.Copy(buffer.Value, clone, buffer.Value.Length);
            CPacket msg = new CPacket(clone, this);
            Program.game_main.enqueue_packet(msg, this);
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
            Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL.SERVER_LIST_REQ:

                    CPacket new_msg = CPacket.create((short)PROTOCOL.SERVER_LIST_SEND);
                    new_msg.push_int32(Program.game_main.room_manager.rooms.Count);

                    send(new_msg);
                    break;

                case PROTOCOL.ENTER_GAME_ROOM_REQ:

                    Program.game_main.matching_req(this);

                    break;

                case PROTOCOL.LOADING_COMPLETED:
                    byte Job = msg.pop_byte();
                    this.battle_room.loading_complete(player, Job);
                    break;

                case PROTOCOL.MOVING_REQ:
                    {
                        byte player_index = msg.pop_byte();

                        float px = msg.pop_Single();
                        float py = msg.pop_Single();
                        float pz = msg.pop_Single();

                        float rx = msg.pop_Single();
                        float ry = msg.pop_Single();
                        float rz = msg.pop_Single();

                        byte idle = msg.pop_byte();
                        byte walk = msg.pop_byte();
                        byte run = msg.pop_byte();
                        byte die = msg.pop_byte();

                        this.battle_room.moving_req(this.player, new Vector3(px, py, pz), new Quaternion(rx, ry, rz), new byte[4] { idle, walk, run, die }, player_index);
                    }
                    break;

                case PROTOCOL.DAMAGED:
                    {
                        byte player_index;
                        float HP;

                        player_index = msg.pop_byte();
                        HP = msg.pop_Single();

                        this.battle_room.damaged(player_index, HP, this);
                    }
                    break;
                case PROTOCOL.USE_SKILLONE:
                    {
                        byte player_index = msg.pop_byte();
                        this.battle_room.useskill(player_index, PROTOCOL.USE_SKILLONE);
                    }
                    break;
                case PROTOCOL.USE_SKILLTWO:
                    {
                        byte player_index = msg.pop_byte();
                        this.battle_room.useskill(player_index, PROTOCOL.USE_SKILLTWO);
                    }
                    break;

                case PROTOCOL.USE_SKILLTHREE:
                    {
                        byte player_index = msg.pop_byte();
                        this.battle_room.useskill(player_index, PROTOCOL.USE_SKILLTHREE);
                    }
                    break;

                case PROTOCOL.PLAYER_DEAD:
                    {
                        byte player_index = msg.pop_byte();
                        this.battle_room.deadPlayer(player_index);
                    }
                    break;
            }
        }

        public void enter_room(CPlayer player, CGameRoom room)
        {
            this.player = player;
            this.battle_room = room;
        }
    }
}