using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeNet;

namespace Test
{
    public class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }
        WeakReference freenet_eventmanager;

        public delegate void CPacketParam(CPacket msg);
        public CPacketParam onmessage;

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void set_eventmanager(CFreeNetEventManager event_manager)
        {
            this.freenet_eventmanager = new WeakReference(event_manager);
        }

        /// <summary>
        /// 메시지를 수신했을 때 호출된다.
        /// 파라미터로 넘어온 버퍼는 워커 스레드에서 재사용 되므로 복사한 뒤 어플리케이션으로 넘겨준다.
        /// </summary>
        /// <param name="buffer"></param>
        void IPeer.on_message(Const<byte[]> buffer)
        {
            // 버퍼를 복사한 뒤 CPacket클래스로 감싼 뒤 넘겨준다.
            // CPacket클래스 내부에서는 참조로만 들고 있는다.
            onmessage(new CPacket(buffer.Value, this));
        }

        void IPeer.on_removed()
        {
            (this.freenet_eventmanager.Target as CFreeNetEventManager).enqueue_network_event(NETWORK_EVENT.disconnected);
        }

        void IPeer.send(CPacket msg)
        {
            this.token.send(msg);
        }

        void IPeer.disconnect()
        {
        }

        void IPeer.process_user_operation(CPacket msg)
        {
        }
    }
}