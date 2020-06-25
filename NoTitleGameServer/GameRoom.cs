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
        private string   MasterInfo    { get;         set; }
        private string[] CntPlayerIP   { get;         set; }
        private string[] CntPlayerInfo { get;         set; }

        public GameRoom(string ID, string RoomTitle, string IP)
        {
            this.RoomTitle = RoomTitle;
            this.MasterInfo = ID;
            this.MasterIP = IP;
            this.CntPlayer = 1;
            this.CntPlayerIP = new string[3];
            this.CntPlayerInfo = new string[3];

            FreeNet.CPacket packet = FreeNet.CPacket.create((short)PROTOCOL.SEND_ENTERROOM);
            packet.push(UserManager.FindUser(MasterInfo).Name);
            packet.push(1);
            UserManager.FindUser(MasterInfo).token.send(packet);
        }

        public bool CanStartGame() => CntPlayer == 4;

        public bool JoinRoom(User user, string IP)
        {
            if (CntPlayer == MAX_PLAYER) return false;

            FreeNet.CPacket packet = FreeNet.CPacket.create((short)PROTOCOL.SEND_ENTERROOM);
            packet.push(UserManager.FindUser(MasterInfo).Name);
            packet.push(1);
            user.token.send(packet);

            for (int i = 0; i < CntPlayer - 1; i++)
            {
                packet = FreeNet.CPacket.create((short)PROTOCOL.SEND_ENTERROOM);
                packet.push(UserManager.FindUser(CntPlayerInfo[i]).Name);
                packet.push(0);
                user.token.send(packet);
            }

            packet = FreeNet.CPacket.create((short)PROTOCOL.SEND_ENTERROOM);
            packet.push(user.Name);
            packet.push(0);

            user.token.send(packet);
            BroadCast(packet);

            CntPlayerInfo[CntPlayer - 1] = user.ID;
            CntPlayerIP[CntPlayer - 1] = IP;

            CntPlayer++;

            FreeNet.CPacket.destroy(packet);

            return true;
        }

        public void BroadCast(FreeNet.CPacket packet)
        {
            UserManager.FindUser(MasterInfo).token.send(packet);
            foreach(string id in CntPlayerInfo)
            {
                if(id != null) UserManager.FindUser(id).token.send(packet);
            }
        }
    }

    public partial class GameRoom
    {
        public readonly static int MAX_PLAYER = 4;
    }
}
