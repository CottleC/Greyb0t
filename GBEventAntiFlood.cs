using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    class GBEventAntiFlood : GBEvent
    {
        public static Queue<DateTime> monitor;//  whenever a message is moved, we need to log it so we don't flood
        public static int sendLimit; //number of messages we can send over 30 seconds
        public static bool antiFloodConsoleMsg = true;//don't want to spam console with antiflood messages, so just show 1 per time it happens
        //how often to move messages around
        public GBEventAntiFlood(int frequency)
        {
            Console.WriteLine("Kicked off AntiFlood monitor...");
            triggerSeconds = frequency;
            monitor = new Queue<DateTime>();
            sendLimit = 99;
        }

        public override void RecurringThing()
        {
            //Console.WriteLine("AntiFlood");
            //Console.WriteLine("|");
            //Console.WriteLine(MessageProcessor.readyAllSpeaks.Count());
            //Console.WriteLine(MessageProcessor.pendingAllspeaks.Count());
            while ((MessageProcessor.pendingAllspeaks.Count() > 0) || (MessageProcessor.pendingWhispers.Count() > 0))
            {
                //if we haven't hit the sendlimit
                if (monitor.Count() < sendLimit)
                {
                    if (MessageProcessor.pendingAllspeaks.Count() > 0)
                    {
                        MessageProcessor.readyAllSpeaks.Enqueue(MessageProcessor.pendingAllspeaks.Dequeue());
                        monitor.Enqueue(DateTime.Now);
                        antiFloodConsoleMsg = true;
                    }
                    else if (MessageProcessor.pendingWhispers.Count() > 0)
                    {
                        MessageProcessor.readyWhispers.Enqueue(MessageProcessor.pendingWhispers.Dequeue());
                        monitor.Enqueue(DateTime.Now);
                        antiFloodConsoleMsg = true;
                    }
                }
                else
                {
                    PurgeMonitor();//make room in the monitor if a message becomes stale
                }
            }
        }

        public void PurgeMonitor()
        {
            //if this timestamp is more than 30 seconds old, purge it
            if (monitor.Count() > 0)
            {
                DateTime temp = monitor.Peek();
                TimeSpan diff = DateTime.Now - temp;
                //stale case
                //Console.WriteLine(diff.TotalSeconds);
                if (diff.TotalSeconds > 30)
                {
                    monitor.Dequeue();
                }
                //flood case
                else
                {
                    if (antiFloodConsoleMsg == true)
                    {
                        Console.WriteLine("AntiFlood triggered!");
                        antiFloodConsoleMsg = false;
                    }      
                }
            }
        }

        public override void CleanUp()
        {
            Console.WriteLine("AntiFlood Event is going out of scope...");
        }
    }
}