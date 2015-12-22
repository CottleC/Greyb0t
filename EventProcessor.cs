using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GreyB0t
{
    /* EventProcessor keeps a list of all events
     * when an event is passed to EventProcessor, 
     * it starts the event thread and adds
     * the event to the active list
     * 
     */
    static class EventProcessor
    {
        public static HashSet<GBEvent> activeEvents = new HashSet<GBEvent>();

        public static void Init()
        {
            //activeEvents = new HashSet<GBEvent>();
        }

        public static void AddGBEvent(GBEvent e)
        {
            if (!activeEvents.Contains(e))
            {
                activeEvents.Add(e);
                e.Start();//start thread associated with this event
            }
        }

        public static void RemoveGBEvent(GBEvent e)
        {
            if(activeEvents.Contains(e))
            {
                activeEvents.Remove(e);
            }
        }
    }
}
