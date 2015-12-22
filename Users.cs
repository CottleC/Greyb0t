using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    class Users
    {
        public List<String> superUsers; //these are never populated when loading in from a file
        public List<String> megaUsers; //the data goes directly to the command processor

        public Users()
        {
            superUsers = new List<String>();
            megaUsers = new List<String>();
        }

        public Users(Users u)
        {

            //add the new users
            for(int i = 0; i<u.superUsers.Count(); i++)
            {
                if (!CommandProcessor.superUsers.Contains(u.superUsers.ElementAt(i)))
                {
                    CommandProcessor.superUsers.Add(u.superUsers.ElementAt(i));
                }
            }

            for (int i = 0; i < u.megaUsers.Count(); i++)
            {
                if (!CommandProcessor.megaUsers.Contains(u.megaUsers.ElementAt(i)))
                {
                    CommandProcessor.megaUsers.Add(u.megaUsers.ElementAt(i));
                }
            }

            //remove old users
            for (int i = 0; i < CommandProcessor.superUsers.Count(); i++)
            {
                if (!u.superUsers.Contains(CommandProcessor.superUsers.ElementAt(i)))
                {
                    CommandProcessor.superUsers.RemoveAt(i);
                }
            }

            for (int i = 0; i < CommandProcessor.megaUsers.Count(); i++)
            {
                if (!u.megaUsers.Contains(CommandProcessor.megaUsers.ElementAt(i)))
                {
                    CommandProcessor.megaUsers.RemoveAt(i);
                }
            }
        }
    }
}
