using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyB0t
{
    abstract class Command
    {
        //list of valid invokations for this command
        //invokation, helptxt
        public Dictionary<String, String> invocation;
        public Message theMsg;

        public Command()
        {

        }

        //this pushes the command to the MessageProcessor when done
        public virtual void ParseCommand(Message m)
        {
            Console.WriteLine(":\tGreyb0t: Parsed Command as a base commmand");
        }

        public virtual void Print(String s)
        {
            Console.WriteLine(":\tGreyb0t: Command "+s);
        }

        public virtual void CleanUp()
        {

        }
    }
}

