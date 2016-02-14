using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Net;

namespace GreyB0t
{
    class ComHunt : Command
    {
        public Dictionary<String, KeyValuePair<DateTime,uint>> players; 
        public Dictionary<String, DateTime> hiders;
        public Dictionary<String, KeyValuePair<String,uint>> hx;
        public List<String> targets;
        public double hideTime = 10;
        public static bool isLoaded = false;//load in from json on if we aren't already loading in from json
        public static String[] killMethods = { "struck", "flayed", "stuck", "drown", "whacked", "burned", "hamstrung", "smashed", "smothered", "poisoned", "bludgeoned", "crushed", "bled", "stymied", "\"misplaced\""};
        public static int cooldown = 1;
        public static uint startingCurrency = 100;
        public static String currency = " Sprinkles";

        public ComHunt()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            players = new Dictionary<String, KeyValuePair<DateTime, uint>>();
            targets = new List<String>();
            hiders = new Dictionary<String, DateTime>();
            hx = new Dictionary<string, KeyValuePair<String, uint>>();
            //define the poinks commands
            invocation.Add("!hunt", "Hunts a user");
            invocation.Add("!hide", "lowers chance of being hunted");
        }

        public ComHunt(ComHunt c)
        {
            theMsg = c.theMsg;
            invocation = c.invocation;
            players = c.players;
            targets = c.targets;
            hiders = c.hiders;
            hx = c.hx;
            this.invocation = c.invocation;
            if (!invocation.ContainsKey("!hunt"))
            {
                invocation.Add("!hunt", "Hunts another player");
            }
            if (!invocation.ContainsKey("!hide"))
            {
                invocation.Add("!hide", "hides!");
            }
            //define the poinks commands
        }

