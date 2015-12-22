using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GreyB0t
{
    class ComEnter : Command
    {
        public String topic;
        public HashSet<String> results;
        public int numEntrants;
        public int runTime;
        public bool isActive;

        public ComEnter()
        {
            topic = "";
            invocation = new Dictionary<String, String>();
            results = new HashSet<String>();
            numEntrants = 0;
            //define the vote commands
            invocation.Add("!enter", "Enters the GiveAway! Example: !enter");
            runTime = 10;
            isActive = false;
        }

        public ComEnter(ComEnter c)
        {
            topic = c.topic;
            invocation = c.invocation;
            results = c.results;
            numEntrants = c.numEntrants;
            //define the vote commands
            if (!invocation.ContainsKey("!enter"))
            {
                invocation.Add("!enter", "Enters the GiveAway! Example: !enter");
            }
            runTime = c.runTime;
            isActive = true;
            String returnMsg = "/me says: " + topic;
            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            Thread thread = new Thread(WarningMsg);
            thread.Start();
        }

        public ComEnter(String desiredTopic, int desiredRuntime)
        {
            topic = desiredTopic;
            invocation = new Dictionary<String, String>();
            results = new HashSet<String>();
            numEntrants = 0;
            if (!invocation.ContainsKey("!enter"))
            {
                invocation.Add("!enter", "Enters the GiveAway! Example: !enter");
            }
            runTime = desiredRuntime;
            isActive = true;
            String returnMsg = "/me says: "+topic;
            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            Thread thread = new Thread(WarningMsg);
            thread.Start();
        }

        public String EndGiveAway()
        {
            if (results.Count > 0)
            {
                String result = "/me says: The GiveAway has ended! There were " + numEntrants + " entrants. The winner is: ";
                Random r = new Random();
                int val = r.Next(0, results.Count());
                result += results.ElementAt(val);
                return result;
            }
            else
            {
                String result = "/me says: The GiveAway has ended with no entrants for some reason!";
                return result;
            }
        }

        public override void ParseCommand(Message m)
        {
            foreach (KeyValuePair<String, String> kvp in invocation)
            {
                //if the user said one of the invokation commands
                if (m.tell.StartsWith(kvp.Key))
                {
                    String returnMsg = "";
                    if (isActive == false)
                    {
                        returnMsg = "/me says: There are currently no active giveaways!";
                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    }
                    else
                    {
                        if (m.tell.Contains("status"))
                        {
                            returnMsg = "/me says: Current Results: ";
                            returnMsg += numEntrants + " entries. If you entered, then you currently have a 1/" + numEntrants + " chance of winning!";
                            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        }
                        else if (!results.Contains(m.username))
                        {
                            numEntrants++;
                            results.Add(m.username);
                            //returnMsg = "/me says:" + m.username + " has entered the giveaway";
                            //MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        }
                        else
                        {
                            //PM them that they are a dork
                            returnMsg = "pssst! You've already entered this giveaway, dork!";
                            KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                            MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                        }
                        
                    }
                }
            }
            
            /*
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                Console.WriteLine("'"+tell+"'");
                if(tell.StartsWith(kvp.Key))
                {

                    if (tell.ToLower().Equals("!enter"))
                    {
                        if(!results.Contains(user))
                        {
                            Console.WriteLine(user+ " entered the giveaway");
                            numEntrants++;
                            results.Add(user);
                            return user+" has entered the giveaway!";
                        }
                        else
                        {
                            return user+", you are already entered into this giveaway!";
                        }
                    }
                    else if (tell.Contains("status"))
                    {
                        Console.WriteLine(":\tGreyb0t: GiveAway Result Request!: " + tell);
                        String response;
                        response = "Current Results: ";
                        response += numEntrants + " entries. If you entered, then you currently have a 1/"+numEntrants+" chance of winning!"; 
                        return response;
                    }
                    else if (tell.Contains("clear"))
                    {
                        if (user.Equals("goatfight") || user.Equals("tuesdaygrey") || user.Equals("atherma"))
                        {
                            numEntrants = 0;
                            Console.WriteLine(":\tGreyb0t: Clear request!: " + tell);
                            results.Clear();
                            String response;
                            response = "GiveAway entries cleared";
                            return response;
                        }
                        else
                        {
                            return "/me blinks";
                        }
                    }
                    else if (tell.Contains("peek"))
                    {
                        if (user.Equals("goatfight") || user.Equals("tuesdaygrey") || user.Equals("atherma"))
                        {
                            Console.WriteLine(":\tGreyb0t: Peek request!: " + tell);
                            String response;
                            response = "These folks have entered: ";
                            if (results.Count() > 0)
                            {
                                foreach (String s in results)
                                {
                                    response += "'" + s + "'" + " |";
                                }
                            }
                            return response;
                        }
                        else
                        {
                            return "/me blinks";
                        }
                    }
                    else if (tell.Contains("end"))
                    {
                        if(results.Count>0)
                        {
                            if (user.Equals("goatfight") || user.Equals("tuesdaygrey") || user.Equals("atherma"))
                            {
                                Console.WriteLine(":\tGreyb0t: GiveAway End request!: " + tell);
                                return EndGiveAway();
                            }
                            else
                            {
                                return "/me blinks";
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(":\tGreyb0t: Couldn't parse !enter as a valid option!");
                        String response = "I didn't understand your '!enter' text!, valid responses for this command are :'!enter', '!enter status', '!enter peek'[mod] or '!enter clear'[mod]";
                        return response;
                    }

                }
            }
            return "/me blinks";
          * */
        }

        public void WarningMsg()
        {
            if (isActive == true)
            {
                Thread.Sleep((runTime / 2) * 60000);//runtime is in minutes
                Console.WriteLine("Enter Warning");
                String returnMsg = "/me says: Only " + (runTime/2) + " minutes left on the GiveAway!(type !enter to get in!)";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //wait till the end
                Thread.Sleep((runTime / 2) * 60000);
                isActive = false;
                returnMsg = EndGiveAway();
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !enter " + s);
        }

        public override void CleanUp()
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Console.WriteLine("Writing votefile to " + CommandProcessor.path + "results.json");
            File.WriteAllText(CommandProcessor.path + "results.json", jsonString);
        }
    }
}

