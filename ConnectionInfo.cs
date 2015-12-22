using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    class ConnectionInfo
    {
        public int port;
        public String serverName;
        public String chan;
        public String owner;
        public String botName;
        public String oAuth;
        public bool isWhisperBot;

        public ConnectionInfo()
        {
            //Hi! I don't do anything!
        }

        public ConnectionInfo(int port, String serverName, String chan, String owner, String botName, String oAuth, bool isWhisperBot)
        {
            this.port = port;
            this.serverName = serverName;
            this.chan = chan;
            this.owner = owner;
            this.botName = botName;
            this.oAuth = oAuth;
            this.isWhisperBot = isWhisperBot;
        }

        public ConnectionInfo(ConnectionInfo c)
        {
            this.port = c.port;
            this.serverName = c.serverName;
            this.chan = c.chan;
            this.owner = c.owner;
            this.botName = c.botName;
            this.oAuth = c.oAuth;
            this.isWhisperBot = c.isWhisperBot;
        }
    }
}
