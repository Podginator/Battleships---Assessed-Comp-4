using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Battleship
{
    public class Menu 
    {
        //This is currently Fine.
        Battleship battleShip;
        Player player1;
        Player player2;

        private bool SelectOption(ConsoleKeyInfo start, out ConsoleKey choice)
        {
			choice = start.Key;
            switch (start.Key)
            {
                case ConsoleKey.S:
                    return true;
                case ConsoleKey.M:
                    return true;
                case ConsoleKey.I:
                    return true;
                default:
                    return false;
            }
        }

        public void StartMenu()
        {
            Console.WriteLine("==============================BATTLESHIPS==============================\n\n");
            Console.WriteLine("                             # #  ( )");
            Console.WriteLine("                                  ___#_#___|__");
            Console.WriteLine("                              _  |____________|  _");
            Console.WriteLine("                       _=====| | |            | | |==== _");
            Console.WriteLine("                 =====| |.---------------------------. | |====");
            Console.WriteLine("   <--------------------'   .  .  .  .  .  .  .  .   '--------------/");
            Console.WriteLine("     \\                                                             /");
            Console.WriteLine("      \\_______________________________________________WWS_________/");
            Console.WriteLine("  wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            Console.WriteLine("  wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            Console.WriteLine("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            Console.WriteLine("  wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
            Console.WriteLine("ASCII Ship from http://ascii.co.uk/art/battleship\n\n");
            Console.WriteLine("(S)ingle Player");
            Console.WriteLine("(M)ultiplayer");
            Console.WriteLine("(I)mport game");
            Console.WriteLine();
            Console.WriteLine(String.Format("© Thomas Rogers 1982."));


			ConsoleKey choice = ConsoleKey.S;
			while (!SelectOption(Console.ReadKey(), out choice)) 
            {
				Console.WriteLine("Invalid selection, please choose again.");
			}

            Console.Clear();
            string file = null;
            string name; 

			if(choice == ConsoleKey.I)
			{
				file = ImportMenu();
			}
			else
			{
				Console.WriteLine("What is Player 1's name?");
                name = Console.ReadLine();
                name = name != "" ? name : "Player 1";
				player1 = new Player(name);

				if(choice == ConsoleKey.S)
				{
					player2 = new AI();
				}
				else if(choice == ConsoleKey.M)
				{
					Console.WriteLine("What is Player 2's name?");
                    name = Console.ReadLine();
                    name = name != "" ? name : "Player 2";
					player2 = new Player(Console.ReadLine());
				}
			}

			battleShip = new Battleship(player1, player2, file);
        }

        private string ImportMenu()
        {
            Console.Clear();
            StreamReader read;
            string filename;


            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"/Games/");
            Console.WriteLine("Current Files:");
            foreach (FileInfo file in directory.GetFiles("*.txt"))
            {
                Console.WriteLine(file.Name.Replace(".txt", ""));
            }

            Console.Write("Select File: ");
            filename = AppDomain.CurrentDomain.BaseDirectory + "/Games/" + Console.ReadLine() + ".txt";

            while (!File.Exists(filename))
            {
                Console.WriteLine("File doesn't exist! Reselect:");
                filename = AppDomain.CurrentDomain.BaseDirectory + "/Games/" + Console.ReadLine() + ".txt";
            }

            read = new StreamReader(filename);
            string fileContent = read.ReadToEnd();

            //Finds player information to import to current game.
            MatchCollection playerInfo = Regex.Matches(fileContent, @"player(?<playerNumber>\d):\[Name:(?<Name>.*?),TotalWins:(?<TotalWins>\d+),TotalLosses:(?<TotalLosses>\d+),Type:(?<Type>.*?)\]");

            foreach (Match match in playerInfo)
            {
                Player p = match.Groups["Type"].ToString() == "Human" ? new Player(match.Groups["Name"].ToString()) : new AI();
                p.TotalWins = int.Parse(match.Groups["TotalWins"].ToString());
                p.TotalLosses = int.Parse(match.Groups["TotalLosses"].ToString());

                if (match.Groups["playerNumber"].ToString() == "1")
                {
                    this.player1 = p;
                }
                else if (match.Groups["playerNumber"].ToString() == "2")
                {
                    this.player2 = p;
                }
            }

            read.Close();            
            return filename;
        }

    }

}
