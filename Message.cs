using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyB0t
{
    class Message
    {
        //:atherma!atherma@atherma.tmi.twitch.tv PRIVMSG #tuesdaygrey :This doctor...
        public String username;//who said it
        public String target;//channel can be twitch or whisper i guess....
        public String tell;//what they said
        public bool isAMessage;
        public bool isCAPS;
        public bool isAWhisper;//for when whisper support is added

        public Message(String rawInput)
        {
            Decipher(rawInput);
        }

        public Message(Message copyMe)
        {
            if (copyMe.isAMessage)
            {
                    this.username = copyMe.username;
                    this.target = copyMe.target;
                    this.tell = copyMe.tell;
                    this.isAMessage = copyMe.isAMessage;
                    this.isAWhisper = copyMe.isAWhisper;
                    this.isCAPS = copyMe.isCAPS;
            }
        }

        public void Decipher(string codex)
        {
            if(codex==null)
            {
                return;
            }

            int startLoc = 0;
            int endLoc = 0;
            startLoc = codex.IndexOf(':');
            endLoc = codex.IndexOf('!');

            if(startLoc != 0)
            {
                //we got a server message
                this.isAMessage = false;
                return;
            }

            //get the username
            if ((startLoc < endLoc) && (endLoc > 0))
            {
                username = codex.Substring(startLoc + 1, endLoc - 1).ToLower();
                //get the target
                startLoc = 0;
                endLoc = 0;

                //if this has a privmsg
                if (codex.IndexOf("PRIVMSG") > 0)
                {
                    codex = codex.Remove(0, codex.IndexOf("PRIVMSG"));
                    startLoc = codex.IndexOf("#") + 1;
                    endLoc = codex.IndexOf(':') - 1;
                    if ((startLoc >= 0) && (endLoc > 0))
                    {
                        target = codex.Substring(startLoc, endLoc - startLoc);
                    }
                    else
                    {
                        isAMessage = false;
                    }

                    startLoc = 0;
                    endLoc = 0;

                    startLoc = codex.IndexOf(':')+1;

                    if (startLoc > 0)
                    {
                        if(/*this is a whisper*/false)
                        {
                            tell = codex.Substring(startLoc, codex.Length - startLoc);
                            isAMessage = true;
                            isAWhisper = true;
                        }
                        else 
                        {
                            tell = codex.Substring(startLoc, codex.Length - startLoc);
                            isAMessage = true;
                            isAWhisper = false;
                        }
                        
                    }
                    else
                    {
                        isAMessage = false;
                    }
                }
                else if (codex.IndexOf("WHISPER") > 0)
                {
                    codex = codex.Remove(0, codex.IndexOf("WHISPER"));
                    startLoc = codex.IndexOf(Bot.handle) + 1;
                    endLoc = codex.IndexOf(':') - 1;
                    if ((startLoc >= 0) && (endLoc > 0))
                    {
                        target = codex.Substring(startLoc, endLoc - startLoc);
                        this.isAWhisper = true;
                    }
                    else
                    {
                        isAMessage = false;
                    }

                    startLoc = 0;
                    endLoc = 0;

                    startLoc = codex.IndexOf(':') + 1;

                    if (startLoc > 0)
                    {
                        tell = codex.Substring(startLoc, codex.Length - startLoc);
                        isAMessage = true;
                    }
                    else
                    {
                        isAMessage = false;
                    }
                }
                else if (codex.IndexOf("JOIN") > 0)
                {
                    this.isCAPS = true;
                    this.tell = "JOIN";
                    //Console.WriteLine("\tJOIN detected");
                }
                else if (codex.IndexOf("PART") > 0)
                {
                    this.isCAPS = true;
                    this.tell = "PART";
                    //Console.WriteLine("\tPART detected");
                }
                
            }
            else
            {
                isAMessage = false;
            }
        }
    }
}

