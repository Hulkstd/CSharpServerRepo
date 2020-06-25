  using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FreeNet;
using MySql.Data.MySqlClient;

namespace NoTitleGameServer.Utility
{
    public static class Utility
    {
        public static MySqlConnection connection;
        public static GameRoomManager GameRoomManager;

        public static Dictionary<string, GameRoom> RoomListToDictionary(CPacket packet)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                var length = packet.pop_int32();
                byte[] bytes = new byte[length];

                for(int i = 0; i < length; i++)
                {
                    bytes[i] = packet.pop_byte();
                }

                stream.Write(bytes, 0, length);
                stream.Seek(0, SeekOrigin.Begin);

                var result = formatter.Deserialize(stream);
                
                return (Dictionary<string, GameRoom>)result;
            }
        }

        public static CPacket DictionaryToRoomList(Dictionary<string, GameRoom> roomList)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, roomList);

                var bytes = stream.ToArray();
                CPacket packet = CPacket.create((short)PROTOCOL.SEND_ROOMLIST);
                packet.push_int32(bytes.Length);

                foreach(byte b in bytes)
                {
                    packet.push(b);
                }

                return packet;
            }
        }

        private static bool com(byte[] a, byte[] b)
        {
            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}
