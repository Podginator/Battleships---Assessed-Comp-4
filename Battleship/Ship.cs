using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Ship
    {
        int timesHit;

        public Ship(string name, int width)
        {
            this.Name = name;
            this.Width = width;
            this.timesHit = 0;
        }

        public int Width { get; private set; }

        public string Name { get; private set; }

        public bool Destroyed 
        {
            get
            {
                return timesHit >= Width;
            }
        }

        public Ship Hit()
        {
            ++timesHit;
            if(timesHit == this.Width)
            {
                Console.WriteLine("You destroyed the {0}", this.Name);
            }
            return this;
        }
    }
}
