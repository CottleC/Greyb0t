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
    class GBEventHuntFile : GBEvent
    {
        public static bool loaded = false;
        public GBEventHuntFile(int i)
        {
            triggerSeconds = i;
            runTime = -1;
            breaker = false;//flag to stop the thread
            RecurringThing();//do this once inititally to load in the huntfile
        }

        public override void RecurringThing()
        {
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                if (temp.EndsWith(".json"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("huntfile"))
                    {
                        if (!loaded)
                        {
                            //read in
                            ComHunt c = new ComHunt();
                            c = JsonConvert.DeserializeObject<ComHunt>(File.ReadAllText("huntfile.json"));
                            c = new ComHunt(c);
                            CommandProcessor.InsertCommand(c);
                            loaded = true;
                        }
                        else
                        {
                            //write out
                        }
                        
                    }
                }
            }
        }

        public override void CleanUp()
        {
            Console.WriteLine("FreshenMods Event is going out of scope...");
        }
    }
}

