using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Player 
    {
        public Player(string name)
        {
            this.Name = name;
            this.TotalWins = 0;
            this.TotalLosses = 0;
        }
        public string Name{ get; set; }

        public int TotalWins{ get; set; }

        public int TotalLosses { get; set;}

        public double WinLossRatio
        {
            get
            {
                try
                {
                    return TotalWins / TotalLosses;
                }
                catch (DivideByZeroException)
                {
                    return 1.0;
                }
            }
        }

        public virtual string SelectMove()
        {
            Console.WriteLine("Enter a Valid Move (A0-J9) or Export to txt:");
            return Console.ReadLine();
        }

        public virtual Tuple<string, Rotation> SetShips(Ship ship, out bool display)
        {
            display = true;
            string placement;
            string direction;
            Rotation shipRotation;

            Console.WriteLine("Place your Battleships");
            Console.Write("Place the {0} ({1} Wide) at which co-ordinates? ", ship.Name, ship.Width);
            placement = Console.ReadLine();

            Console.Write("And what direction would you like to place it (Up/Down/Left/Right)");
            direction = Console.ReadLine();

            switch (direction.ToLower())
            {
                case("down"):
                    shipRotation = Rotation.Down;
                    break;
                case("up"):
                    shipRotation = Rotation.Up;
                    break;
                case("left"):
                    shipRotation = Rotation.Left;
                    break;
                case("right"):
                    shipRotation = Rotation.Right;
                    break;
                default:
                    shipRotation = Rotation.None;
                    break;
            }

            return new Tuple<string, Rotation>(placement, shipRotation);
        }

    }
}
