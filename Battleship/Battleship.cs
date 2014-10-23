using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{

    public enum Rotation { Up, Down, Left, Right, None }
    public enum ShipType { None = 0, Aircraft, Battleship, Submarine, Destroyer, Patrol }
    public enum CheckType { None, Miss, Hit, Destroyed }

    public class Battleship
    {
        Menu game;
        Logging log;

        public Battleship(Player player1, Player player2, string file=null)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.CurrentPlayer = player1;
            this.PlayerGrids = new Dictionary<Player, Grid>()
            {
                {Player1, new Grid()},
                {Player2, new Grid()}
            };
            this.log = new Logging(this);
            this.NewGame(file);
        }

        public Dictionary<Player, Grid> PlayerGrids { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Player CurrentPlayer { get; set; }
        public Player Opponent
        {
            get { return CurrentPlayer == Player1 ? Player2 : Player1; }
        }

        public bool GameOver()
        {
            foreach (Ship ship in PlayerGrids[Opponent].ShipList.Values)
            {
                if (!ship.Destroyed)
                {
                    return false;
                }
            }
            CurrentPlayer.TotalWins++;
            Opponent.TotalLosses++;

            Console.Clear();
            Console.WriteLine("=={0} is the Winner!==", CurrentPlayer.Name);
            Console.WriteLine("Total wins: {0}\nTotal Losses: {1}\nWin/Loss Ratio: {2}", CurrentPlayer.TotalWins, CurrentPlayer.TotalLosses, CurrentPlayer.WinLossRatio);

            Console.WriteLine("Play Again? (Yes/No)");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "yes")
            {
                NewGame();
            }

            return true;
        }

        //Tests if [0] is between A-J and [1] is between 0-9
        public bool ValidInput(string test)
        {
            try
            {
                if (test.Length >= 3 && test[2] != 'p')
                {
                    throw new IndexOutOfRangeException();
                }

                if (test.ToLower().Split(' ')[0] == "export")
                {
                    log.ExportGame(test.Split(' ')[1]);
                    return false;
                }
                else if (test.ToUpper()[0] < 'A' || test.ToUpper()[0] > 'J')
                {
                    Console.WriteLine("Invalid");
                    return false;
                }
                else if (test.ToUpper()[1] < '0' || test.ToUpper()[1] > '9')
                {
                    Console.WriteLine("Invalid Entry");
                    return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Entry Invalid.");
                return false;
            }
            return true;
        }

        public bool PlayMove(string move)
        {
            int letter = (char)move.ToUpper()[0] - 'A';
            int number = (char)move[1] - '0';

            bool successful = PlayerGrids[Opponent].AttackGrid(this, number, letter);
            //log.LogMoves(move.ToUpper(), successful);
            return successful;
        }

        private void SetShip(Player player)
        {
            foreach (var ship in PlayerGrids[player].ShipList)
            {
                Tuple<string, Rotation> result; 
                int letterCoordinate;
                int numberCoordinate;
                bool viewAll;

                do
                {
                    do
                    {
                        result = player.SetShips(ship.Value, out viewAll);
                    } while (!ValidInput(result.Item1)); 
                    try
                    {
                        letterCoordinate = (char)result.Item1.ToUpper()[0] - 'A';
                        numberCoordinate = (char)result.Item1[1] - '0';
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Entry invalid.");
                        return;
                    }
                } while (!PlayerGrids[player].PlaceShips(ship.Value, letterCoordinate, numberCoordinate, result.Item2));

                Console.Clear();
                //var viewAll here is used to prevent the AI from flashing where 
                //it's ships are.
                PlayerGrids[player].DisplayGrid(viewAll);
            }

            Console.Clear();
        }

        public void NewGame(string import=null)
        {
            Console.Clear();
            //This is generated here because if you decide to reset the game it has to regenerate the board.
            if (import != null)
            {
                 log.Import(import);
            }
            else
            {
                PlayerGrids[Player1] = new Grid();
                PlayerGrids[Player1].DisplayGrid();
                SetShip(Player1);

                PlayerGrids[Player2] = new Grid();
                PlayerGrids[Player2].DisplayGrid();
                SetShip(Player2);
            }

            Play();
        }

        //Entire game (Both Single And Multiplayer) runs from here.
        public void Play(string file = null)
        {
            string input;
            while (!GameOver())
            {                
                Console.WriteLine("{0}'s Turn", CurrentPlayer.Name);
                PlayerGrids[Opponent].DisplayGrid();
                do
                {
                    //Will pass a basic grid if it's an AI.
                    if (CurrentPlayer.GetType() == typeof(AI))
                    {
                        ConvertGrid();
                    }
                    input = CurrentPlayer.SelectMove();
                } while (!ValidInput(input));
                PlayMove(input);
            }
            return;
        }

        /*Players are designed to NEVER take an full instance of
          a grid. This converts a grid to the bare minimum, never
          exposing information about ships
          This is passed to the AI for it to decipher. 
          It also sends a list of Hits to the AI for selectMove */ 
        private void ConvertGrid()
        {
            List<Tuple<int, int>> successfulHit = new List<Tuple<int,int>>();

            TileProperty[,] board = PlayerGrids[Opponent].GridLayout;
            for (int y = 0; y < board.GetLength(0); ++y)
            {
                for (int x = 0; x < board.GetLength(1); ++x)
                {
                    if (!board[x, y].HasChecked)
                    {
                        ((AI)CurrentPlayer).BoardCopy[x, y] = CheckType.None;
                    }
                    else if (!board[x, y].SuccessfulCheck)
                    {
                        ((AI)CurrentPlayer).BoardCopy[x, y] = CheckType.Miss;
                    }
                    else if (board[x, y].HasShip.Destroyed)
                    {
                        ((AI)CurrentPlayer).BoardCopy[x, y] = CheckType.Destroyed;
                    }
                    else
                    {
                        ((AI)CurrentPlayer).BoardCopy[x, y] = CheckType.Hit;
                        successfulHit.Add(Tuple.Create<int, int>(x, y));
                    }
                }
            }
            ((AI)CurrentPlayer).SuccessfulHits = successfulHit;
        }
    }


}
