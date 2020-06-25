namespace GameServer
{
    using FreeNet;
    using System.Collections.Generic;

    class CPlayer // 게임을 플레이하는 플레이어에 관련된 내용을 담은 클래스 플레이어 인덱스와 스킬의 쿨타임 및 채력
    {
        CGameUser owner;
        public byte player_index { get; private set; }
        public float HP;
        public bool isStuned;
        public JOB job;
        public TEAM team;
        public int kill, death;

        public CPlayer(CGameUser user, byte player_index)
        {
            this.owner = user;
            this.player_index = player_index;

        }

        public void reset()
        {
            job = JOB.BEGIN;
            team = TEAM.BEGIN;
            isStuned = false;
            HP = 150;
            kill = 0;
            death = 0;
        }

        public void send(CPacket msg)
        {
            this.owner.send(msg);
            CPacket.destroy(msg);
        }

        public void send_for_broadcast(CPacket msg)
        {
            this.owner.send(msg);
        }
    }
}