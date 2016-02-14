using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace GreyB0t
{
    class GBEventChannelRank : GBEvent
    {
        //events DO stuff on time intervals
        //Typically there is a 
        //kickoff message: vote on [stuff] has started!
        //recurring message: vote has x seconds left!
        //end message: vote has finished! the winnder was god!


        public GBEventChannelRank(int i)
        {
            triggerSeconds = i;
            runTime = -1;
            breaker = false;//flag to stop the thread
            RecurringThing();
        }

        public override void RecurringThing()
        {
            //check uptime
            bool isUp = true;
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
            }
            catch (Exception e)
            {
                isUp = false;
            }
            //start distribution
            double amt = Viewership.pointsPerHour / 60.00;
            if(!isUp)
            {
                Viewership.gainRate = 0.5;
            }
            else
            {
                Viewership.gainRate = 1.0;
            }

            try
            {
                Console.WriteLine("Adding:" + amt + " to viewers. At rate:"+Viewership.gainRate);
                try
                {
                    ChannelInfo.UpdateUsers();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Reverting to cached list for rank addition, exception while connecting to twitch api");
                }
                foreach (String s in ChannelInfo.channelMembers)
                {
                    Viewership.AddViewer(s.ToLower());
                }
                Console.SetCursorPosition(0,Console.CursorTop -1);
                Viewership.ApplyPointsToAll(amt);
            }
            catch (Exception e)
            {
                Console.WriteLine("GBEventChannelRan: exception on periodic point addition interval");
            }
        }

        public override void CleanUp()
        {
            Console.WriteLine("GBEventPeriodicMessages Event is going out of scope...");
        }
    }
}

