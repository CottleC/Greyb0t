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
    class GBEventFreshenMods : GBEvent
    {
        public static DateTime lastWriteTime;
        public GBEventFreshenMods(int i)
        {
            triggerSeconds = i;
            runTime = -1;
            breaker = false;//flag to stop the thread
            RecurringThing();
        }

        public override void RecurringThing()
        {
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                FileInfo f = new FileInfo(temp);
                if (temp.EndsWith(".json"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("users_"))
                    {
                        if (lastWriteTime != null)
                        {
                            
                            if (f.LastWriteTime == lastWriteTime)
                            {

                            }
                            else
                            {
                                Users users;
                                users = JsonConvert.DeserializeObject<Users>(File.ReadAllText(temp));
                                users = new Users(users);
                                Console.WriteLine(":\tGreyb0t: Deserialized mods file @: " + temp);
                                //update this some other time
                                ComModerate c = new ComModerate();
                                CommandProcessor.InsertCommand(c);
                                temp.Replace("//", "/");
                                lastWriteTime = f.LastWriteTime;
                            }
                            
                        }
                        else
                        {
                            Users users;
                            users = JsonConvert.DeserializeObject<Users>(File.ReadAllText(temp));
                            users = new Users(users);
                            Console.WriteLine(":\tGreyb0t: Deserialized mods file @: " + temp);
                            //update this some other time
                            ComModerate c = new ComModerate();
                            CommandProcessor.InsertCommand(c);
                            temp.Replace("//", "/");
                            lastWriteTime = f.LastWriteTime;
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

