using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComPermit : Command
    {
        private static Dictionary<String, DateTime> permits;
        private static int timeAllowed;

        public ComPermit()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            permits = new Dictionary<String, DateTime>();
            timeAllowed = 180;
            //define the Roll commands
            invocation.Add("!permit", "allows a user to post a link for 3 minutes");
            invocation.Add("*", "all messages are passed through this class");
        }

        public override void ParseCommand(Message m)
        {
            foreach(KeyValuePair<String, String> kvp in invocation)
            {
                //Console.WriteLine(m.tell);
                if (m.tell.StartsWith(kvp.Key) && (CommandProcessor.megaUsers.Contains(m.username) || CommandProcessor.superUsers.Contains(m.username)) && (!kvp.Key.Equals("*")))
                {
                    //get rid of the !vote part so we can see what vote they made
                    String request = m.tell.Replace("!permit ", "");
                    request = request.ToLower();
                    Console.WriteLine(m.username + " requested a permit for: " + m.tell);
                    if (m.tell.Count() > 1)
                    {
                        if (!permits.ContainsKey(request))
                        {
                            String returnMsg = "NULL msg from ComPermit";
                            returnMsg = "/me says: " + request + " can post a link for 3 minutes";
                            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                            permits.Add(request, DateTime.Now);
                        }
                        
                    }
                    else
                    {
                        String returnMsg = "pssst! !permit command failed. perhaps you didn't specify a user OR you aren't a mod!";
                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                    }
                }
                else if(kvp.Key.Equals("*"))
                {
                    //Console.WriteLine("Generic tell");
                    if ((m.tell.Contains(".com")) || (m.tell.Contains(".tv")) || (m.tell.Contains(".net")) || (m.tell.Contains(".org")) || (m.tell.Contains(".eu")) || (m.tell.Contains(".co.uk") || (m.tell.Contains("youtu.be")) || (m.tell.Contains(".us"))))
                    {
                        if ((permits.ContainsKey(m.username)))
                        {
                            TimeSpan timeSince = DateTime.Now - permits[m.username];
                            if (timeSince.Seconds < timeAllowed)
                            {   //inform the user that they consumed a permit
                                String returnMsg = "NULL msg from ComPermit";
                                returnMsg = "/me says: " + m.username + " consumed a permit to say this: " + m.tell;
                                KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                permits.Remove(m.username);
                            }
                            else
                            {   //oh noes, you had a permit but it became lost to the ravages of time
                                String returnMsg = "pssst! your link permit, it expired![" + timeSince.Seconds + "/" + timeAllowed + "]";
                                KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                                MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                permits.Remove(m.username);
                            }
                        }
                        else
                        {
                            //You didn't have a permit to post a link
                            String returnMsg = "NULL msg from ComPermit";
                            returnMsg = ".timeout " + m.username + " 1";
                            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        }
                    }
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !permit " + s);
        }

        public override void CleanUp()
        {
            //undefined for !permit
        }
    }
}