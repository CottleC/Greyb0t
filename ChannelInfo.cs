using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace GreyB0t
{
    static class ChannelInfo
    {
        public static HashSet<String> channelMembers = new HashSet<String>();
        public static bool isStreaming = false;

        public static void UpdateUsers()
        {
            int failedcount = 0;
            bool didFail = true;
            if (Bot.handle != null)
            {
                while ((failedcount < 5) && (didFail))
                {
                    try
                    {
                        string raw = new WebClient().DownloadString("http://tmi.twitch.tv/group/user/" + Bot.owner + "/chatters");
                        string mods = "";
                        string viewers = "";
                        //mods are the first occurence of users between []
                        int starter = raw.IndexOf('[');
                        int ender = raw.IndexOf(']');
                        mods = raw.Substring(starter, ender - starter);
                        //users are the first occurrence of [] after the word viewers
                        raw = raw.Substring(raw.IndexOf("viewers"), raw.Length - raw.IndexOf("viewers"));
                        starter = raw.IndexOf('[');
                        ender = raw.IndexOf(']');
                        viewers = raw.Substring(starter, ender - starter);
                        mods = mods.Replace("\n", "");
                        mods = mods.Replace("[", "");
                        mods = mods.Replace("\\", "");
                        mods = mods.Replace("\"", "");
                        viewers = viewers.Replace("\n", "");
                        viewers = viewers.Replace("[", "");
                        viewers = viewers.Replace("\\", "");
                        viewers = viewers.Replace("\"", "");
                        String sTargs = mods + ',' + viewers;
                        sTargs = sTargs.Replace(" ", "");
                        channelMembers = new HashSet<String>(sTargs.Split(','));
                        didFail = false;

                        /*
                        foreach (String s in channelMembers)
                        {
                            Console.WriteLine("Update Found:"+s);
                        }*/
                    }
                    catch (Exception e)
                    {
                        String returnMsg = "NULL msg from ChannelInfo";
                        Console.WriteLine("\tChannelInfoException!");
                        //MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                        failedcount++;
                        didFail = true;
                    }
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
                        streamStart = streamStart.AddHours(-5);//here is where an exception is thrown if the stream isnt up
                        isStreaming = true;
                    }
                    catch (Exception e)
                    {
                        isStreaming = false;
                    }
                }
            }
        }

        public static void CAPSUserUpdate(Message m)
        {
            if(m.tell != null)
            {
                if (m.tell.Equals("JOIN"))
                {
                    AddUser(m.username);
                }
                else if (m.tell.Equals("PART"))
                {
                    RemoveUser(m.username);
                }
            }
        }

        public static void AddUser(String user)
        {
            if((user!=null) && (user.Count() > 0))
            {
                if (!channelMembers.Contains(user))
                {
                    channelMembers.Add(user);
                }
            }
        }

        public static void RemoveUser(String user)
        {
            if ((user != null) && (user.Count() > 0))
            {
                if (!channelMembers.Contains(user))
                {
                    channelMembers.Remove(user);
                }
            }
        }

        public static void PrintUsers()
        {
            //String temp =  "";
            UpdateUsers();
            Console.WriteLine("\tThe following users are in channel:");
            foreach (String member in channelMembers)
            {
                Console.WriteLine(member);
                //temp += " "+member;
            }
            //String msg = "/me says: The following Users are in channel: "+temp;
            //MessageProcessor.pendingAllspeaks.Enqueue(msg);
        }
    }
}
