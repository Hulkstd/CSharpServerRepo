using System;
using System.Collections.Generic;
using System.Text;

namespace NoTitleGameServer
{
    public class GameRoomManager
    {
        private Dictionary<string, GameRoom> gameRooms;

        public GameRoomManager()
        {
            gameRooms = new Dictionary<string, GameRoom>();
        }

        public bool MakingRoom(string ID, string Title, string IP)
        {
            try
            {
                GameRoom gameRoom = new GameRoom(ID, Title, IP);

                if(gameRooms.ContainsKey(ID))
                {
                    return false;
                }
                gameRooms.Add(ID, gameRoom);
            }
            catch(Exception e)
            {
                return false;
            }

            return true;
        }

        public GameRoom FindRoom(string MasterID)
        {
            try
            {
                if(gameRooms.ContainsKey(MasterID))
                {
                    return gameRooms[MasterID];
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Dictionary<string, GameRoom> GetGameRooms()
        {
            return gameRooms;
        }
    }
}
