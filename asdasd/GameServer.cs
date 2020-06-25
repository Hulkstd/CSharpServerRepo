using System;
using System.Collections.Generic;
using System.Text;
using FreeNet;
using System.Threading;

namespace NoTitleGameServer
{
    public class GameServer
    {
        object operation_lock;
        Queue<CPacket> user_operations;

        Thread logic_thread;
        AutoResetEvent loop_event;

        public GameServer()
        {
            this.operation_lock = new object();
            this.loop_event = new AutoResetEvent(false);
            this.user_operations = new Queue<CPacket>();

            this.logic_thread = new Thread(gameloop);
            this.logic_thread.Start();
        }

        private void gameloop()
        {
            while(true)
            {
                CPacket packet = null;
                lock(this.operation_lock)
                {
                    if(this.user_operations.Count > 0)
                    {
                        packet = this.user_operations.Dequeue();
                    }
                }

                if(packet != null)
                {
                    Thread thread = new Thread((t) => 
                    {
                        process_receive(packet);
                        while(((Thread)t).ThreadState != ThreadState.Stopped)
                        {
                            ((Thread)t).Join();
                        }
                    });
                    thread.Start(thread);
                }

                if(this.user_operations.Count <= 0)
                {
                    this.loop_event.WaitOne();
                }
            }
        }

        public void enqueue_packet(CPacket msg, Player peer)
        {
            lock(this.operation_lock)
            {
                this.user_operations.Enqueue(msg);
                this.loop_event.Set();
            }
        }

        void process_receive(CPacket msg)
        {
            //todo:
            // user msg filter 체크.

            msg.owner.process_user_operation(msg);
        }

        public void user_disconnected(Player player)
        {

        }
    }
}
