using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    [Serializable]
    class Hunt_Player
    {
        public double huntTime;
        public String name;
        public KeyValuePair<Hunt_Player, uint> vendetta;//last player to strike this player and the amount they struck for
        public DateTime hiddenTime;
        public DateTime lastHuntTime;
        public uint currencies;

        public Hunt_Player()
        {
            this.name = "Unknown!";
            this.currencies = 0;
            this.vendetta = new KeyValuePair<Hunt_Player, uint>();
            this.hiddenTime = DateTime.Now.AddDays(-365);
            this.lastHuntTime = DateTime.Now.AddDays(-365);
            huntTime = 10;
        }

        public Hunt_Player(String name, uint currency)
        {
            this.name = name;
            this.currencies = currency;
            this.vendetta = new KeyValuePair<Hunt_Player, uint>(new Hunt_Player(), 0);
            this.hiddenTime = DateTime.Now.AddDays(-365);
            this.hiddenTime = DateTime.Now.AddDays(-365);
            this.lastHuntTime = DateTime.Now.AddDays(-365);
            huntTime = 10;
        }

        public bool GetIsHidden()//returns if this player is hidden
        {
            return (((TimeSpan)(DateTime.Now - hiddenTime)).Minutes <= Hunt_Game.hideTime);
        }

        public bool SetIsHidden()
        {
            if (!GetIsHidden())
            {
                hiddenTime = DateTime.Now;
                return true;//indicate this player was hidden using NOW as the time
            }
            else
            {
                return false;//indicate this player cannot be hidden atm
            }
        }

        public void StrikePlayer(uint lossAmt, Hunt_Player attacker)
        {
            this.currencies -= lossAmt;
            vendetta = new KeyValuePair<Hunt_Player, uint>(attacker, lossAmt);
        }

        public void SetCurrencies(uint amt, Hunt_Player attacker)
        {
            this.currencies = amt;
        }

        public bool Equals(Hunt_Player given)
        {
            return (given.name.Equals(this.name));
        }

        public void SetLastHuntTime(DateTime d)
        {
            lastHuntTime = d;
        }

        public DateTime GetLastHuntTime()
        {
            return lastHuntTime;
        }

        public String GetCooldown()
        {
            TimeSpan remaining = lastHuntTime - DateTime.Now;

            if (remaining.TotalMinutes > 1)
            {
                return remaining.TotalMinutes.ToString("00");
            }
            else
            {
                return null;
            }
        }
    }
}
