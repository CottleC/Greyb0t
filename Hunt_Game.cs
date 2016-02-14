using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GreyB0t
{
    [Serializable]
    static class Hunt_Game
    {
        public static Dictionary<String, Hunt_Player> players = new Dictionary<String, Hunt_Player>(); //list of players in the game! woot
        public static Dictionary<String, Hunt_Player> targets = new Dictionary<String, Hunt_Player>(); //viewer cache
        public static uint hideTime = 10; //in minutes
        public static List<String> killMethods = new List<String>{ "struck", "flayed", "stuck", "drown", "whacked", "burned", "hamstrung", "smashed", "smothered", "poisoned", "bludgeoned", "crushed", "bled", "stymied", "\"misplaced\"" };
        public static uint cooldown = 60; //minutes to refresh player actions
        public static uint startingCurrency = 100;
        public static String currency = "Sprinkles";

        public static void AddPlayer(String name)
        {
            if(!players.ContainsKey(name))
            {
                players.Add(name ,new Hunt_Player(name,startingCurrency));
            }
        }

        public static void RemovePlayer(Hunt_Player given)
        {
            if (players.ContainsKey(given.name))
            {
                players.Remove(given.name);
            }
            else
            {
                //nothing to do
            }
        }

        //ComHunt sends a string (playername) and the game will manage the rest
        public static void KickoffHunt(String hunter, int numtargets)
        {
            if (!players.ContainsKey(hunter))
            {
                AddPlayer(hunter);
            }
            players[hunter].SetLastHuntTime(DateTime.Now);
            if (numtargets == 1)
            {
                String returnMsg = "NULL msg from ComHunt";
                returnMsg = "/me says:" + hunter + " is prowling... ";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //x second sleep
                Thread.Sleep((int)(players[hunter].huntTime*1000));
                DoHunt(hunter, numtargets);
            }
            else
            {
                String returnMsg = "NULL msg from ComHunt";
                returnMsg = "/me is enraged and slinks off into the shadows... ";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //x second sleep
                Thread.Sleep((int)(players[hunter].huntTime * 1000));
                DoHunt(hunter, numtargets);
            }

        }


        private static void DoHunt(String hunter, int numTargets)
        {
            ChannelInfo.UpdateUsers();
            //if a target does not exist in the players list, add them
            foreach (String s in ChannelInfo.channelMembers)
            {
                if (!players.ContainsKey(s))
                {
                    AddPlayer(s.ToLower());
                }

            }

            List<String> finalTargets = new List<String>();
            //if people hide, add them to this list once
            //if people haven't hidden, add them twice
            foreach (String s in targets.Keys)
            {
                //don't add the hunter to this list
                if (!s.Equals(hunter))
                {
                    //add this person
                    finalTargets.Add(s);

                    if (players[s].GetIsHidden())
                    {
                        //don't add
                    }
                    else
                    {
                        finalTargets.Add(s);
                        //add this person again
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
                uint potentialMonies = players[hit].currencies;
                val = r.Next(100, (int)players[hit].currencies);

                if (!players.ContainsKey(hit.ToLower()))
                {
                    AddPlayer(hit.ToLower());
                }

                //reset the target to 100 monies if they fell below 100
                if ((players[hit].currencies - val) < 100)
                {
                    players[hit].SetCurrencies(100, players[hunter]);
                }
                else
                {

                    players[hit].SetCurrencies((uint)(players[hit].currencies-val), players[hunter]);
                }

                //give target monies to the hunter
                
                //take the target off cooldown if there are enough players
                if (finalTargets.Count() > 16)
                {
                    players[hit].SetLastHuntTime(DateTime.Now.AddDays(-1));
                }
                else
                {
                    //players[hit] = new KeyValuePair<DateTime, uint>(players[hit].Key, players[hit].Value);
                }

                hits.Add(hit);
                vals.Add(val);
                players[hit].vendetta = new KeyValuePair<Hunt_Player,uint>(players[hunter],(uint)val);
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
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].currencies + "). " + finalHits + "'s cooldown(s) refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                WriteOut();
            }
            else if (finalTargets.Count() < 16)
            {
                int strVal = r.Next(0, killMethods.Count());
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].currencies + "). " + Bot.handle + " hit so hard that nobody's cooldown was refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                WriteOut();
            }
            else
            {
                int strVal = r.Next(0, killMethods.Count());
                String msg = "/me says:" + hunter + " " + killMethods[strVal] + ": " + finalHits + ", stealing " + finalVals + currency + "!(" + hunter + ":" + players[hunter].currencies + "). " + finalHits + "'s cooldown(s) refreshed.";
                MessageProcessor.pendingAllspeaks.Enqueue(msg);
                WriteOut();
            }

            if (finalHits.Contains(Bot.handle))
            {
                DoHunt(Bot.handle, 3);
            }
        }

        //ComHunt sends a string (playername) and the game will manage the rest
        public static void DoHide(String playerName)
        {
            if (!players.ContainsKey(playerName))
            {
                AddPlayer(playerName);
            }
            players[playerName].SetIsHidden();
        }

        //Reset all player cooldowns
        public static void Refresh()
        {
            foreach (Hunt_Player p in players.Values)
            {
                p.SetLastHuntTime(DateTime.Now.AddDays(-1));
            }
        }

        //Reset player cooldowns
        public static void Refresh(String playerName)
        {
            if (!players.ContainsKey(playerName))
            {
                AddPlayer(playerName);
            }
            players[playerName].SetLastHuntTime(DateTime.Now.AddDays(-1));
        }

        //Restart player
        public static void Restart(String playerName)
        {
            if(players.ContainsKey(playerName))
            {
                RemovePlayer(players[playerName]);
            }
            
        }

        //Display high scores
        public static void DoScores()
        {

        }

        public static Hunt_Player GetPlayer(String name)
        {
            if (players.ContainsKey(name))
            {
                return players[name];
            }
            else
            {
                return new Hunt_Player();
            }
        }

        //writes out the huntdb
        public static void WriteOut()
        {
            //defined for ViewerShip
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(CommandProcessor.path + "HuntGame.db", FileMode.Create, FileAccess.Write, FileShare.None);
            Dictionary<String, Hunt_Player> outPlayers = new Dictionary<String, Hunt_Player>(Hunt_Game.players);
            formatter.Serialize(stream, outPlayers);
            stream.Close();
        }

        public static void ReadIn()
        {
            //defined for ViewerShip
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                if (temp.EndsWith(".db"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("HuntGame"))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Hunt_Game.players = (Dictionary<String, Hunt_Player>)formatter.Deserialize(stream);
                        stream.Close();
                    }
                }
            }
        }
    }
}
