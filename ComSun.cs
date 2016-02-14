using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComSun : Command
    {

        public ComSun()
        {
            theMsg = null;
            invocation = new Dictionary<String,String>(); ;
            //define the Dance commands
            invocation.Add("!sun", "notices sun");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                if(m.tell.StartsWith(kvp.Key))
                {
                    String returnMsg = "NULL msg from ComSun";
                    //notices sun
                    returnMsg = "/me notices sun 0_0";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    return;
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !sun " + s);
        }

        public override void CleanUp()
        {
            //undefined for !dance
        }
    }
}

