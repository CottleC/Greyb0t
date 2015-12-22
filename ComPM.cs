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
                if(m.tell.StartsWith(kvp.Key))
                {
                    Console.WriteLine(m.username + " requested a pm");
                    String returnMsg = "NULL msg from ComPM";
                    //do some rolly logic
                    returnMsg = "pssst! Hi!";
                    KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                    MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !roll " + s);
        }

        public override void CleanUp()
        {
            //undefined for !roll
        }
    }
}

