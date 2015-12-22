using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GreyB0t
{
    /* MessageProcessor keeps all pending messages in their respective queues (whisper/allspeak queues)
     * An event which manages flooding moves messages from the pending queues to the active queues
     * Only so many messages can be moved to the active queue in a given period of time (flooding)
     * Any message in the ReadyQueue is immediately snatched up by the bot monitoring the ready queue
     * And spoken in the monitored channel
     */
    static class MessageProcessor
    {
        public static Queue<String> pendingAllspeaks = new Queue<String>();
        public static Queue<String> readyAllSpeaks = new Queue<String>();
        //whispers have a "message","targetUser"
        public static Queue<KeyValuePair<String, String>> pendingWhispers = new Queue<KeyValuePair<String,String>>();
        public static Queue<KeyValuePair<String, String>> readyWhispers = new Queue<KeyValuePair<String, String>>();


        public static void Init()
        {
            //create antiflood event
            EventProcessor.AddGBEvent(new GBEventAntiFlood(1));
        }

        public static void AddAllSpeak(String m)
        {
            pendingAllspeaks.Enqueue(m);
        }

        public static void AddWhisper(KeyValuePair<String,String> item)
        {
            pendingWhispers.Enqueue(item);
        }

        public static void MoveAllSpeakToReadyQ()
        {
            readyAllSpeaks.Enqueue(pendingAllspeaks.Dequeue());
        }

        public static void MoveWhisperToReadyQ()
        {
            readyWhispers.Enqueue(pendingWhispers.Dequeue());
        }
    }
}
