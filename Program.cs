using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace GreyB0t
{
    class Program
    {

        static void Main(string[] args)
        {
            //required inits
            EventProcessor.Init();
            CommandProcessor.Init();
            MessageProcessor.Init();

            Console.CancelKeyPress += delegate
            {
                //if the app is closed gracefully write transient data to disk
                CommandProcessor.ProcessExit();
            };

            //for each file named cnxn_*.json in the bot dir, connect to a server with the provided info
            //expected use: 2 files, one for whisper server and one for regular twitch irc
            string[] files = Directory.GetFiles(CommandProcessor.path);
            foreach (string temp in files)
            {
                FileInfo f = new FileInfo(temp);
                if (temp.EndsWith(".json"))
                {
                    String filename = temp.Substring(CommandProcessor.path.Count());
                    if (filename.StartsWith("cnxn_"))
                    {
                        ConnectionInfo ci;
                        ci = JsonConvert.DeserializeObject<ConnectionInfo>(File.ReadAllText(temp));
                        ci = new ConnectionInfo(ci);
                        Console.WriteLine(":\tGreyb0t: Deserialized cnxn info file @: " + temp);
                        Bot aBot = new Bot(ci);
                        Thread thr = new Thread(aBot.Connect);
                        thr.Start();
                    }
                }
            }

        }
    }
}
