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
    class ComTest : Command
    {

        public ComTest()
        {
            Hunt_Game.ReadIn();
            theMsg = null;
            invocation = new Dictionary<String, String>();
            //define the poinks commands
            invocation.Add("!testhunt", "Hunts a user");
            invocation.Add("!testhide", "lowers chance of being hunted");
        }

        public override void ParseCommand(Message m)
        {
            foreach (KeyValuePair<String, String> kvp in invocation)
            {
                if (m.tell.StartsWith(kvp.Key))
                {
                    string[] args = m.tell.ToString().Split(' ');
                    int numArgs = args.Count();
                    String returnMsg = "";

                    if(numArgs > 0)
                    {
                        if (numArgs == 1)
                        {
                            if (args[0].Equals("!testhunt"))
                            {
                                Hunt_Game.KickoffHunt(m.username, 1);
                            }
                        }
                        else if (numArgs == 2)
                        {
                            if (args[0].Equals("!testhunt") && args[1].Equals("info"))
                            {
                                Hunt_Player temp = Hunt_Game.GetPlayer(m.username);
                                if (temp.GetCooldown() == null)
                                {
                                    returnMsg = "/me says:" + m.username + ": " + temp.currencies + ". Cooldown: none.";
                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                }
                                else
                                {
                                    returnMsg = "/me says:" + m.username + ": " + temp.currencies + ". Cooldown: " + temp.GetCooldown() +".";
                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                }
                            }
                            else if (args[0].Equals("!testhunt") && args[1].Equals("dbinfo"))
                            {
                                uint numSprink = 0;
                                if (Hunt_Game.players.Count() > 0)
                                {
                                    foreach (KeyValuePair<string, Hunt_Player> player in Hunt_Game.players)
                                    {
                                        numSprink += player.Value.currencies;
                                    }
                                }

                                returnMsg = "/me says:" + m.username + ", the database has " + Hunt_Game.players.Count() + " players, greedily guarding "+ numSprink +" Sprinkles.";
                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                            }
                            else if (args[0].Equals("!testhunt") && args[1].Equals("convertdb"))
                            {
                                returnMsg = "/me says:" + m.username + ", I am starting db conversion...";
                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                //loadin the old huntDB and grab all its juicy data
                                {
                                    string[] files = Directory.GetFiles(CommandProcessor.path);
                                    foreach (string temp in files)
                                    {
                                        if (temp.EndsWith(".json"))
                                        {
                                            String filename = temp.Substring(CommandProcessor.path.Count());
                                            if (filename.StartsWith("huntfile"))
                                            {
                                                //read in
                                                ComHunt c = new ComHunt();
                                                c = JsonConvert.DeserializeObject<ComHunt>(File.ReadAllText("huntfile.json"));
                                                c = new ComHunt(c);

                                                //setup the db
                                                foreach (KeyValuePair<String, KeyValuePair<DateTime,uint>> player in c.players)
                                                {
                                                    //add a player to the Hunt_Game with the appropriate currency
                                                    Hunt_Game.AddPlayer(player.Key.ToLower());
                                                    if (player.Value.Value > 100)
                                                    {
                                                        Hunt_Game.players[player.Key.ToLower()].currencies = 0; //dont want to double add this ever
                                                        Hunt_Game.players[player.Key.ToLower()].currencies = player.Value.Value;
                                                    }
                                                    Hunt_Game.players[player.Key.ToLower()].lastHuntTime = DateTime.Now.AddDays(-1);
                                                    Hunt_Game.players[player.Key.ToLower()].hiddenTime = DateTime.Now.AddDays(-1);
                                                    Hunt_Game.players[player.Key.ToLower()].huntTime = 10;
                                                }
                                                //populate the vendetta list
                                                foreach (KeyValuePair<String, KeyValuePair<DateTime, uint>> player in c.players)
                                                {
                                                    //add a player to the Hunt_Game with the appropriate currency
                                                    try
                                                    {
                                                        Hunt_Game.players[player.Key.ToString()].vendetta = new KeyValuePair<Hunt_Player, uint>(Hunt_Game.players[c.hx[player.Key].Key], c.hx[player.Key].Value);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Hunt_Game.players[player.Key.ToString()].vendetta = new KeyValuePair<Hunt_Player, uint>(new Hunt_Player(), (uint)0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                returnMsg = "/me says:" + m.username + ", I finsihed db conversion with " + Hunt_Game.players.Count() + " converted entries.";
                                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                Hunt_Game.WriteOut();
                            }

                        }
                    }
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !testclass " + s);
        }

        public override void CleanUp()
        {
            //undefined for !ComTest
        }
    }
}