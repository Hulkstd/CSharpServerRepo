using System.Collections.Generic;

namespace GameServer
{
    class CGameRoomManager
    {
        public List<CGameRoom> rooms;

        public CGameRoomManager()
        {
            this.rooms = new List<CGameRoom>();
        }


        public void create_room(List<CGameUser> Players)
        {
            CGameRoom battleroom = new CGameRoom();
            battleroom.enter_gameroom(Players);

            this.rooms.Add(battleroom);
        }

        public void remove_room(CGameRoom room)
        {
            room.destroy();
            this.rooms.Remove(room);
        }
    }
}