using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

//the purpose of this class is to monitor greybot's folder for fresh giveaway files
//if one is found it is automatically enrolled as the current giveaway
namespace GreyB0t
{
    class GBEventVoteSentinel : GBEvent
    {

        public GBEventVoteSentinel(int i)
        {
            triggerSeconds = i;
            runTime = -1;
            breaker = false;//flag to stop the thread
        }

        public override void RecurringThing()
        {
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                if (temp.EndsWith(".json"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("vote_"))
                    {
                        ComVote c;
                        c = JsonConvert.DeserializeObject<ComVote>(File.ReadAllText(temp));
                        c = new ComVote(c);
                        Console.WriteLine(":\tGreyb0t: Deserialized vote file @: " + temp);
                        //update this some other time
                        CommandProcessor.InsertCommand(c);
                        temp.Replace("//", "/");
                        File.Delete(temp);
                    }
                }
            }
        }

        public override void CleanUp()
        {
            Console.WriteLine("GiveAwaySentinel Event is going out of scope...");
        }
    }
}

