using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComTime : Command
    {

        public ComTime()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            //define the Roll commands
            invocation.Add("!time", "Returns the local time");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                //Console.WriteLine(m.tell);
                if(m.tell.Equals(kvp.Key))
                {
                    Console.WriteLine(m.username + " requested the time.");
                    String returnMsg = "NULL msg from ComTime";
                    returnMsg = "/me says: " + "It is :" + DateTime.Now + " at Tuesday's house!";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !time " + s);
        }

        public override void CleanUp()
        {
            //undefined for !roll
        }
    }
}

