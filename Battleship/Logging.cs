using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Battleship
{
    public class Logging
    {

        Battleship battleship;
        string dir = AppDomain.CurrentDomain.BaseDirectory;

        public Logging(Battleship battleship)
        {
            this.battleship = battleship;
        }

        public void LogMoves(string move, bool hit)
        {
            string path = dir+"/Logs/" + battleship.Player1.Name + "VS" + battleship.Player2.Name + ".txt";
            string successfulHit = hit ? "Hit" : "Miss";
            using (StreamWriter write = new StreamWriter(path, true))
            {
                write.WriteLine("{0} {1} {2}", battleship.CurrentPlayer.Name, move, successfulHit);
                write.Close();
            }
        }

        //Imports based on displacement from o or x
        public void Import(string file)
        {
            string line;
            int lineOn = 0;
            Ship ship;

            using (StreamReader read = new StreamReader(file))
            {
				Grid grid = battleship.PlayerGrids[battleship.Player1];
                while ((line = read.ReadLine()) != null)
                {
                    int keyIn = 0;
                    foreach (char key in line)
                    {
                        if (key <= '\r' || lineOn>20)
                        {
                            break;
                        }

						bool hasChecked = key <= 'o';
						grid.GridLayout[lineOn%10, keyIn%10].HasChecked = hasChecked;

						ShipType type = (ShipType)((key > 'o' ? 'x' : 'o') - key);
						if (type != ShipType.None)
						{
							ship = grid.ShipList[type];
                            if(hasChecked)
                            {
                                ship.Hit();
                            }
						}
						else
						{
							ship = null;
						}
                        grid.GridLayout[lineOn % 10, keyIn % 10].HasShip = ship;
                        ++keyIn;
                    }
                    ++lineOn;
                    
					if (lineOn == 10)
					{
						grid = battleship.PlayerGrids[battleship.Player2];
					}


                }
            }
        }

        //If the tyle has been hit it will be o, otherwise x. 
        //then, if there's a ship reference in that tile it will displace that character by a certain amount.
        public void ExportGame(string file)
        {
            string path = dir+"/Games/" + file + ".txt";
            StreamWriter write = new StreamWriter(path, true);
            string line = "";
            char shipType;

			foreach (Player player in new List<Player> {battleship.Player1, battleship.Player2})
			{
				TileProperty[,] grid = battleship.PlayerGrids[player].GridLayout;
				for (int x = 0; x < grid.GetLength(0); ++x)
				{
					for (int y = 0; y < grid.GetLength(1); ++y)
					{
						TileProperty cell = grid[x, y];
                        shipType = (char)0;
                        if (cell.HasShip != null)
                        {
                            shipType = (char)(battleship.PlayerGrids[player].ShipList.First(ship => ship.Value == cell.HasShip).Key);
                        }
                        line += (char)((cell.HasChecked ? 'o' : 'x') - shipType);
					}
					line += "\r\n";
				}
            }

            write.WriteLine(line);
            write.Write("player1:[Name:{0},TotalWins:{1},TotalLosses:{2},Type:{3}]", battleship.Player1.Name, battleship.Player1.TotalWins, battleship.Player1.TotalLosses, battleship.Player1.GetType() == typeof(AI) ? "Computer" : "Human");
            write.Write("player2:[Name:{0},TotalWins:{1},TotalLosses:{2},Type:{3}]",battleship.Player2.Name, battleship.Player2.TotalWins, battleship.Player2.TotalLosses, battleship.Player2.GetType() == typeof(AI) ? "Computer" : "Human");
            write.Write("currentPlayer:[Name:{0}]",battleship.Player1.Name);
            write.Close();
            Console.WriteLine("Export to {0} Successful!", path);
        }
    }
}
