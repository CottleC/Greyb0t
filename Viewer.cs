using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GreyB0t
{
    [Serializable]
    class Viewer
    {
        public String name;
        public String customRank;
        public double coin;
        public double multiplier = 1.0;
        public static double subMultiplier = 1.5;
        public static double stdMultiplier = 1.0;

        public Viewer()
        {
            name = "null";
            coin = 0;
        }

        public Viewer(String name, double coin)
        {
            this.name = name.ToLower();
            this.coin = coin;
        }

        public Viewer(Viewer v)
        {
            name = v.name.ToLower();
            coin = v.coin;
        }

        public void AdjustMultiplier(double d)
        {
            this.multiplier = d;
        }

        public void SetCustomRank(String s)
        {
            this.customRank = s;
        }

        public String GetCustomRank()
        {
            if (this.customRank != null)
            {
                return this.customRank;
            }
            //else return a 'null'
            return "null";
        }

        public void SetCoin(double given)
        {
            coin = given;
        }

        public void addCoin(double given)
        {
            coin += (given*multiplier);
        }

        public void addCoinUnModified(double given)
        {
            coin += (given);
        }

        public void RemoveCoin(double given)
        {
            if (coin - given >= 0)
            {
                coin -= given;
            }
            else
            {
                coin = 0;
            }
        }

        public int GetCoin()
        {
            return (int)this.coin;
        }

        public bool Equals(Viewer given)
        {
            return (given.name.Equals(this.name));
        }

        public bool Equals(Hunt_Player given)
        {
            return (given.name.Equals(this.name));
        }

        public override int GetHashCode()
        {
            if (name.Equals("null"))
            {
                return 0;
            }
            return name.GetHashCode();
        }

    }
}
