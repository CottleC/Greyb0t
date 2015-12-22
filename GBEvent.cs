using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    class GBEvent
    {
        //events DO stuff on time intervals
        //Typically there is a 
        //kickoff message: vote on [stuff] has started!
        //recurring message: vote has x seconds left!
        //end message: vote has finished! the winnder was god!

        protected Thread thread;
        protected int triggerSeconds;
        protected int runTime;
        protected bool breaker;//flag to stop the thread

        public GBEvent()
        {
            thread = new Thread(ThreadLoop);
            triggerSeconds = -1;
            runTime = -1;
        }

        public void Start()
        {
            thread.Start();
        }

        private void ThreadLoop()
        {
            while ((breaker != true))
            {
                Thread.Sleep(triggerSeconds * 1000);
                RecurringThing();
            }
        }

        public virtual void RecurringThing()
        {
            Console.WriteLine("An event recurred");
        }

        //do any writing to files here
        public virtual void CleanUp()
        {

        }
    }
}

