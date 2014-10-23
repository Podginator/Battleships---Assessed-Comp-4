using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Battleship
{
    public class Grid
    {
        public TileProperty[,] GridLayout { get; set; }
        public Dictionary<ShipType, Ship> ShipList { get; set; }

        public Grid()
        {
            GridLayout = new TileProperty[10, 10];
            ShipList = new Dictionary<ShipType, Ship>()
            {  
                {ShipType.Aircraft, new Ship("Aircraft Carrier", 5)},
                {ShipType.Battleship, new Ship("Battleship", 4)}, 
                {ShipType.Submarine, new Ship("Submarine", 3)},
                {ShipType.Destroyer, new Ship("Destroyer", 3)},
                {ShipType.Patrol, new Ship("Patrol Boat", 2)}
            };
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int b = 0; b < 10; b++)
                {
                    GridLayout[i, b] = new TileProperty();
                }
            }
        }

        public bool PlaceShips(Ship ship, int letter, int number, Rotation rotation)
        {
            try
            {
                if (rotation == Rotation.Down)
                {
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        if (GridLayout[number + i, letter].HasShip != null)
                        {
                            Console.WriteLine("This Overlaps another ship, please try again");
                            return false;
                        }
                    }
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        GridLayout[number + i, letter].HasShip = ship;
                    }
                    return true;
                }
                else if (rotation == Rotation.Right)
                {
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        if (GridLayout[number, letter+i].HasShip != null)
                        {
                            Console.WriteLine("This Overlaps another Value, please try again");
                            return false;
                        }
                    }
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        GridLayout[number, letter + i].HasShip = ship;
                    }
                    return true;
                }
                else if (rotation == Rotation.Up)
                {
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        if (GridLayout[number - i, letter].HasShip != null)
                        {
                            Console.WriteLine("This Overlaps another ship, please try again");
                            return false;
                        }
                    }
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        GridLayout[number - i, letter].HasShip = ship;
                    }
                    return true;
                }
                else if (rotation == Rotation.Left)
                {
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        if (GridLayout[number, letter - i].HasShip != null)
                        {
                            Console.WriteLine("This Overlaps another Value, please try again");
                            return false;
                        }
                    }
                    for (int i = 0; i < ship.Width; ++i)
                    {
                        GridLayout[number, letter - i].HasShip = ship;
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Incorrect Value");
                    return false;
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("That's an invalid selection. Try Again");
                return false;
            }
        }

        public bool AttackGrid(Battleship battleship, int number, int letter)
        {

            Console.Clear();
            Ship hitShip = null;
            Player player = battleship.CurrentPlayer;

            try
            {
                if (!GridLayout[number, letter].HasChecked)
                {
                    if (GridLayout[number, letter].HasShip != null)
                    {
                        hitShip = GridLayout[number, letter].HasShip.Hit();

                        GridLayout[number, letter].HasChecked = true;
                        Console.WriteLine("You hit a ship!");
                        return true;
                    }
                    else
                    {
                        GridLayout[number, letter].HasChecked = true;
                        //Changes the current player if the move hits (Will stay the same if it misses or has invalid input)
                        battleship.CurrentPlayer = battleship.CurrentPlayer == battleship.Player1 ? battleship.Player2 : battleship.Player1;
                        Console.WriteLine("You missed!");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Move already made. Try again");
                    return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Invalid Selection. Try again.");
                return false;
            }
        }
        
        public void DisplayGrid(bool displayFull=false)
        {
            int breakup = 0;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  A  B  C  D  E  F  G  H  I  J  ");
            foreach (TileProperty tile in this.GridLayout)
            {
                if (breakup % 10 == 0)
                {
                    if (breakup != 0)
                    {
                        Console.Write("\n");
                        Console.Write("\n");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(breakup / 10);
                }

                if (displayFull && tile.HasShip != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(" o ");
                }
                else if (!tile.HasChecked)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(" x ");
                }
                else if (tile.SuccessfulCheck)
                {
                    if (tile.HasShip.Destroyed)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.Write(" o ");
                }
                else if (tile.HasChecked && tile.HasShip == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" x ");
                }
                breakup++;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n\n");
        }
    }
}
