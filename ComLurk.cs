using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComLurk : Command
    {
        public static String[] lurkMethods = {" thinks keyboards are for newbs and begins to lurk, hard.",
                                              " has icing on their fingers and can no longer type!",
                                              " has activated lurk mode.",
                                              " just disapperated into the lurker's realm.",
                                              " curls up and settles in for a nice relaxing lurk."
                                             };
        public ComLurk()
        {
            theMsg = null;
            invocation = new Dictionary<String,String>(); ;
            //define the Dance commands
            invocation.Add("!lurk", "Does a little jig");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                if(m.tell.StartsWith(kvp.Key))
                {
                    String returnMsg = "NULL msg from ComRoll";
                    //do some fancy setnence stuff
                    Random r = new Random();
                    int strVal = r.Next(0, lurkMethods.Count());
                    returnMsg = m.username+" "+lurkMethods[strVal];
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !lurk " + s);
        }

        public override void CleanUp()
        {
            //undefined for !lurk
        }
    }
}

