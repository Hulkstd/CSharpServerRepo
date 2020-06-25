using FreeNet;

namespace GameServer
{
    using FreeNet;
    using System;
    using System.Collections.Generic;

    class CGameRoom
    {
        enum PLAYER_STATE : byte
        {
            ENTERED_ROOM,

            LOADING_COMPLETE,

            DEAD,

            ALIVE
        }
        List<CPlayer> players;

        Dictionary<byte, PLAYER_STATE> player_state;
        Dictionary<byte, Vector3> player_position;
        Dictionary<byte, Quaternion> player_rotation;

        public CGameRoom()
        {
            this.players = new List<CPlayer>();
            this.player_state = new Dictionary<byte, PLAYER_STATE>();
            this.player_position = new Dictionary<byte, Vector3>();
            this.player_rotation = new Dictionary<byte, Quaternion>();
        }

        void broadcast(CPacket msg)
        {
            this.players.ForEach(player => player.send_for_broadcast(msg));
            CPacket.destroy(msg);
        }


        void change_playerstate(CPlayer player, PLAYER_STATE state)
        {
            if (this.player_state.ContainsKey(player.player_index))
            {
                this.player_state[player.player_index] = state;
            }
            else
            {
                this.player_state.Add(player.player_index, state);
            }
        }

        bool allplayers_ready(PLAYER_STATE state)
        {
            foreach (KeyValuePair<byte, PLAYER_STATE> kvp in this.player_state)
            {
                if (kvp.Value != state)
                {
                    return false;
                }
            }

            return true;
        }

        public void enter_gameroom(List<CGameUser> users)
        {
            this.players.Clear();
            this.player_state.Clear();
            byte i = 0;
            foreach (CGameUser user in users)
            {
                CPlayer player = new CPlayer(user, i++);
                this.players.Add(player);

                change_playerstate(player, PLAYER_STATE.ENTERED_ROOM);

                CPacket msg = CPacket.create((Int16)(PROTOCOL.START_LOADING));
                msg.push(player.player_index);
                player.HP = 100;
                msg.push_single(player.HP);
                msg.push_single(i);
                msg.push_single(i);
                msg.push_single(i);
                player.send(msg);

                user.enter_room(player, this);
            }
        }

        public void moving_req(CPlayer player, Vector3 vector3, Quaternion quaternion, byte[] anim, byte player_index)
        {
            if (player_position.ContainsKey(player_index))
            {
                player_position[player_index] = vector3;
            }
            else
            {
                player_position.Add(player_index, vector3);
            }

            if(player_rotation.ContainsKey(player_index))
            {
                player_rotation[player_index] = quaternion;
            }
            else
            {
                player_rotation.Add(player_index, quaternion);
            }

            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            msg.push(player_index);
            msg.push_single(player.HP);

            msg.push_single(vector3.x);
            msg.push_single(vector3.y);
            msg.push_single(vector3.z);

            msg.push_single(quaternion.x);
            msg.push_single(quaternion.y);
            msg.push_single(quaternion.z);

            msg.push(anim[0]); // idle
            msg.push(anim[1]); // walk
            msg.push(anim[2]); // run
            msg.push(anim[3]); // die

            players.ForEach(player2 =>
            {
                if (player2.player_index != player_index)
                    player2.send(msg);
            });
        }

        public void useskill(byte player_index, PROTOCOL skill)
        {
            CPacket msg = CPacket.create((short)skill);
            msg.push(player_index);

            players.ForEach(player =>
            {
                if (player_index != player.player_index)
                    player.send(msg);
            });
        }

        public void deadPlayer(byte player_index)
        {
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_DEAD);
            msg.push(player_index);

            players.ForEach(player =>
            {
                if (player.player_index != player_index)
                    player.send(msg);
                else
                {
                    player.death++;
                }
            });
        }

        public void damaged(byte player_index, float HP, CGameUser p)
        {
            players.ForEach(player =>
            {
                if (player.player_index == player_index)
                    player.HP = HP;
            });
            if(HP <= 0)
            {
                p.player.kill++;
            }
        }

        public void loading_complete(CPlayer player, byte Job)
        {
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);
            player.job = (JOB)Job;
            if(!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                return;
            }

            battle_start();
        }

        void battle_start()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.GAME_START);

            int red, blue;

            red = blue = players.Count / 2;

            msg.push((byte)players.Count);
            
            this.players.ForEach(player =>
            {
                int r = new Random().Next() % 2;

                if (r == 0) red--;
                else blue--;
                
                msg.push(player.player_index);

                msg.push((byte)player.job);
                msg.push(((byte)(r + 1)));
            });

            broadcast(msg);
        }

        public void destroy()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
            broadcast(msg);

            this.players.Clear();
        }
    }

}