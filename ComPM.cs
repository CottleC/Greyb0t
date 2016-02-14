using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComPM : Command
    {

        public ComPM()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            //define the Roll commands
            invocation.Add("!pmme", "PM's back a response to the requester");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                Console.WriteLine(m.tell);
                if(m.tell.StartsWith(kvp.Key) && (m.isAWhisper==false))
                {
                    Console.WriteLine(m.username + " requested a pm");
                    String returnMsg = "NULL msg from ComPM";

                    returnMsg = "pssst! Hi! you requested a whisper!";
                    KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                    MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                }
                else if (m.tell.StartsWith(kvp.Key) && (m.isAWhisper == true))
                {
                    Console.WriteLine(m.username + " requested a pm");
                    String returnMsg = "NULL msg from ComPM";

                    returnMsg = "pssst! Hi!, You whispered me!";
                    KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                    MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                    ChannelInfo.PrintUsers();
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !pmme " + s);
        }

        public override void CleanUp()
        {
            //undefined for !roll
        }
    }
}

