using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComDance : Command
    {

        public ComDance()
        {
            theMsg = null;
            invocation = new Dictionary<String,String>(); ;
            //define the Dance commands
            invocation.Add("!dance", "Does a little jig");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                if(m.tell.StartsWith(kvp.Key))
                {
                    String returnMsg = "NULL msg from ComRoll";
                    //do some dancy logic
                    returnMsg = "/me dances: :D-<";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    returnMsg = "/me dances: :D|-<";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    returnMsg = "/me dances: :D/-<";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !dance " + s);
        }

        public override void CleanUp()
        {
            //undefined for !dance
        }
    }
}

