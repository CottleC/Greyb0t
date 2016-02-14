using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    class Bot
    {
        public bool isWhisperBot;
        public int port;
        /*state 0=purgatory,1=connected to twitch,2=connected to target chan*/
        public int state;
        public String bulwark;
        public static String handle; //the botname is accessible from everywhere
        public static String owner; //the channel owner is acceisble from everywhere
        public String server;
        public String channel;
        public String pass;

        System.Net.Sockets.TcpClient sock;
        System.IO.TextReader input;
        System.IO.TextWriter output;

        public Bot(ConnectionInfo cnxn)
        {
            this.isWhisperBot = cnxn.isWhisperBot;
            this.port = cnxn.port;
            bulwark = "";
            handle = cnxn.botName;
            owner = cnxn.owner;
            this.server = cnxn.serverName;
            this.channel = cnxn.chan;
            pass = cnxn.oAuth;
            sock = new System.Net.Sockets.TcpClient();
            state = 0;//unconnected, haven't tried to connect
        }

        public void Connect()
        {
            //init data fields
            Console.WriteLine("Connecting with: \n\tname: " + handle +
            "\n\tserver: " + server +
            "\n\tchannel: " + channel +
            "\n\tport: " + port + "\n"
            );
            //try to connect
            sock.Connect(server, port);
            if (!sock.Connected)
            {
                Console.WriteLine("wah waahhh.. Failed to connect");
                state = 0;
                return;
            }

            //more setup, this happens if we've connected only
            input = new System.IO.StreamReader(sock.GetStream());
            output = new System.IO.StreamWriter(sock.GetStream());

            //initial logon
            output.Write(
                "PASS " + pass + "\r\n" +
                "USER " + handle + " 0 * :" + owner + "\r\n" +
                "NICK " + handle + "\r\n"
                );
            output.Flush();

            //perform connect spam(if desired)
            ConnectMsg();
            //stays in processesing loop for duration of connection
            ProcessingLoop();
        }

        public void ProcessingLoop()
        {
            try
            {
                while (sock.Connected)
                {
                    Console.WriteLine("\n*****\n*****\n*****Connectin Established" + (isWhisperBot ? "(Whisper)":"(Public)")+"\n*****\n*****\n*****");
                    for (bulwark = input.ReadLine(); ; bulwark = input.ReadLine())
                    {
                        if (bulwark != null)
                        {
                            if (bulwark.Count() > 1)//getting a lot of dead stream data from server
                            {
                                //Console.WriteLine("ProcessingLoop bulwark:" + bulwark);
                                Parse(bulwark);
                            }
                        }
                    }
                }

                while (!sock.Connected)
                {
                    state = 0;
                    Console.WriteLine("\n*****\n*****\n*****Connectin Lost" + (isWhisperBot ? "(Whisper)" : "(Public)") + "\n*****\n*****\n*****");
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ProcessingLoopException");
                Console.WriteLine("Sleeping 10 and trying again...");
                Thread.Sleep(10);
                sock.Close();
                this.Connect();
            }
        }

        //pushes this command to CommandProcessor. If it is a valid command the response is pushed to the appropriate pending queue
        //this class' writeout method is responsible for fetching all output responses
        private String Parse(String theMsg)
        {
            String returnVal = "";
            ParseAutoCommands(theMsg);

            Message msg = new Message(theMsg);
            if (msg.isAMessage)
            {
                //Console.WriteLine(":\tGreyB0t: " + msg.username + " spoke to '" + msg.target + "'" + " and said, \"" + msg.tell + "\"");
                CommandProcessor.Find(msg);
            }

            return returnVal;
        }

        /*responsible for determining when we log on,
         *when we have landed in our target channel,
         *and when we receive a ping. You will be disconnected
         *if you do NOT respond to a ping
         */
        private void ParseAutoCommands(String theMsg)
        {
            if (theMsg != null)
            {
                //if we get a ">", it means we have logged on, send the join
                if (theMsg.Contains("You are in a maze of twisty passages") && state == 0) //if we haven't been connected yet
                {
                    state = 1; //connected
                    Console.WriteLine(":\tGreyB0t: Successfully logged on.");
                    output.Write(
                        "MODE " + handle + " B\r\n" +
                        "JOIN " + channel + "\r\n"
                        );
                    output.Flush();
                }//if we get ":End of ", it means we are in the target channel
                else if (theMsg.Contains("JOIN " + channel) && state == 1)//if we are connected to twitch and haven
                {
                    state = 2;
                    Console.WriteLine(":\tGreyB0t: Entered target channel: " + channel);
                }//pingtest

                if (theMsg.StartsWith("PING "))
                {
                    Console.WriteLine("~Got a ping~");
                    output.Write(theMsg.Replace("PING", "PONG") + "\r\n");
                    output.Flush();
                }
            }
        }

        /*This is the first thing the bot sends after a successful logon
         */
        private void ConnectMsg()
        {
            /*   _,-,
             *  T_  |
             *  ||`-'
             *  ||
             *  ||
             *  ~~
             */
            //must sebd caps request to get JOIN/PART/command info
            if (!isWhisperBot)
            {
                /*
                String preparedText = "PRIVMSG " + channel + " :.me brandishes +3 banhammer\r\n";
                Console.WriteLine(preparedText);
                output.Write(preparedText);
                output.Flush();
                */

                String lastResponse = "CAP REQ :twitch.tv/commands \r\n";
                Console.WriteLine(":\tGreyb0t sent: " + lastResponse);
                output.Write(
                lastResponse
                );
                output.Flush();

                lastResponse = "CAP REQ :twitch.tv/membership \r\n";
                Console.WriteLine(":\tGreyb0t sent: " + lastResponse);
                output.Write(
                lastResponse
                );
                output.Flush();

                lastResponse = ".color cadetblue";
                String preparedText = "PRIVMSG " + channel + " :" + lastResponse + " \r\n";
                output.Write(preparedText);
                output.Flush();
                //www.twitch.tv/warwitchtv/profile
            }
            else
            {
                String lastResponse = "CAP REQ :twitch.tv/commands \r\n";
                Console.WriteLine(":\tGreyb0t sent: " + lastResponse);
                output.Write(
                lastResponse
                );
                output.Flush();
            }
            

            //write outputs on a separate thread(.5 sec timer)
            Thread thread = new Thread(WriteOut);
            thread.Start();//writeout is async
        }

        /*WriteOut occurs on an async thread, it pops from the ready messages
         *q and immediately sends a response. Hopefully the antiflood monitor
         *has been activated or else this could be bad (it has)
         */
        private void WriteOut()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (isWhisperBot)
                {
                    while (MessageProcessor.readyWhispers.Count() > 0)
                    {
                        KeyValuePair<String, String> kvp = MessageProcessor.readyWhispers.Dequeue();
                        String preparedText = "PRIVMSG " + channel + " :.w " + kvp.Value + " " + kvp.Key + " \r\n";
                        Console.WriteLine(preparedText);
                        output.Write(preparedText);
                        output.Flush();
                    }
                }
                else
                {
                    while (MessageProcessor.readyAllSpeaks.Count() > 0)
                    {
                        String response = MessageProcessor.readyAllSpeaks.Dequeue();
                        String preparedText = "PRIVMSG " + channel + " :" + response + " \r\n";
                        output.Write(preparedText);
                        output.Flush();
                    }
                }
            }
            //here be dragons
        }
    }
}
