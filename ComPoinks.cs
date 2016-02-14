using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace GreyB0t
{
    class ComPoinks : Command
    {
        //the first constructor checks for a poinksDB, the resulting object does not
        public ComPoinks()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            Viewership.Loadin();
            //define the poinks commands
            invocation.Add("!poinks", "Displays some stuff about a user");
            invocation.Add("!rank", "Displays some stuff about a user");
        }

        public ComPoinks(ComPoinks c)
        {
            theMsg = c.theMsg;
            invocation = c.invocation;
            //define the poinks commands
            invocation.Add("!poinks", "Displays some stuff about a user");
            invocation.Add("!rank", "Displays some stuff about a user");
        }

        public override void ParseCommand(Message m)
        {
            if (m.isAWhisper)
            {
                //Console.WriteLine(Viewership.DetermineRank(m.username));
                foreach (KeyValuePair<String, String> kvp in invocation)
                {
                    if (m.tell.StartsWith(kvp.Key) && CommandProcessor.megaUsers.Contains(m.username))
                    {
                        string[] args = m.tell.ToString().Split(' ');
                        int numArgs = args.Count();
                        String returnMsg = "";
                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                        if (numArgs > 0)
                        {
                            //before actually parsing this, check to see if we are adding a custom rank to a user
                            if ((numArgs > 3) && (CommandProcessor.megaUsers.Contains(m.username)))
                            {
                                if (args[1].Equals("setrankflavor"))
                                {
                                    String name = args[2];
                                    String rank = "";
                                    for (int i = 3; i < args.Length; i++)
                                    {
                                        rank += args[i] + " ";
                                    }
                                    Viewership.SetViewerCustomRank(name, rank);
                                }
                            }


                            ChannelInfo.UpdateUsers();
                            switch (numArgs)
                            {   case(1):
                                        returnMsg = Viewership.GetViewer(m.username).name + ":" + Viewership.GetViewer(m.username).GetCoin();
                                        whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                        break;
                                case(3):
                                        if (args[1].Equals("add"))
                                        {
                                            try
                                            {
                                                double given = Convert.ToDouble(args[2]);
                                                Viewership.ApplyUnmodifiedPointsToAll(given);
                                                returnMsg = "Added " + given + " " + Viewership.currencyName + " to everyone!";
                                                returnMsg = "/me says: " + returnMsg;
                                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                            }
                                            catch (Exception e)
                                            {
                                                returnMsg = "I was expecting args[2] to be a number, instead it was:" + args[2];
                                                whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                            }
                                        }
                                        else if (args[1].Equals("+rate"))
                                        {
                                            if (Viewership.viewers.ContainsKey(args[2].ToLower()))//if this arg is the name of a user
                                            {
                                                Viewership.viewers[args[2].ToLower()].AdjustMultiplier(Viewer.subMultiplier);
                                                returnMsg = "Rate:" + Viewership.viewers[args[2].ToLower()].multiplier + " applied to member:" + args[2].ToLower();
                                                returnMsg = "/me says: " + returnMsg;
                                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                            }
                                            else
                                            {
                                                returnMsg = "The user you specified isn't in the poinksdb";
                                                whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                            }
                                        }
                                        else if (args[1].Equals("-rate"))
                                        {
                                            if (Viewership.viewers.ContainsKey(args[2].ToLower()))//if this arg is the name of a user
                                            {
                                                Viewership.viewers[args[2].ToLower()].AdjustMultiplier(Viewer.stdMultiplier);
                                                returnMsg = "Rate:" + Viewership.viewers[args[2].ToLower()].multiplier + " applied to member:" + args[2].ToLower();
                                                returnMsg = "/me says: " + returnMsg;
                                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                            }
                                            else
                                            {
                                                returnMsg = "The user you specified isn't in the poinksdb";
                                                whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                            }
                                        }
                                        break;
                                case(4):
                                        if (args[1].Equals("add"))
                                        {
                                            if (Viewership.viewers.ContainsKey(args[2].ToLower()))//if this arg is the name of a user
                                            {
                                                try
                                                {
                                                    ChannelInfo.UpdateUsers();
                                                    double given = Convert.ToDouble(args[3]);
                                                    Viewership.ApplyPointsToViewer(args[2].ToLower(), given);
                                                    returnMsg = "Added " + given + " " + Viewership.currencyName + " to: " + args[2].ToLower() + "!";
                                                    returnMsg = "/me says: " + returnMsg;
                                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                                }
                                                catch (Exception e)
                                                {
                                                    returnMsg = "I was expecting args[3] to be a number, instead it was:" + args[3];
                                                    whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                                    MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                                }
                                            }
                                            else
                                            {
                                                returnMsg = "The user you specified isn't in the poinksdb...";
                                                whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                            }
                                        }
                                        break;
                                default:
                                        //returnMsg = "Your request to add points didn't conform to any known format";
                                        //whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                        //MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                        break;
                            }
                            /*
                            if (m.tell.Contains("add"))
                            {
                                m.tell = m.tell.Replace("!poinks", "");
                                m.tell = m.tell.Replace("add", "");
                                m.tell = m.tell.Trim();
                                try
                                {
                                    int num = Convert.ToInt32(m.tell);
                                    //user requested poinks
                                    Viewership.ApplyPointsToAll(num);
                                    String returnMsg = "Added " + num + " " + Viewership.currencyName + " to everyone";
                                    returnMsg = "/me says: " + returnMsg;
                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                }
                                catch (Exception e)
                                {
                                    int num = Convert.ToInt32(m.tell);
                                    //user requested poinks
                                    String returnMsg = "Hey you didn't send me a proper number to add";
                                    KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                    MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                }
                            }
                            else
                            {
                                //user requested poinks
                                String returnMsg = Viewership.GetViewer(m.username).name + ":" + Viewership.GetViewer(m.username).GetCoin();
                                KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                            }
                             */
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<String, String> kvp in invocation)
                {
                    if (m.tell.StartsWith(kvp.Key))
                    {
                        String returnMsg = "["+m.username+":"+Viewership.GetViewer(m.username).GetCoin()+ " "+Viewership.currencyName+"]: "+Viewership.DetermineRank(m.username);
                        returnMsg = "/me :" + returnMsg;
                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    }
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !poinks " + s);
        }

        public override void CleanUp()
        {
            Viewership.LoadOut();
        }
    }
}