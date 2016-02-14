using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GreyB0t
{

    static class CommandProcessor
    {
        public static Message theMsg;
        public static String command;
        public static String path;
        public static List<String> superUsers;
        public static List<String> megaUsers; //megausers>superusers
        private static Dictionary<String,Command> commandList;

        public static void Find(Message input)
        {
            command = "NULL";
            theMsg = new Message(input);
            command = ExtractCommand();
        }

        public static void Init()
        {
            commandList = new Dictionary<String, Command>();
            String exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = exe.Remove(exe.LastIndexOf('\\') + 1);
            Console.WriteLine(":\tGreyb0t: The path is: " + path);
            List<Command> tempList = new List<Command>();//add everything in a forloop
            superUsers = new List<String>();
            megaUsers = new List<String>();
            ComRoll roll = new ComRoll();
            tempList.Add(roll);
            ComTime time = new ComTime();
            tempList.Add(time);
            ComEnter enter = new ComEnter();
            tempList.Add(enter);
            ComPM pm = new ComPM();
            tempList.Add(pm);
            ComVote voting = new ComVote();
            tempList.Add(voting);
            ComRequestServerInfo serverInfo = new ComRequestServerInfo();
            tempList.Add(serverInfo);
            ComUptime uptime = new ComUptime();
            tempList.Add(uptime);
            ComDance dance = new ComDance();
            tempList.Add(dance);
            ComLurk lurk = new ComLurk();
            tempList.Add(lurk);
            ComPermit permit = new ComPermit();
            tempList.Add(permit);
            ComSay say = new ComSay();
            tempList.Add(say);
            ComPoinks poinks = new ComPoinks();
            tempList.Add(poinks);
            ComSun sun = new ComSun();
            tempList.Add(sun);
            ComTest test = new ComTest();
            tempList.Add(test);
            //ComHunt hunt = new ComHunt();
            //dont add hunt because it loads from a file if necessary
            //tempList.Add(hunt);
            
            //update this some other time

            foreach (Command c in tempList)
            {
                foreach (KeyValuePair<String, String> kvp in c.invocation)
                {
                        commandList.Add(kvp.Key, c);
                }
            }


            EventProcessor.AddGBEvent(new GBEventGiveAwaySentinel(10));
            EventProcessor.AddGBEvent(new GBEventHuntFile(600));//10 minutes
            EventProcessor.AddGBEvent(new GBEventVoteSentinel(10));
            EventProcessor.AddGBEvent(new GBEventServerSentinel(10));
            EventProcessor.AddGBEvent(new GBEventFreshenMods(60));
            EventProcessor.AddGBEvent(new GBEventPeriodicMessages(10));
            EventProcessor.AddGBEvent(new GBEventChannelRank(60));
            //ProcessVotes(); //loads in all active votes (should only be 1
            //ProcessGiveAways();
            
            //builds a votefile
            
            /*
            List<String> ops = new List<String>();
            ops.Add("yes");
            ops.Add("no");
            ComVote vote = new ComVote("Use the Green Screen?", ops, 2);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(vote);
            Console.WriteLine(path + "vote_gs.json");
            File.WriteAllText(path+"vote_gs.json",jsonString);
            */
            
            
            //builds a giveaway file
            /*
            String topic = "Giveaway topic";
            int runTime = 2;
            ComEnter giveaway = new ComEnter();
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(giveaway);
            Console.WriteLine(path + "giveaway_gs.json");
            File.WriteAllText(path + "giveaway_gs.json", jsonString);
            */

            //builds a serverinfo file
            /*
            String topic = "yourmom.com";
            ComRequestServerInfo s = new ComRequestServerInfo(topic);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(s);
            Console.WriteLine(path + "server_info.json");
            File.WriteAllText(path + "server_info.json", jsonString);
            */
            
            /*
            //Builds a Users file
            Users users = new Users();
            users.megaUsers.Add("TuesdayGrey");
            users.megaUsers.Add("GoatFight");
            users.superUsers.Add("Dendi");
            users.superUsers.Add("Puppy");
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            Console.WriteLine(path + "users.json");
            File.WriteAllText(path + "users.json", jsonString);
            */

            /*
            //Builds a ConnectionInfo file
            ConnectionInfo ci = new ConnectionInfo(6667, "irc.twitch.tv", "#tuesdaygrey", "tuesdaygrey", "greyB0t", "oauth:censored:p", false);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ci);
            Console.WriteLine(path + "cnxn_std.json");
            File.WriteAllText(path + "cnxn_std.json", jsonString);
            */

            
            //Builds a Poinks file
            /*
            ComPoinks p = new ComPoinks();
            p.poinks.Add("goatFight", new KeyValuePair<uint, String>(0,"knob"));
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(p);
            Console.WriteLine(path + "poinksDB.json");
            File.WriteAllText(path + "poinksDB.json", jsonString);
            */
            
            //builds a periodic message
            //PeriodicMessage pmsg = new PeriodicMessage("Hi! I'm a test periodic message! I will repeat in 5 minutes.",5,00,true);
            //pmsg.CleanUp();

        }

        //overwrite an existing command or add a new one
        public static void InsertCommand(Command c)
        {
            foreach (KeyValuePair<String, String> kvp in c.invocation)
            {
                if (!commandList.ContainsKey(kvp.Key))
                {
                    Console.WriteLine(":\tGreyb0t: new command!: " + kvp.Key);
                    commandList.Add(kvp.Key, c);
                }
                else
                {
                    commandList[kvp.Key] = c;
                }
            }
        }

        private static String ExtractCommand()
        {
            try
            {
                if (theMsg != null)
                {
                    if (theMsg.isAMessage)
                    {
                        bool foundCommand = false;
                        foreach (KeyValuePair<String, Command> kvp in commandList)
                        {
                            string[] thisTell = theMsg.tell.ToString().Split(' ');
                            if (thisTell.Count() > 0)
                            {
                                if (thisTell[0].Equals(kvp.Key))
                                {
                                    Console.WriteLine(":\tGreyb0t: received command: " + kvp.Key + " from user, " + theMsg.username);
                                    kvp.Value.ParseCommand(theMsg);
                                    foundCommand = true;
                                }
                                else if (kvp.Key.Equals("*"))
                                {
                                    //Console.WriteLine(":\tGreyb0t: received command: " + kvp.Key + " from user, " + theMsg.username);
                                    kvp.Value.ParseCommand(theMsg);
                                    foundCommand = true;
                                }
                            }
                        }
                        if ((foundCommand == false) && theMsg.tell.Count() > 0 && theMsg.tell.StartsWith("!"))
                        {
                            MessageProcessor.pendingWhispers.Enqueue(new KeyValuePair<String, String>("Psst!... I don't know that command yet", theMsg.username));
                        }
                    }
                    else if (theMsg.isCAPS)
                    {
                        ChannelInfo.CAPSUserUpdate(theMsg);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in ExtractCommand");
            }
            return "";
        }

        public static void ProcessExit()
        {
            Console.WriteLine(":\tGreyb0t: CommandProcessor is going out of scope! Writing valuable data to disk...");
            foreach(Command c in commandList.Values)
            {
                c.CleanUp();
            }
        }

    }
}
