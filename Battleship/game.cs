using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Game
    {
        static void Main(string[] args)
        {
            Console.WindowHeight += 2;
            Menu game = new Menu();
            while (true)
            {
                Console.Clear();
                game.StartMenu();
            }
        }
    }
}
