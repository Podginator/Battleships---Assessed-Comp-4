using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class AI : Player
    {
        public CheckType[,] BoardCopy { get; set; }
        public List<Tuple<int, int>> SuccessfulHits { get; set; }
        Random random;

        public AI() : base("Computer")
        {
            this.random = new Random();
            this.BoardCopy = new CheckType[10, 10];
        }

        public override Tuple<string, Rotation> SetShips(Ship ship, out bool display)
        {
            display = false;
            char letterCoordinate;
            char numberCoordinate;
            Rotation shipRotation;

            letterCoordinate = (char) ('A' + random.Next(0, 10));
            numberCoordinate = (char)('0' + random.Next(0, 10));
            shipRotation = random.Next(1, 3) == 1 ? Rotation.Right : Rotation.Down;

            return new Tuple<string, Rotation>((letterCoordinate + numberCoordinate.ToString()), shipRotation);
        }

        public override string SelectMove()
        { 
            //Iterates over  the List of previously successful moves 
            //This is done to that it iterates over the minimum number of board positions
            //for efficiency.
            if (SuccessfulHits.Count != 0)
            {
                foreach (Tuple<int, int> coordinates in SuccessfulHits)
                {
                    int x = coordinates.Item1;
                    int y = coordinates.Item2;

                    //First section trys to establish a pattern in the code by doing
                    //A local search around the hits
                    if (x != 0 && BoardCopy[x - 1, y] == CheckType.Hit)
                    {
                        if (x != 9 && BoardCopy[x + 1, y] == CheckType.None)
                        {
                            return DisplayMove((char)(y + 'A') + ((char)(x + 1 + '0')).ToString());
                        }
                    }

                    if (x != 9 && BoardCopy[x + 1, y] == CheckType.Hit)
                    {
                        if (x != 0 && BoardCopy[x - 1, y] == CheckType.None)
                        {
                            return DisplayMove((char)(y + 'A') + ((char)(x - 1 + '0')).ToString());
                        }
                    }

                    if (y != 0 && BoardCopy[x, y - 1] == CheckType.Hit)
                    {
                        if (y != 9 && BoardCopy[x, y + 1] == CheckType.None)
                        {
                            return DisplayMove((char)(y + 1 + 'A') + ((char)(x + '0')).ToString());
                        }
                    }

                    if (y != 9 && BoardCopy[x, y + 1] == CheckType.Hit)
                    {
                        if (y != 0 && BoardCopy[x, y - 1] == CheckType.None)
                        {
                            return DisplayMove((char)(y - 1 + 'A') + ((char)(x + '0')).ToString());
                        }
                    }

                    //If there's no pattern continue
                    //but only if there's a hit nearby
                    if (SuccessfulSkip(x, y))
                    {
                        continue;
                    }

                    //Checks to see if it's surrounded on all sides(making it impossible to chose a move)
                    if (IsSurrounded(x, y))
                    {
                        continue;
                    }

                    //Otherwise choose a random direction to go in.
                    return CorrectGuess(x, y);
                }
            }
            //If none of this is successful then just return a random move.
            return DisplayMove(SelectRandom());
        }

        /*Iterates to the end of a sequence to check if 
         *the end cells aren't .Miss.
         *If they aren't then there's two ships next to each other */
        private bool HasCompletedEnd(int x, int y)
        {
            bool verticalRight = false;
            bool verticalLeft = false;
            bool horizontalUp = false;
            bool horizontalDown = false;

            for (int i = 0; i < 5; i++)
            {
                if (!verticalRight && (y == 9 || (y < 10 - i && BoardCopy[x, y + i] == CheckType.Hit)))
                {
                    if ((y == 9 || (y < 10 - (i + 1) && (BoardCopy[x, y + (i + 1)] == CheckType.Miss || BoardCopy[x, y + (i + 1)] == CheckType.Destroyed))))
                    {
                        verticalRight = true;
                    }
                }
                if (!verticalLeft && (y == 9 || (y < 10 - i && BoardCopy[x, y + i] == CheckType.Hit)))
                {
                    if ((y == 0 || (y >= 0 + (i + 1) && (BoardCopy[x, y - (i + 1)] == CheckType.Miss || BoardCopy[x, y - (i + 1)] == CheckType.Destroyed))))
                    {
                        verticalLeft = true;
                    }
                }
                if (!horizontalUp && (x == 9 || (x < 10 - i && BoardCopy[x + i, y] == CheckType.Hit)))
                {

                    if ((x == 9 || (x < 10 - (i + 1) && (BoardCopy[x + (i + 1), y] == CheckType.Miss || BoardCopy[x + (i + 1), y] == CheckType.Destroyed))))
                    {
                        horizontalUp = true;
                    }
                }
                if (!horizontalDown && (x == 9 || (x < 10 - i && BoardCopy[x + i, y] == CheckType.Hit)))
                {
                    if ((x == 0 || (x >= 0 + (i + 1) && (BoardCopy[x - (i + 1), y] == CheckType.Miss || BoardCopy[x - (i + 1), y] == CheckType.Destroyed))))
                    {
                        horizontalDown = true;
                    }
                }
            }

            if ((verticalRight && verticalLeft) || (horizontalUp && horizontalDown))
            {
                return true;
            }

            return false;

        }

        //If there's a successful hit in close proximity it will continue
        private bool SuccessfulSkip(int x, int y)
        {
            if (((y == 9 || BoardCopy[x, y + 1] != CheckType.None) || (y == 0 || BoardCopy[x, y - 1] != CheckType.None))
                || ((x == 9 || BoardCopy[x + 1, y] != CheckType.None) || (x == 0 || BoardCopy[x - 1, y] != CheckType.None)))
            {

                if(HasCompletedEnd(x, y))
                {
                    return false;
                }

                if ((y != 9 && BoardCopy[x, y + 1] == CheckType.Hit) || (y != 0 && BoardCopy[x, y - 1] == CheckType.Hit)
                    || ((x != 9 && BoardCopy[x + 1, y] == CheckType.Hit) || (x != 0 && BoardCopy[x - 1, y] == CheckType.Hit)))
                {
                    return true;
                }
            }
            return false;
        }

        //Returns a random guess direction
        private string CorrectGuess(int x, int y)
        {
            while (true)
            {
                int randomDirection = random.Next(1, 5);
                if (randomDirection == 1 && x != 9)
                {
                    if (x != 9 && BoardCopy[x + 1, y] == CheckType.None)
                    {
                        return DisplayMove((char)(y + 'A') + ((char)(x + 1 + '0')).ToString());
                    }
                }
                else if (randomDirection == 2 && x != 0)
                {
                    if (BoardCopy[x - 1, y] == CheckType.None)
                    {
                        return DisplayMove((char)(y + 'A') + ((char)(x - 1 + '0')).ToString());
                    }
                }
                if (randomDirection == 3 && y != 9)
                {
                    if (BoardCopy[x, y + 1] == CheckType.None)
                    {
                        return DisplayMove((char)(y + 1 + 'A') + ((char)(x + '0')).ToString());
                    }
                }
                else if (randomDirection == 4 && y != 0)
                {
                    if (BoardCopy[x, y - 1] == CheckType.None)
                    {
                        return DisplayMove((char)(y - 1 + 'A') + ((char)(x + '0')).ToString());
                    }
                }
            }
        }

        //Basically just sleeps the thread so that it isn't over in 1 nanosecond.
        private string DisplayMove(string move)
        {
            Console.WriteLine(move);
            System.Threading.Thread.Sleep(750);
            return move;
        }

        private bool IsSurrounded(int x, int y)
        {
            if ((x == 0 || BoardCopy[x - 1, y] != CheckType.None) && (x == 9 || BoardCopy[x + 1, y] != CheckType.None)
                && (y == 0 || BoardCopy[x, y - 1] != CheckType.None) && (y == 9 || BoardCopy[x, y + 1] != CheckType.None))
            {
                return true;
            }
            return false;
        }

        private string SelectRandom()
        {
            char firstChar;
            char secondChar;

            do
            {
                firstChar = (char)('A' + random.Next(0, 10));
                secondChar = (char)('0' + random.Next(0, 10));
            } while (BoardCopy[secondChar - '0', firstChar-'A'] != CheckType.None);


            return firstChar.ToString() + secondChar.ToString();
        }
    }
}
