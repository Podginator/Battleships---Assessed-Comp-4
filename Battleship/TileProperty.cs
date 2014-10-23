using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class TileProperty
    {
        public TileProperty()
        {
            this.HasShip = null;
            this.HasChecked = false;
        }

        public Ship HasShip { get; set; }

        public bool HasChecked { get; set; }

        public bool SuccessfulCheck
        {
            get { return HasShip!=null && HasChecked; }
        }
    }
}
