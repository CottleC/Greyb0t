using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GreyB0t
{
    class ComRequestServerInfo : Command
    {
        public String topic;
        bool isActive;

        public ComRequestServerInfo()
        {
            topic = "";
            invocation = new Dictionary<String,String>();
            invocation.Add("!server", "Returns serverinfo for the game Tuesday is currently playing");
            isActive = false;
        }

        public ComRequestServerInfo(ComRequestServerInfo c)
        {
            topic = c.topic;
            invocation = c.invocation;
            if (!invocation.ContainsKey("!server"))
            {
                invocation.Add("!server", "Returns serverinfo for the game Tuesday is currently playing");
            }
            isActive = true;
        }

        public ComRequestServerInfo(String desiredTopic)
        {
            topic = desiredTopic;
            invocation = new Dictionary<String, String>();
            if (!invocation.ContainsKey("!server"))
            {
                invocation.Add("!server", "Returns serverinfo for the game Tuesday is currently playing");
            }
            isActive = true;
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
                        //PM them that there is no server info
                        returnMsg = "pssst! There is no serverinfo available at this time!";
                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                    }
                    else
                    {
                        //PM them that there is no server info
                        returnMsg = "pssst! You can join Tuesday at: "+topic;
                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(returnMsg, m.username);
                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                    }
                }
            }
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !RequestServerInfo " + s);
        }

        public override void CleanUp()
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Console.WriteLine("Writing serverinfo to " + CommandProcessor.path + "server_info.json");
            File.WriteAllText(CommandProcessor.path + "server_info.json", jsonString);
        }
    }
}

