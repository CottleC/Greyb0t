using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GreyB0t
{
    class ComVote : Command
    {
        public String topic;
        public List<String> validOptions;
        public Dictionary<string,string> results;
        public bool isActive = false;
        public int runTime;
        public int numEntrants;

        public ComVote()
        {
            invocation = new Dictionary<String, String>();
            validOptions = new List<String>();
            results = new Dictionary<string, string>();
            //define the vote commands
            invocation.Add("!vote", "Casts a vote. Example:!vote yes");
            runTime = -1;
            numEntrants = 0;
        }

        public ComVote(String desiredTopic, List<String> options, int runTime)
        {
            invocation = new Dictionary<String, String>();
            validOptions = new List<String>();
            results = new Dictionary<string, string>();
            this.topic = desiredTopic;
            //define the vote commands
            if (!invocation.ContainsKey("!vote"))
            {
                invocation.Add("!vote", "Casts a vote. Example:!vote yes");
            }
            this.runTime = runTime;
            numEntrants = 0;
            //define the vote options
            foreach(String s in options)
            {
                validOptions.Add(s);
            }
        }

        public ComVote(ComVote c)
        {
            numEntrants = 0;
            invocation = c.invocation;
            validOptions = c.validOptions;
            results = c.results;
            isActive = true;
            this.runTime = c.runTime;
            this.topic = c.topic;
            String returnMsg = "/me says: " + topic;
            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            Thread thread = new Thread(WarningMsg);
            thread.Start();
        }

        public override void ParseCommand(Message m)
        {
            if (this.isActive)
            {
                foreach (KeyValuePair<String, String> kvp in invocation)
                {
                    if (m.tell.StartsWith(kvp.Key))
                    {
                        //get rid of the !vote part so we can see what vote they made
                        String request = m.tell.Replace("!vote ", "");
                        String returnMsg = "";
                        if (validOptions.Contains(request))
                        {
                            if (results.Keys.Contains(m.username))
                            {
                                //Console.WriteLine(":\tGreyb0t: Successfully Parsed Command as !vote " + request + " but this user has already voted!");
                                MessageProcessor.pendingWhispers.Enqueue(new KeyValuePair<String, String>("psst! You've already participated in this vote, your vote was: " + results[m.username], m.username));
                            }
                            else
                            {
                                //Console.WriteLine(":\tGreyb0t: Successfully Parsed Command as !vote " + request);
                                results.Add(m.username, request);
                                numEntrants++;
                            }
                        }
                        else if (request.Contains("result"))
                        {
                            //Console.WriteLine(":\tGreyb0t: Result Request! " + request);
                            returnMsg = "Current Results: ";
                            int numvotes = results.Values.Count;

                            foreach (String s in validOptions)
                            {
                                if (results.Values.Contains(s))
                                {
                                    int count = results.Count(kp => kp.Value.Contains(s));
                                    returnMsg += " " + s + "=" + (count * 100) / (numvotes) + "%";
                                }
                            }
                            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        }/*
                    else if (request.Contains("clear"))
                    {
                        if (m.user.Equals("goatfight"))
                        {
                            Console.WriteLine(":\tGreyb0t: Clear request! " + request);
                            results.Clear();
                            String response;
                            response = "Vote results cleared";
                            return response;
                        }
                    }
                    */
                        else
                        {
                            //Console.WriteLine(":\tGreyb0t: Couldn't parse !vote as a valid option!");
                            String response = "I didn't understand your vote text!, valid responses for this vote are :";

                            foreach (String s in validOptions)
                            {
                                response += s + " ";
                            }
                            MessageProcessor.pendingWhispers.Enqueue(new KeyValuePair<String, String>(response, m.username));
                        }
                    }
                }
            }
            else
            {
                String returnMsg = "/me says: There are currently no active votes!";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            }
            /*
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                Console.WriteLine(m.tell);
                if (m.tell.StartsWith(kvp.Key))
                {
                    String request = m.tell.Replace("!vote ", "");
                    if (validOptions.Contains(request))
                    {
                        if (results.Keys.Contains(m.user))
                        {
                            Console.WriteLine(":\tGreyb0t: Successfully Parsed Command as !vote " + request + " but this user has already voted!");
                            return "You've already voted! Your vote was '" + results[m.user] + "'";
                        }
                        else
                        {
                            Console.WriteLine(":\tGreyb0t: Successfully Parsed Command as !vote " + request);
                            results.Add(m.user, request);
                            return "You voted '"+request+"'!";
                        }
                    }
                    else if (request.Contains("result"))
                    {
                        Console.WriteLine(":\tGreyb0t: Result Request! " + request);
                        String response;
                        response = "Current Results: ";
                        int numvotes = results.Values.Count;

                        foreach (String s in validOptions)
                        {
                            if (results.Values.Contains(s))
                            {
                                int count = results.Count(kp => kp.Value.Contains(s));
                                response += " " + s + "=" + (count*100)/(numvotes) + "%";
                            }
                        }
                        return response;
                    }
                    else if (request.Contains("clear"))
                    {
                        if (m.user.Equals("goatfight"))
                        {
                            Console.WriteLine(":\tGreyb0t: Clear request! " + request);
                            results.Clear();
                            String response;
                            response = "Vote results cleared";
                            return response;
                        }
                    }
                    else
                    {
                        Console.WriteLine(":\tGreyb0t: Couldn't parse !vote as a valid option!");
                        String response = "I didn't understand your vote text!, valid responses for this vote are :";

                        foreach (String s in validOptions)
                        {
                            response += s + " ";
                        }
                        return response;
                    }

                }
            }
            return "/me blinks";
          * */
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !vote " + s);
        }

        public void WarningMsg()
        {
            if (isActive == true)
            {
                Thread.Sleep((runTime / 2) * 60000);//runtime is in minutes
                Console.WriteLine("Enter Warning");
                String returnMsg = "/me says: Only " + (runTime / 2) + " minutes left on the vote! : "+topic;
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                //wait till the end
                Thread.Sleep((runTime / 2) * 60000);
                isActive = false;
                returnMsg = EndVote();
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            }
        }

        public String EndVote()
        {
            if (results.Count > 0)
            {
                String result = "/me says: The Vote has ended! There were " + numEntrants + " entrants. The results are: ";

                foreach (String s in validOptions)
                {
                    if (results.Values.Contains(s))
                    {
                        int count = results.Count(kp => kp.Value.Contains(s));
                        result += " " + s + "=" + (count * 100) / (numEntrants) + "%";
                    }
                }
                return result;
            }
            else
            {
                String result = "/me says: The vote has ended with no entrants for some reason!";
                return result;
            }
        }

        public override void CleanUp()
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Console.WriteLine("Writing votefile to " + CommandProcessor.path + "results_vote.json");
            File.WriteAllText(CommandProcessor.path + "results_vote.json", jsonString);
        }
    }
}

