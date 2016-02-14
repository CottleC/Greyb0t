using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComSay : Command
    {

        public ComSay()
        {
            theMsg = null;
            invocation = new Dictionary<String,String>(); ;
            //define the Dance commands
            invocation.Add("!say", "Says a thing");
        }

        public override void ParseCommand(Message m)
        {
            if(m.isAWhisper)//you can only whisper this command
            {
                if (CommandProcessor.megaUsers.Contains(m.username.ToLower()) )//only mega users can use it
                {
                    foreach (KeyValuePair<String, String> kvp in invocation)
                    {
                        if (m.tell.StartsWith(kvp.Key))
                        {
                            String returnMsg = "";
                            m.tell = m.tell.Replace("!say","");
                            m.tell = m.tell.TrimStart();
                            returnMsg += m.tell;
                            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        }
                    }
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !say " + s);
        }

        public override void CleanUp()
        {
            //undefined for !lurk
        }
    }
}

