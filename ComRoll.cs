using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComRoll : Command
    {
        public int maxVal;

        public ComRoll()
        {
            maxVal = 100;
            theMsg = null;
            invocation = new Dictionary<String,String>(); ;
            //define the Roll commands
            invocation.Add("!rollthedice", "Rolls a very large die");
        }

        public ComRoll(int range)
        {
            maxVal = range;
            theMsg = null;
            invocation = new Dictionary<String, String>();
            //define the Roll commands
            invocation.Add("!rollthedice", "Rolls a very large die");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                Console.WriteLine(m.tell);
                if(m.tell.StartsWith(kvp.Key))
                {
                    Console.WriteLine(m.username + " rolled the dice.");
                    String returnMsg = "NULL msg from ComRoll";
                    //do some rolly logic
                    Random r = new Random();
                    int val = r.Next(1, maxVal);
                    returnMsg = "/me says:" + m.username + " rolled a " + maxVal + " sided die and got: " + val + "!";
                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !rollthedice " + s);
        }

        public override void CleanUp()
        {
            //undefined for !roll
        }
    }
}

