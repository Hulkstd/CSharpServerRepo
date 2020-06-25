using System;
using System.Collections.Generic;
using System.Text;

namespace NoTitleGameServer
{
    public enum PROTOCOL : short // REQ : Client -> Server , SEND, DEC : Server -> Client
    {
        BEGIN = 0,

        ///<summary>
        ///서버에서 만들어진 방 리스트 요청. (필요한 정보 : )
        ///</summary>
        REQ_ROOMLIST = 1,

        ///<summary>
        ///서버에서 만들어진 방 리스트. (들어가는 정보 : 이진화 정보{string})  
        ///( Utility클래스중 RoomListToDictionary를 이용하여 Dictionary<int, string>으로 변환.)
        ///</summary>
        SEND_ROOMLIST = 2,

        ///<summary>
        ///서버에 방 만들기 요청 (필요한 정보 : 플레이어ID{string}, 방이름{string}, 시용자 ip{string})
        ///</summary>
        REQ_MAKINGROOM = 3,

        ///<summary>
        ///방 만들기 성공 여부 (들어가는 정보 : 성공여부{byte})
        ///</summary>
        DEC_MAKINGROOM = 4,

        ///<summary>
        ///방 접속 요청 (필요한 정보 : 들어가는방 방장ID{string}, 플레이어ID{string}, 사용자 ip{string})
        ///</summary>
        REQ_ENTERROOM = 5,

        ///<summary>
        ///방 접속 요청 성공 여부 (들어가는 정보 : 성공여부{byte})
        ///</summary>
        DEC_ENTERROOM = 6,

        ///<summary>
        ///게임 시작 요청 (필요한 정보 : 방장 플레이어ID{string})
        ///</summary>
        REQ_STARTGAME = 7,

        ///<summary>
        ///게임 사작 가능 여부 (들어가는 정보 : 성공 여부{byte})
        ///</summary>
        DEC_STARTGAME = 8,

        ///<summary>
        ///회원가입 요청 (필요한 정보 : ID{string} PW{string} Name{string})
        ///</summary>
        REQ_REGISTER = 9,

        ///<summary>
        ///회원가입 결과 (들어가는 정보 : 성공 여부{byte} 실패 요인{enum RegisterFailure}
        ///</summary>
        DEC_REGISTER = 10,

        ///<summary>
        ///로그인 요청 (필요한 정보 : ID{string} PW{string})
        ///</summary>
        REQ_LOGIN = 11,

        ///<summary>
        ///로그인 결과 (들어가는 정보 : 성공 여부{byte} 실패 요인{enum LoginFailure})
        ///</summary>
        DEC_LOGIN = 12,

        ///<summary>
        ///방 입장 (들어가는 정보 : Name{string} IsMaster{bool})
        ///</summary>
        SEND_ENTERROOM = 13,

        ///<summary>
        ///닉네임 요청 (필요한 정보 : ID{string})
        ///</summary>
        REQ_NICKNAME = 14,

        ///<summary>
        ///닉네임 조회 결과 (들어가는 정보 : Name{string})
        ///</summary>
        SEND_NICKNAME = 15,
    }

    [Flags]
    public enum RegisterFailure : short
    {
        ID_ALREADY_EXISTS = 0x01,
        NICKNAME_ALREADY_EXISTS = 0x02,
    }

    [Flags]
    public enum LoginFailure : short
    {
        ID_INCORRECT = 0x01,
        PW_INCORRECT = 0x02,
    }
}
