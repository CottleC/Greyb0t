using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GreyB0t
{
    [Serializable]
    static class Viewership //viewer database
    {
        public static String currencyName = "Cake";
        public static Dictionary<String, Viewer> viewers = new Dictionary<String, Viewer>();
        public static double startingVal = 0;//how much a user starts with
        public static double gainRate = 1.0;//current gainrate of the channel(multiplier 1.1x, 1.5x, etc...)
        public static double pointsPerHour = 16;
        public static List<KeyValuePair<int, String>> tiers = new List<KeyValuePair<int,String>>();

        public static Viewer GetViewer(String name)
        {
            Viewer v = new Viewer(name.ToLower(), 0);
            if (!viewers.ContainsKey(name))
            {
                viewers.Add(name, v);
                return viewers[name];
            }
            else
            {
                return viewers[name];
            }
        }

        public static void AddViewer(String name)
        {
            Viewer v = new Viewer(name.ToLower(), 0);
            if (!viewers.ContainsKey(name))
            {
                viewers.Add(name, v);
            }
        }

        public static void ApplyPointsToAll(double pts)
        {
            if (viewers.Count > 0)
            {
                Console.WriteLine("+" + pts * gainRate);
                foreach(String s in viewers.Keys)
                {
                    if (ChannelInfo.channelMembers.Contains(s.ToLower()))//only apply points to online users
                    {
                        Console.WriteLine(s.ToLower() + " is online to receive poinks.");
                        viewers[s].addCoin(pts * gainRate);
                    }
                }
            }
            Viewership.LoadOut();
        }

        public static void ApplyUnmodifiedPointsToAll(double pts)
        {
            if (viewers.Count > 0)
            {
                foreach (String s in viewers.Keys)
                {
                    if (ChannelInfo.channelMembers.Contains(s.ToLower()))//only apply points to online users
                    {
                        Console.WriteLine(s.ToLower()+" is online to receive poinks.");
                        viewers[s].addCoin(pts);
                    }
                }
            }
            Viewership.LoadOut();
        }

        public static void ApplyPointsToViewer(String name, double amt)
        {
            if (viewers.Count > 0)
            {
                if(viewers.ContainsKey(name))
                {
                    viewers[name].addCoin(gainRate * amt);
                }
            }
            Viewership.LoadOut();
        }

        public static int GetCurrency(String name)
        {
            Viewer v = new Viewer(name.ToLower(), 0);
            if (!viewers.ContainsKey(name))
            {
                return (int)0.0;
            }
            else
            {
                return (int)viewers[name].GetCoin();
            }
        }

        public static void Loadin()
        {
            //defined for ViewerShip
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                if (temp.EndsWith(".db"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("poinks"))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Viewership.viewers = (Dictionary<String, Viewer>)formatter.Deserialize(stream);
                        stream.Close();
                    }
                }
            }
            tiers.Add(new KeyValuePair<int,String>(0,"Stellar Dust Cake"));
            tiers.Add(new KeyValuePair<int, String>(240, "Steller Cloud Cake"));
            tiers.Add(new KeyValuePair<int, String>(720, "Asteroid Cake"));
            tiers.Add(new KeyValuePair<int, String>(1680, "Meteor Cake"));

            tiers.Add(new KeyValuePair<int, String>(3600, "Planet Cake"));
            tiers.Add(new KeyValuePair<int, String>(7200, "Exo Planet Cake"));

            tiers.Add(new KeyValuePair<int, String>(14400, "Protostar Cake"));
            tiers.Add(new KeyValuePair<int, String>(21600, "Dwarf Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(28800, "SuperGiant Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(36000, "HyperGiant Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(43200, "Neutron Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(50400, "Quark Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(57600, "Preon Star Cake"));
            tiers.Add(new KeyValuePair<int, String>(64800, "Wolf-Rayet Cake"));

            tiers.Add(new KeyValuePair<int, String>(86400, "Planetary System Cake"));
            tiers.Add(new KeyValuePair<int, String>(93600, "Binary System Cake"));
            tiers.Add(new KeyValuePair<int, String>(100800, "Triplet System Cake"));

            tiers.Add(new KeyValuePair<int, String>(144000, "Star Cluster Cake"));
            tiers.Add(new KeyValuePair<int, String>(158400, "Nebula Cake"));

            tiers.Add(new KeyValuePair<int, String>(172800, "Spiral Galaxy Cake"));
            tiers.Add(new KeyValuePair<int, String>(216000, "Elliptical Cake"));
            tiers.Add(new KeyValuePair<int, String>(259200, "Galaxy Cluster Cake"));
        }

        public static void SetViewerCustomRank(String name, String rank)
        {
            if (!viewers.ContainsKey(name))
            {
                AddViewer(name);
            }

            if (rank.TrimEnd().Equals("delete"))
            {
                viewers[name].SetCustomRank("null");
            }
            else
            {
                viewers[name].SetCustomRank(rank);
            }
        }

        public static String DetermineRank(String name)
        {
            if (!viewers.ContainsKey(name))
            {
                AddViewer(name);
            }

            if (!viewers[name].GetCustomRank().Equals("null"))
            {
                return viewers[name].GetCustomRank();
            }

            String s = "Indeterminate fiend!";
            int index = 0;
            bool valid = true;
            for (; index < tiers.Count(); index++)
            {
                if (viewers[name].GetCoin() <= tiers.ElementAt(index).Key)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }

                if (valid == false)
                {
                    if (index > 0)
                    {
                        //the previous index is the rank
                        s = tiers.ElementAt(index).Value;
                    }
                    else
                    {
                        //this is the rank
                        s = tiers.ElementAt(index).Value;
                    }
                }
            }
            return s;
        }

        public static void LoadOut()
        {
            //defined for ViewerShip
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(CommandProcessor.path+"poinks.db", FileMode.Create, FileAccess.Write, FileShare.None);
            Dictionary<String, Viewer> outViewers = new Dictionary<String, Viewer>(Viewership.viewers);
            formatter.Serialize(stream, outViewers);
            stream.Close();
        }

    }
}
