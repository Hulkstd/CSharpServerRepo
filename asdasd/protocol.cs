using System;
using System.Collections.Generic;
using System.Text;

namespace NoTitleGameServer
{
    public enum PROTOCOL : short // REQ : Client -> Server , SEND, DEC : Server -> Client
    {
        BEGIN = 0,
        
        //서버에서 만들어진 방 리스트 요청. (필요한 정보 : )
        REQ_ROOMLIST = 1,

        //서버에서 만들어진 방 리스트. (들어가는 정보 : 이진화 정보{string})  
        //( Utility클래스중 RoomListToDictionary를 이용하여 Dictionary<int, string>으로 변환.)
        SEND_ROOMLIST = 2,

        //서버에 방 만들기 요청 (필요한 정보 : 플레이어ID{string}, 방이름{string}, 시용자 ip{string})
        REQ_MAKINGROOM = 3,

        //방 만들기 성공 여부 (들어가는 정보 : 성공여부{bool})
        DEC_MAKINGROOM = 4,

        //방 접속 요청 (필요한 정보 : 들어가는방 방장ID{string}, 플레이어ID{string}, 사용자 ip{string})
        REQ_ENTERROOM = 5,

        //방 접속 요청 성공 여부 (들어가는 정보 : 성공여부{bool})
        DEC_ENTERROOM = 6,

        //게임 시작 요청 (필요한 정보 : 방장 플레이어ID{string})
        REQ_STARTGAME = 7,

        //게임 사작 가능 여부 (들어가는 정보 : 성공 여부{bool})

        DEC_STARTGAME = 8,
    }
}
