using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace GreyB0t
{
    class ComUptime : Command
    {
        String uptimeString;
        public static bool isUp;
        public ComUptime()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>(); ;
            //define the Roll commands
            invocation.Add("!uptime", "Grabs uptime of stream");
            uptimeString = "";
            isUp = false;
        }

        public override void ParseCommand(Message m)
        {
            try
            {
                string raw = new WebClient().DownloadString("https://api.twitch.tv/kraken/streams?channel=tuesdaygrey");
                string date = "";
                string time = "";
                int starter = raw.IndexOf("created_at") + 13;
                date = raw.Substring(starter, 10);
                time = raw.Substring(starter + 11, 8);

                DateTime streamStart;
                string dateTime = date + " " + time;
                DateTime.TryParse(dateTime, out streamStart);
                streamStart = streamStart.AddHours(-5);
                //Console.WriteLine("Got DateTime: "+streamStart.ToString());


                TimeSpan uptime = DateTime.Now - streamStart;

                Console.WriteLine("Got upTime: " + uptime);
                isUp = true;
                foreach (KeyValuePair<String, String> kvp in invocation)
                {
                    if (m.tell.StartsWith(kvp.Key))
                    {
                        Console.WriteLine(m.username + " requested uptime.");
                        String returnMsg = "NULL msg from ComUptime";
                        returnMsg = "/me says: Uptime: " + uptime.Hours + " hours " + uptime.Minutes.ToString("00") + " minutes.";
                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                    }
                }
            }
            catch (Exception e)
            {
                String returnMsg = "NULL msg from ComUptime";
                returnMsg = "/me says: Current uptime info unavailable";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                isUp = false;
            }
        }

        public static bool GetIsUp()
        {
            try
            {
                string raw = new WebClient().DownloadString("https://api.twitch.tv/kraken/streams?channel=tuesdaygrey");
                string date = "";
                string time = "";
                int starter = raw.IndexOf("created_at") + 13;
                date = raw.Substring(starter, 10);
                time = raw.Substring(starter + 11, 8);

                DateTime streamStart;
                string dateTime = date + " " + time;
                DateTime.TryParse(dateTime, out streamStart);
                streamStart = streamStart.AddHours(-5);
                //Console.WriteLine("Got DateTime: "+streamStart.ToString());


                TimeSpan uptime = DateTime.Now - streamStart;

                Console.WriteLine("Got upTime: " + uptime);

                Console.WriteLine("requested uptime.");
                String returnMsg = "NULL msg from ComUptime";
                returnMsg = "/me says: Uptime: " + uptime.Hours + " hours " + uptime.Minutes.ToString("00") + " minutes.";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                isUp = true;

            }
            catch (Exception e)
            {
                String returnMsg = "NULL msg from ComUptime";
                returnMsg = "/me says: Current uptime info unavailable";
                MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                isUp = false;
            }
            return isUp;
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !uptime " + s);
        }

        public override void CleanUp()
        {
            //undefined for !roll
        }
    }
}

