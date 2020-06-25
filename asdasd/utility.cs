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
            string code = packet.pop_string();

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                var decompressed = System.Text.Encoding.Default.GetBytes(code);

                stream.Write(decompressed, 0, decompressed.Length);
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

                var result = BitConverter.ToString(stream.ToArray());
                CPacket packet = CPacket.create((short)PROTOCOL.SEND_ROOMLIST);
                packet.push(result);

                return packet;
            }
        }
    }
}
