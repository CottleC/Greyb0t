using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace GreyB0t
{
    class PeriodicMessage
    {
        public String tell = "";//what they said
        public int repeatTime = 5; //how often this should repeat
        public int repeatOrigin = 0; //what minute on the hour to start on
        public bool startImmediately = false;
        protected Thread thr;
        private bool breaker = false;
        public String x = "_01";
        int numRuns = 0;

        public PeriodicMessage()
        {
            this.tell = "";
            this.repeatTime = 60;
            this.repeatOrigin = 30;
            this.startImmediately = false;
        }

        public PeriodicMessage(String msg, int repeatTime, int repeatOrigin, bool startImmediately )
        {
            this.tell = msg;
            this.repeatTime = repeatTime;
            this.repeatOrigin = repeatOrigin;
            this.startImmediately = startImmediately;
            thr = new Thread(ThreadLoop);
        }

        public PeriodicMessage(PeriodicMessage p)
        {
            this.tell = p.tell;
            this.repeatTime = p.repeatTime;
            this.repeatOrigin = p.repeatOrigin;
            this.startImmediately = p.startImmediately;
            thr = new Thread(ThreadLoop);
        }

        public void Start()
        {
            thr.Start();
        }

        private void ThreadLoop()
        {
            String temp = repeatOrigin.ToString("00");
            if(!startImmediately)
            {
                while(!DateTime.Now.Minute.ToString("00").Equals(temp))//while it is not the start time
                {
                    Thread.Sleep(1000);
                }

            }
            String msg = "/me says: " + tell;
            MessageProcessor.pendingAllspeaks.Enqueue(msg);

            while ((breaker != true))
            {
                Thread.Sleep(repeatTime * 1000);
                RecurringThing();
            }
        }

        public void RecurringThing()
        {
            String returnMsg = "/me says: "+tell;
            MessageProcessor.pendingAllspeaks.Enqueue(returnMsg);
            numRuns++;
        }

        public void CleanUp()
        {
            //defined for !this class
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Console.WriteLine("Writing periodicmessage to " + CommandProcessor.path + "periodicMsg"+x+".json");
            File.WriteAllText(CommandProcessor.path + "periodicMsg" + x + ".json", jsonString);
        }

    }
}

