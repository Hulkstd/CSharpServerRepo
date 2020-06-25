namespace GameServer
{

    public enum PROTOCOL : short
    {
        BEGIN = 0,

        // 로딩을 시작해라.
        START_LOADING = 1,

        LOADING_COMPLETED = 2,

        // 게임 시작
        GAME_START = 3,

        // 클라이언트의 이동 요청.
        MOVING_REQ = 4,

        // 플레이아가 이동 했음을 알린다.
        PLAYER_MOVED = 5,

        // 플레이어가 직업 1번 스킬을 사용함
        USE_SKILLONE = 6,

        // 플레이어가 직업 2번 스킬을 사용함
        USE_SKILLTWO = 7,

        // 플레이어가 직업 3번 스킬을 사용함
        USE_SKILLTHREE = 8,

        // 데미지를 받음
        DAMAGED = 9,

        // 플레이어 사망
        PLAYER_DEAD = 10,

        // 점프
        JUMP = 11,

        // 게임방 리스트 보냄.
        SERVER_LIST_SEND = 295,

        // 게임방 리스트 요청.
        SERVER_LIST_REQ = 296,

        // 게임방 생성 요청,
        CREATE_GAME_ROOM_REQ = 297,

        // 게임방 입장 요청.
        ENTER_GAME_ROOM_REQ = 298,

        // 게임 종료.
        GAME_OVER = 299,

        ROOM_REMOVED = 300,

        END
    }

    public enum JOB : short
    {
        BEGIN = 0,

        FIRE = 1,

        ICE = 2,

        END
    }

    public enum TEAM : short
    {
        BEGIN = 0,

        RED = 1,

        BLUE = 2,

        SP = 3,

        END
    }
}