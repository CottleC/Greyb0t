using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace GreyB0t
{
    class GBEventPeriodicMessages : GBEvent
    {
        //events DO stuff on time intervals
        //Typically there is a 
        //kickoff message: vote on [stuff] has started!
        //recurring message: vote has x seconds left!
        //end message: vote has finished! the winnder was god!

        public static List<PeriodicMessage> pmList = new List<PeriodicMessage>();

        public GBEventPeriodicMessages(int i)
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
                    if (filename.StartsWith("periodicMsg"))
                    {
                        PeriodicMessage p = new PeriodicMessage();
                        p = JsonConvert.DeserializeObject<PeriodicMessage>(File.ReadAllText(temp));
                        p = new PeriodicMessage(p);
                        Console.WriteLine(":\tGreyb0t: Deserialized PeriodicMsg file @: " + temp);
                        //update this some other time
                        bool hasThisMsg = false;
                        foreach(PeriodicMessage lP in pmList)
                        {
                            if(p.tell.Equals(lP.tell))
                            {
                                hasThisMsg = true;
                                break;
                            }
                        }
                        if (!hasThisMsg)
                        {
                            pmList.Add(p);
                            p.Start();
                        }
                        temp.Replace("//", "/");
                    }
                }
            }
            //!timeout entrophist 1Console.WriteLine("The Periodic Message list is of size: "+pmList.Count());
        }

        public override void CleanUp()
        {
            Console.WriteLine("GBEventPeriodicMessages Event is going out of scope...");
        }
    }
}

