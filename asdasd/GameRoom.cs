using System;
using System.Collections.Generic;
using System.Text;
using NoTitleGameServer.Database;

namespace NoTitleGameServer
{
    [Serializable]
    public partial class GameRoom
    {
        public  string   RoomTitle     { get; private set; }
        public  int      CntPlayer     { get; private set; }
        public  string   MasterIP      { get; private set; }
        private User     MasterInfo    { get;         set; }
        private string[] CntPlayerIP   { get;         set; }
        private User[]   CntPlayerInfo { get;         set; }

        public GameRoom(string ID, string RoomTitle, string IP)
        {
            this.RoomTitle = RoomTitle;
            this.MasterInfo = User.FindUser(ID);
            this.MasterIP = IP;
        }
    }

    public partial class GameRoom
    {
        public readonly static int MAX_PLAYER = 4;
    }
}
