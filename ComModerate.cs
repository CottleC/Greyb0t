using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GreyB0t
{
    class ComModerate : Command
    {
        public ComModerate()
        {
            theMsg = null;
            invocation = new Dictionary<String, String>();
            //mod commands
            invocation.Add("!timeout", "gives a user a timeout");
            invocation.Add("!ban", "gives a user a banslap[permanant]");
            invocation.Add("!unban", "unpermanants a ban");
            invocation.Add("!slow", "sets chatter frequency");
            invocation.Add("!slowoff", "offs chatter frequency limiter");
            invocation.Add("!clear", "clears chat history");
            //editor commands
            invocation.Add("!commercial", "clears chat history");
            invocation.Add("!host", "hosts a stream");
            invocation.Add("!unhost", "unhosts a stream");
        }

        public override void ParseCommand(Message m)
        {
                String returnMsg = "NULL";
                string[] args = m.tell.ToString().Split(' ');
                int numArgs = args.Count();
                if (numArgs > 0)
                {
                    if (invocation.Keys.Contains(args[0]))
                    {
                        switch (args[0])
                        {
                            case "!timeout":
                                if (CommandProcessor.superUsers.Contains(m.username) || CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    if (numArgs == 2)//default timeout
                                    {
                                        returnMsg = ".timeout " + args[1];
                                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                        String details = "pssst! I passed your command like this: '" + returnMsg + "'";
                                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(details, m.username);
                                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                    }
                                    else if (numArgs == 3)
                                    {
                                        returnMsg = ".timeout " + args[1] + " " + args[2];
                                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                        String details = "pssst! I passed your command like this: '" + returnMsg + "'";
                                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(details, m.username);
                                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                    }
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }

                                break;
                            case "!ban":
                                if (CommandProcessor.superUsers.Contains(m.username) || CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    if (numArgs > 1)
                                    {
                                        returnMsg = ".ban " + args[1];
                                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                    }
                                    else
                                    {
                                        String details = "Pstt... your ban didn't have the correct syntax!";
                                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(details, m.username);
                                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                    }
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }
                                break;
                            case "!unban":
                                if (CommandProcessor.superUsers.Contains(m.username) || CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    if (numArgs > 1)
                                    {
                                        returnMsg = "/unban " + args[1];
                                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                    }
                                    else
                                    {
                                        String details = "Pstt... your unban didn't have the correct syntax!";
                                        KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(details, m.username);
                                        MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
                                    }
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }
                                break;
                            case "!slow":
                                break;
                            case "!slowoff":
                                break;
                            case "!clear":
                                if (CommandProcessor.superUsers.Contains(m.username) || CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    returnMsg = ".clear";
                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }
                                break;
                            case "!commercial":
                                break;
                            case "!host":
                                if (CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    if (numArgs > 1)
                                    {
                                        returnMsg = ".host " + args[1];
                                        MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                    }
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }
                                break;
                            case "!unhost":
                                if (CommandProcessor.megaUsers.Contains(m.username))
                                {
                                    returnMsg = ".unhost ";
                                    MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
                                }
                                else
                                {
                                    DenyAccess(m.username);
                                }
                                break;
                        }
                    }
                }
        }

        public void DenyAccess(String user)
        {
            String details = "The gears groan and the ground rumbles, but you cannot summon the strength needed to use that command.";
            KeyValuePair<String, String> whisperKvp = new KeyValuePair<string, string>(details, user);
            MessageProcessor.pendingWhispers.Enqueue(whisperKvp);
        }

        public override void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: !moderate " + s);
        }

        public override void CleanUp()
        {
            //undefined for !moderate
        }
    }
}