        public override void ParseCommand(Message m)
        {
            foreach (KeyValuePair<String, String> kvp in invocation)
            {

                if (m.tell.StartsWith(kvp.Key) && kvp.Key.Equals("!hunt"))
                {
                    //if the player already exists in the playersDB
                    if (m.tell.Contains("info"))
                    {
                        String name = m.tell.Replace("!hunt info", "");
                        name = name.ToLower();
                        if (name.Count() < 1)
                        {
                            name = m.username;
                        }
                        else
                        {
                            name = name.Trim();
                            if(name.Contains('@'))
                            {
                                name = name.Remove('@');
                            }
                        }

                        if (players.ContainsKey(name))
                        {
                            TimeSpan remaining = players[name].Key.AddHours(cooldown) - DateTime.Now;
                            if (remaining.TotalMinutes.ToString("00").Contains("-"))
                            {
                                if(hx.ContainsKey(name))
                                {
                                    String msg = "/me says: " + name + " is worth " + players[name].Value + currency +" and off hunting cooldown(Vendetta:[" + hx[name].Key + "] for [" + hx[name].Value + "]).";
                                    MessageProcessor.pendingAllspeaks.Enqueue(msg);
                                }
                                else
                                {
                                    String msg = "/me says: " + name + " is worth " + players[name].Value + currency + " and off hunting cooldown(Vendetta: Unknown!).";
                                    MessageProcessor.pendingAllspeaks.Enqueue(msg);
                                }
                            }
                            else
                            {
                                if (hx.ContainsKey(name))
                                {
                                    String msg = "/me says: " + name + " is worth " + players[name].Value + currency + " and on hunting cooldown for " + remaining.TotalMinutes.ToString("00") + " more minutes.(Vendetta:[" + hx[name].Key + "] for [" + hx[name].Value + "]).";
                                    MessageProcessor.pendingAllspeaks.Enqueue(msg);
                                }
                                else
                                {
                                    String msg = "/me says: " + name + " is worth " + players[name].Value + currency + " and on hunting cooldown for " + remaining.TotalMinutes.ToString("00") + " more minutes.(Vendetta: Unknown!).";
                                    MessageProcessor.pendingAllspeaks.Enqueue(msg);
                                }
                            }
                        }
                        else
                        {
                            String msg = "/me says: " + name + " hasn't played yet, but everyone starts worth " + startingCurrency + currency + " and off hunting cooldown.";
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                        }

                    }
                    else if (m.tell.Contains("refresh") && CommandProcessor.megaUsers.Contains(m.username))
                    {
                        String name = m.tell.Replace("!hunt refresh", "");
                        if(name.Count()>1)
                        {
                            name = name.Trim();
                            if (players.ContainsKey(name))
                            {
                                players[name] = new KeyValuePair<DateTime, uint>(DateTime.Now.AddHours(-25), players[name].Value);
                                String msg = "/me says: " + name + "'s hunting cooldowns are reset.";
                                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                            }
                            else
                            {
                                String msg = "/me says: "+m.username+", I can't find the user\""+name+"\"";
                                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                            }
                            
                        }
                        else
                        {
                            List<String> temp = new List<string>();
                            temp = players.Keys.ToList<String>();
                            foreach (String user in temp)
                            {
                                players[user] = new KeyValuePair<DateTime, uint>(DateTime.Now.AddHours(-25), players[user].Value);
                            }

                            String msg = "/me says: All hunting cooldowns are reset.";
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                        }
                    }
                    else if (m.tell.Contains("restart") && CommandProcessor.megaUsers.Contains(m.username))
                    {
                        m.tell = m.tell.Replace("!hunt restart ", "");
                        if (players.ContainsKey(m.tell.ToLower()))
                        {
                            String msg = "/me says: " + m.tell + " has been restarted";
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                            players[m.tell.ToLower()] = new KeyValuePair<DateTime, uint>(players[m.tell.ToLower()].Key.AddDays(-1), 100);
                        }
                        else
                        {
                            String returnMsg = "NULL msg from ComPM";
                            returnMsg = "I couldn't find a user with the name: "+ m.tell.ToLower() +" to restart!";
                            KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                            MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                        }
                    }
                    else if (m.tell.Contains("score"))
                    {
                        if (players.Count() > 5)
                        {
                            List<KeyValuePair<String, uint>> topPlayers = new List<KeyValuePair<String, uint>>();
                            foreach (KeyValuePair<String, KeyValuePair<DateTime, uint>> item in players)
                            {
                                KeyValuePair<String, uint> entry = new KeyValuePair<string, uint>(item.Key, item.Value.Value);
                                topPlayers.Add(entry);
                            }

                            topPlayers.Sort(delegate(KeyValuePair<String, uint> x, KeyValuePair<String, uint> y) { return x.Value.CompareTo(y.Value); });

                            String msg = "/me says: The top 5 players are: ";
                            for (int i = topPlayers.Count()-1; i > topPlayers.Count()-6; i--)
                            {
                                if(i == topPlayers.Count()-1)
                                {
                                    msg += topPlayers.ElementAt(i).Key + "[" + topPlayers.ElementAt(i).Value+"]";
                                }
                                else if (i == topPlayers.Count()-5)
                                {
                                    msg += " and " + topPlayers.ElementAt(i).Key + "[" + topPlayers.ElementAt(i).Value+"].";
                                }
                                else
                                {
                                    msg += ", " + topPlayers.ElementAt(i).Key + "[" + topPlayers.ElementAt(i).Value+"]";
                                }
                            }
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                        }
                        else
                        {
                            String msg = "/me says: There aren't enough players for a leaderboard.";
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                        }
                    }
                    else
                    {
                        //hunt a specific person
                        String name = m.tell.Replace("!hunt","");
                        if (name.Count() > 1)
                        {
                            name = name.Trim();
                        }
                        //TODO add the stuff for targetted hunting

                        if (players.ContainsKey(m.username))
                        {
                            if (!ChannelInfo.isStreaming)
                            {
                                String msg = "/me says: Hunting is only active while the stream is live!";
                                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                                return;
                            }
                            //if they haven't hunted in 24 hours
                            DateTime temp = players[m.username].Key;
                            TimeSpan remaining = players[m.username].Key.AddHours(cooldown) - DateTime.Now;
                            if (remaining.TotalMinutes > 0)
                            {
                                String msg = "/me says:" + m.username + " is on hunting cooldown for " + remaining.TotalMinutes.ToString("00") + " more minutes.";
                                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                            }
                            else
                            {
                                ThreadStart threadStart = delegate { KickoffHunt(m.username, 1); };
                                Thread thread = new Thread(threadStart);
                                thread.Start();
                            }
                        }
                        else
                        {
                            KeyValuePair<DateTime, uint> info = new KeyValuePair<DateTime, uint>(DateTime.Now.AddHours(-25), 100);
                            players.Add(m.username, info);
                            ThreadStart threadStart = delegate { KickoffHunt(m.username, 1); };
                            Thread thread = new Thread(threadStart);
                            thread.Start();
                        }
                        
                    }
                }
                else if (m.tell.StartsWith(kvp.Key) && kvp.Key.Equals("!hide"))
                {
                    if (hiders.ContainsKey(m.username))
                    {
                        DateTime temp = hiders[m.username];
                        TimeSpan diff = DateTime.Now - temp;
                        //if this hider should be moved because they've been hiding for more than 10 minutes
                        if (diff > TimeSpan.FromMinutes(hideTime))
                        {
                            hiders.Remove(m.username);
                            hiders.Add(m.username, DateTime.Now);
                        }
                        else
                        {
                            String msg = "/me says:" + m.username + " is already hidden! [ for " + ((int)hideTime - diff.TotalMinutes).ToString("00") + " more minutes]";
                            MessageProcessor.pendingAllspeaks.Enqueue(msg);
                        }
                    }
                    else
                    {
                        hiders.Add(m.username, DateTime.Now);
                    }
                }
            }
        }


        public void KickoffHunt(String hunter, int numtargets)
        {
            if (numtargets == 1)
            {
                String returnMsg = "NULL msg from ComHunt";
                returnMsg = "/me says:" + hunter + " is prowling... ";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //10 second sleep
                Thread.Sleep(10000);
                HuntLogic(hunter, numtargets);
            }
            else
            {
                String returnMsg = "NULL msg from ComHunt";
                returnMsg = "/me is enraged and slinks off into the shadows... ";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //10 second sleep
                Thread.Sleep(10000);
                HuntLogic(hunter, numtargets);
            }
            
        }

        public void HuntLogic(String hunter, int numTargets)
        {
            try
            {
                string raw = new WebClient().DownloadString("http://tmi.twitch.tv/group/user/"+Bot.owner+"/chatters");
                string mods = "";
                string viewers = "";
                //mods are the first occurence of users between []
                int starter = raw.IndexOf('[');
                int ender = raw.IndexOf(']');
                mods = raw.Substring(starter, ender-starter);
                //users are the first occurrence of [] after the word viewers
                raw = raw.Substring(raw.IndexOf("viewers"), raw.Length - raw.IndexOf("viewers"));
                starter = raw.IndexOf('[');
                ender = raw.IndexOf(']');
                viewers = raw.Substring(starter, ender - starter);
                mods = mods.Replace("\n", "");
                mods = mods.Replace("[", "");
                mods = mods.Replace("\\", "");
                mods = mods.Replace("\"", "");
                viewers = viewers.Replace("\n","");
                viewers = viewers.Replace("[", "");
                viewers = viewers.Replace("\\", "");
                viewers = viewers.Replace("\"", "");
                string sTargs = mods + ',' + viewers;
                sTargs = sTargs.Replace(" ", "");
                targets = sTargs.Split(',').ToList();
            }
            catch (Exception e)
            {
                String returnMsg = "NULL msg from ComHunt";
                returnMsg = "/me says: this hunt had issues!";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            }
            //if a target does not exist in the players list, add them
            foreach (String s in targets)
            {
                if(!players.ContainsKey(s))
                {
                    KeyValuePair<DateTime, uint> kvp = new KeyValuePair<DateTime, uint>(DateTime.Now.AddHours(-25), 100);
                    players.Add(s, kvp);
                }
            }

            List<String> finalTargets = new List<String>();
            //if people hide, add them to this list once
            //if people haven't hidden, add them twice
            foreach (String s in targets)
            {
                //don't add the hunter to this list
                if(!s.Equals(hunter))
                {
                    //add this person
                    finalTargets.Add(s);

                    if (hiders.ContainsKey(s))
                    {
                        DateTime temp = hiders[s];
                        TimeSpan diff = DateTime.Now - temp;
                        //if this hider should be moved because they've been hiding for more than 10 minutes
                        if (diff > TimeSpan.FromMinutes(hideTime))
                        {
                            hiders.Remove(s);
                            finalTargets.Add(s);
                        }
                        else
                        {
                            //do not double add this person 
                        }
                    }
                    else
                    {
                        finalTargets.Add(s);
                    }
                }
            }
            List<String> hits = new List<String>();
            List<int> vals = new List<int>();
            Random r = new Random();

            for (int i = 0; i < numTargets; i++)
            {
                //choose a target
                int val = r.Next(0, finalTargets.Count());
                String hit = finalTargets.ElementAt(val);
                uint potentialMonies = players[hit].Value;
                val = r.Next(100, (int)players[hit].Value);

                //reset the target to 100 monies if they fell below 100
                if ((players[hit].Value - val) < 100)
                {
                    KeyValuePair<DateTime, uint> targetVals = new KeyValuePair<DateTime, uint>(players[hit].Key, 100);
                    players[hit] = targetVals;
                }
                else
                {
                    KeyValuePair<DateTime, uint> targetVals = new KeyValuePair<DateTime, uint>(players[hit].Key, players[hit].Value - (uint)val);
                    players[hit] = targetVals;
                }

                //put cooldown on hunter
                //give target monies to the hunter
                players[hunter] = new KeyValuePair<DateTime, uint>(DateTime.Now, (uint)val + players[hunter].Value);
                //take the target off cooldown if there are enough players
                if (finalTargets.Count() > 16)
                {
                    players[hit] = new KeyValuePair<DateTime, uint>(DateTime.Now.AddHours(-25), players[hit].Value);
                }
                else
                {
                    players[hit] = new KeyValuePair<DateTime, uint>(players[hit].Key, players[hit].Value);
                }

                hits.Add(hit);
                vals.Add(val);
                if (hx.ContainsKey(hit))
                {
                    hx.Remove(hit);
                    hx.Add(hit, new KeyValuePair<string, uint>(hunter, (uint)val));
                }
                else
                {
                    hx.Add(hit, new KeyValuePair<string, uint>(hunter, (uint)val));
                }

            }

            String finalHits = "";
            String finalVals = "";

            for (int i = 0; i < hits.Count(); i++)
            {
                if (hits.Count() == 1)
                {
                    finalHits = hits.ElementAt(0);
                    finalVals = vals.ElementAt(0).ToString(); ;
                }
                else if (i == hits.Count() - 1)
                {
                    finalHits += " and " + hits.ElementAt(i);
                    finalVals += " and " + vals.ElementAt(i);
                }
                else
                {
                    finalHits += ", " + hits.ElementAt(i);
                    finalVals += ", " + vals.ElementAt(i).ToString();
                }
            }

            //killtext
            r = new Random();
            if (numTargets == 1)
            {
                int strVal = r.Next(0, killMethods.Count());
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].Value + "). " + finalHits + "'s cooldown(s) refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                CleanUp();
            }
            else if (finalTargets.Count() < 16)
            {
                int strVal = r.Next(0, killMethods.Count());
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].Value + "). " + Bot.handle + " hit so hard that nobody's cooldown was refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                CleanUp();
            }
            else
            {
                int strVal = r.Next(0, killMethods.Count());
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].Value + "). " + finalHits + "'s cooldown(s) refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                CleanUp();
            }

            if (finalHits.Contains(Bot.handle))
            {
                KickoffHunt(Bot.handle,3);
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !poinks " + s);
        }

        public override void CleanUp()
        {
            //defined for !ComHunt
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Console.WriteLine("Writing huntfile to " + CommandProcessor.path + "huntfile.json");
            File.WriteAllText(CommandProcessor.path + "huntfile.json", jsonString);
        }
    }
}